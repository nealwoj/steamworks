using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;
using System.Linq;
using TMPro;

public class LobbyController : MonoBehaviour
{
    public static LobbyController Instance { get; private set; }

    const int MAX_PLAYERS = 2;

    //UI
    public GameObject lobbyName, rightIcon, leftIcon, rightName, leftName, inviteButton, readyButton;

    [HideInInspector]
    public GameObject localplayer;
    public ulong lobby_id;

    private CustomNetworkManager manager;

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

    // Start is called before the first frame update
    void Start()
    {
        lobby_id = SteamLobby.Instance.lobby_id;
        Time.timeScale = 1f;

        readyButton.SetActive(false);
        inviteButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //called when someone joins a lobby
    public void UpdateLobby(string username)
    {
        lobbyName.SetActive(true);
        lobbyName.GetComponent<TextMeshProUGUI>().text = username + "'s Lobby";

        if (Manager.playerList.Count >= MAX_PLAYERS)
            StartMatch();
    }
    public void UpdateScreen()
    {
        leftIcon.GetComponent<RawImage>().texture = SteamLobby.Instance.GetHost().icon;
        leftName.GetComponent<TextMeshProUGUI>().text = SteamLobby.Instance.GetHost().player_name.ToString();

        if (Manager.playerList.Count >= MAX_PLAYERS)
        {
            rightIcon.GetComponent<RawImage>().texture = SteamLobby.Instance.GetGuest().icon;
            rightName.GetComponent<TextMeshProUGUI>().text = SteamLobby.Instance.GetGuest().player_name.ToString();
        }
    }

    public void StartMatch()
    {
        inviteButton.SetActive(false);
        readyButton.SetActive(true);
        Time.timeScale = 0f;
    }

    public void InviteFriends()
    {
        //SteamFriends.ActivateGameOverlay("Friends");
        SteamFriends.ActivateGameOverlayInviteDialog((CSteamID)lobby_id);
    }

    public void ReadyUp()
    {
        localplayer.GetComponent<PlayerObjectController>().ReadyUp();
    }
}
