using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lobbyCodeText;

    [Space(10)] 
    [Header("Ready Button")] 
    [SerializeField] private Button _startButton;
    
    [Space(10)]
    [Header("Ready Button")]
    [SerializeField] private Button _readyButton;
    [SerializeField] private TextMeshProUGUI _readyText;

    [Space(10)]
    [SerializeField] private string _gameSceneName;
    
    private void OnEnable()
    {
        _readyButton.onClick.AddListener(OnReadyPressed);

        if (GameLobbyManager.Instance.IsHost)
        {
            _startButton.onClick.AddListener(OnStartButtonClicked);
            
            GameLobbyEvents.OnLobbyReady += OnLobbyReady;
            GameLobbyEvents.OnLobbyUnReady += OnLobbyUnReady;
        }
    }
    

    private void OnDisable()
    {
        _readyButton.onClick.RemoveAllListeners();
        _startButton.onClick.RemoveAllListeners();
        
        GameLobbyEvents.OnLobbyReady -= OnLobbyReady;
        GameLobbyEvents.OnLobbyUnReady -= OnLobbyUnReady;

    }

    private void Start()
    {
        _lobbyCodeText.text = GameLobbyManager.Instance.GetLobbyCode();
    }

    private async void OnReadyPressed()
    {
        bool isReady = GameLobbyManager.Instance.IsPlayerReady();
        _readyText.text = !isReady ? "Not Ready" : "Ready";
        
        await GameLobbyManager.Instance.SetPlayerReady();
    }
    
    private void OnLobbyReady()
    {
        _startButton.gameObject.SetActive(true);
    }

    private void OnLobbyUnReady()
    {
        _startButton.gameObject.SetActive(false);
    }

    private async void OnStartButtonClicked()
    {
        await GameLobbyManager.Instance.StartGame(_gameSceneName);
    }
}
