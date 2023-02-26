using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerObjectController player;

    public List<PlayerObjectController> playerList { get; } = new List<PlayerObjectController>();

    //called everytime a player enters the server, sync through conn
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Movement")
        {
            PlayerObjectController instance = Instantiate(player);

            //setup
            instance.connection_id = conn.connectionId;
            instance.player_id = playerList.Count + 1;
            instance.steam_id = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)SteamLobby.Instance.lobby_id, playerList.Count);
            
            int img = SteamFriends.GetLargeFriendAvatar((CSteamID)instance.steam_id);
            if (img != -1)
                instance.icon = SteamImageToTexture(img);
            else
                instance.icon = Texture2D.redTexture;

            NetworkServer.AddPlayerForConnection(conn, instance.gameObject);
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

    public void StartGame(string scene)
    {
        ServerChangeScene(scene);
    }
}