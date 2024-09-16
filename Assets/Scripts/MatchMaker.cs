using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System;
using TMPro;
using Object = UnityEngine.Object;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Netcode;
using Unity.Services.Relay.Models;

public class MatchMaker : MonoBehaviour
{
    public TextMeshProUGUI updateText;

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        updateText.text = "I am Host";
    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();
        updateText.text = "I am Client";
    }
}
