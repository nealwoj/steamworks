using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Steamworks;

public class PlayerObjectController : NetworkBehaviour
{
    //synced across the network
    [SyncVar] public int connection_id;
    [SyncVar] public int player_id;
    [SyncVar] public ulong steam_id;
    [SyncVar] public Texture2D icon;

    //essentially a function pointer for when a variable changes
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string player_name;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool ready;

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

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //hooks
    private void PlayerNameUpdate(string oldValue, string newValue)
    {
        //host
        if (isServer)
        {
            this.player_name = newValue;
        }
    }
    private void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            ready = newValue;
        }
        if (isClient)
        {

        }
    }

    public override void OnStartAuthority()
    {
        CmdSetName(SteamFriends.GetPersonaName().ToString());

        gameObject.name = "LocalPlayer";
        LobbyController.Instance.localplayer = gameObject;
        //StartGame("Movement");
    }

    public override void OnStartClient()
    {
        Manager.playerList.Add(this);
        LobbyController.Instance.UpdateScreen();
    }

    public override void OnStopClient()
    {
        Manager.playerList.Remove(this);
    }

    //command functions get sent out to every client
    [Command]
    private void CmdSetName(string name)
    {
        //update player name
        this.PlayerNameUpdate(this.player_name, name);
    }
    [Command]
    private void CmdStartGame(string scene)
    {
        //update scene
        Manager.StartGame(scene);
    }
    [Command]
    private void CmdSetReady()
    {
        //changes ready status of player to each client
        this.PlayerReadyUpdate(this.ready, !this.ready);
    }

    //runs the command function if host
    public void StartGame(string scene)
    {
        if (isOwned)
        {
            CmdStartGame(scene);
        }
    }
    public void ReadyUp()
    {
        if (isOwned)
        {
            CmdSetReady();
        }
    }
}
