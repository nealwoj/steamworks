using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class PlayerCanvas : MonoBehaviour
{
    public string player_name;
    public int connection_id;
    public ulong steam_id;
    private bool retrievedIcon;
}
