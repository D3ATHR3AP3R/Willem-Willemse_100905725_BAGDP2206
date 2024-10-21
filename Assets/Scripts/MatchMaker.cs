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
    public GameObject networkUI;
    public GameObject joinUi;
    public GameObject hostUi;
    public GameObject mainCamera;
    public GameObject cameraPrefab;

    public TMP_InputField joinAddress;
    public TMP_InputField hostAddress;

    public new List<GameObject> cameraLists;

    public UnityTransport.ConnectionAddressData ConnectionData;

    public static MatchMaker instance;

    private void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void AddressLog()
    {
        Debug.Log("Address: " + ConnectionData.Address);
        updateText.text = "Address: " + ConnectionData.Address;
    }

    public void HostUI()
    {
        networkUI.SetActive(false);
        hostUi.SetActive(true);
    }

    public void HostBtn()
    {
        hostUi.SetActive(false);
        Host(hostAddress.text);
    }

    public void Host(string address)
    {
        ConnectionData.Address = address;
        NetworkManager.Singleton.StartHost();
        updateText.text = "I am Host";
        networkUI.SetActive(false);

        Invoke("AddressLog", 1f);
    }

    public void JoinUI()
    {
        networkUI.SetActive(false);
        joinUi.SetActive(true);
    }

    public void JoinBtn()
    {
        joinUi.SetActive(false);
        Join(joinAddress.text);
    }

    public void Join(string address)
    {
        ConnectionData.Address = address;
        NetworkManager.Singleton.StartClient();
        updateText.text = "I am Client";
        networkUI.SetActive(false);

        Invoke("AddressLog", 1f);
    }

    public void CameraAttach(GameObject player, int ownerId)
    {
        if (ownerId == 0)
        {
            mainCamera.transform.SetParent(player.transform);
            player.GetComponent<PlayerController>().hasCam = true;
            cameraLists.Add(mainCamera);
        }
        else
        {
            GameObject newCam = Instantiate(cameraPrefab);
            cameraLists.Add(newCam);
            cameraLists[ownerId].transform.SetParent(player.transform);
            player.GetComponent<PlayerController>().hasCam = true;

            for (int i = 0; i < cameraLists.Count; i++)
            {
                if(i != ownerId)
                {
                    cameraLists[i].GetComponentInChildren<Camera>().enabled = false;
                }
                else
                {
                    cameraLists[i].GetComponentInChildren<Camera>().enabled = true;
                }
            }

        }
    }

    public void UpdateCamera(int ownerId)
    {
        for (int i = 0; i < cameraLists.Count; i++)
        {
            if (i != ownerId)
            {
                cameraLists[i].GetComponentInChildren<Camera>().enabled = false;
            }
            else
            {
                cameraLists[i].GetComponentInChildren<Camera>().enabled = true;
            }
        }
    }
}
