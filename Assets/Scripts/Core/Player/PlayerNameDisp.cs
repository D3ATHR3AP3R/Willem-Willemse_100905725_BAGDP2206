using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerNameDisp : MonoBehaviour
{
    [SerializeField] private TMP_Text _DisplayName;
    [SerializeField] private PlayerSettings _Settings;

    private void Start()
    {
        HandleDisplayName(string.Empty, _Settings.playerName.Value);

        _Settings.playerName.OnValueChanged += HandleDisplayName;
    }

    private void HandleDisplayName(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        _DisplayName.text = newName.ToString();
    }

    private void OnDestroy()
    {
        _Settings.playerName.OnValueChanged -= HandleDisplayName;
    }
}
