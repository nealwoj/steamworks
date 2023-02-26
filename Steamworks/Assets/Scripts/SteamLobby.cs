using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;
using Steamworks;
using TMPro;

public class SteamLobby : MonoBehaviour
{
    public static SteamLobby Instance { get; private set; }

    public bool init;
    public ulong lobby_id;
    public CSteamID steam_id, host_id;
    private const string hostKey = "HostAddress";
    private CustomNetworkManager manager;

    //function pointers for Steamworks
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> joinRequest;
    protected Callback<LobbyEnter_t> lobbyEntered;
    protected Callback<AvatarImageLoaded_t> loadIcon;

    //UI
    public GameObject hostButton;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
                return manager;

            return manager = CustomNetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SteamAPI.Init();

        //check if steam is open
        if (!SteamManager.Initialized)
            return;

        manager = GetComponent<CustomNetworkManager>();

        //set functions to callbacks
        lobbyCreated = Callback<LobbyCreated_t>.Create(CreateLobby);
        joinRequest = Callback<GameLobbyJoinRequested_t>.Create(JoinRequest);
        lobbyEntered = Callback<LobbyEnter_t>.Create(JoinLobby);
        loadIcon = Callback<AvatarImageLoaded_t>.Create(LoadIcons);
    }

    private void Update()
    {

    }

    public void HostPrivate(string scene)
    {
        //create a friends only lobby with max of 2 connections
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 2);

        LobbyController.Instance.inviteButton.SetActive(true);
    }
    public void HostPublic(string scene)
    {
        //create a public lobby with max of 2 connections
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 2);
    }

    private void CreateLobby(LobbyCreated_t callback)
    {
        //checks for Steamworks API/Web API errors
        if (callback.m_eResult != EResult.k_EResultOK)
            return;

        manager.StartHost();

        //set host to current Steam user
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostKey, SteamUser.GetSteamID().ToString());

        //set lobby name
        string name = SteamFriends.GetPersonaName().ToString();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", name);

        host_id = SteamUser.GetSteamID();
        //steam_id = SteamUser.GetSteamID();
        //SteamMatchmaking.SetLobbyOwner((CSteamID)callback.m_ulSteamIDLobby, steam_id);

        Debug.Log("Lobby Created: " + name);
    }
    private void JoinRequest(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);

        host_id = callback.m_steamIDFriend;

        Debug.Log("Join request from: " + SteamFriends.GetPersonaName().ToString());
    }
    private void JoinLobby(LobbyEnter_t callback)
    {
        lobby_id = callback.m_ulSteamIDLobby;
        steam_id = SteamUser.GetSteamID();
        string name = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");

        if (!NetworkServer.active)
        {
            //if not host, start client
            manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostKey);
            manager.StartClient();
        }

        hostButton.SetActive(false);
        LobbyController.Instance.UpdateLobby(name);
        Debug.Log("Joined Lobby: " + name);
    }
    private void LoadIcons(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID == (CSteamID)GetHost().steam_id)
        {
            LobbyController.Instance.leftIcon.GetComponent<RawImage>().texture = SteamImageToTexture(callback.m_iImage);
            LobbyController.Instance.leftName.GetComponent<TextMeshProUGUI>().text = GetHost().player_name.ToString();
        }
        else
        {
            LobbyController.Instance.rightIcon.GetComponent<RawImage>().texture = SteamImageToTexture(callback.m_iImage);
            LobbyController.Instance.rightName.GetComponent<TextMeshProUGUI>().text = GetGuest().player_name.ToString();
        }
    }

    private Texture2D SteamImageToTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }

        return texture;
    }

    public PlayerObjectController GetHost()
    {
        for (int i = 0; i < Manager.playerList.Count; i++)
            if (Manager.playerList[i].connection_id == 0)
                return Manager.playerList[i];

        Debug.LogError("Could not get host!");
        return null;
    }
    public PlayerObjectController GetGuest()
    {
        for (int i = 0; i < Manager.playerList.Count; i++)
            if (Manager.playerList[i].connection_id == 1)
                return Manager.playerList[i];

        Debug.LogError("Could not get guest!");
        return null;
    }
}