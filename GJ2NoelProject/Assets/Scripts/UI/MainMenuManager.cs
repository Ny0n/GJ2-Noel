using System;
using Network;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Play Menu")]
        [SerializeField] private GameObject playMenu;
        [SerializeField] private Button hostButton;
        [SerializeField] private Button joinButton;
        
        [Header("Join Menu")]
        [SerializeField] private GameObject joinMenu;
        [SerializeField] private Button submitCodeButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TextMeshProUGUI codeText;

        [Header("Name Panel")] 
        [SerializeField] private GameObject _namePanel;
        [SerializeField] private Button _nameSubmit;
        [SerializeField] private TextMeshProUGUI _nameText;

        private const string LOBBY_SCENE = "Lobby";
        
        private void OnEnable()
        {
            hostButton.onClick.AddListener(SetUpNamePanelHost);
            joinButton.onClick.AddListener(SetUpNamePanelJoin);
            
            submitCodeButton.onClick.AddListener(OnSubmitClicked);
        }

        private async void OnSubmitClicked()
        {
            var code = codeText.text;
            code = code.Substring(0, code.Length - 1);

            var succeeded = await GameLobbyManager.Instance.JoinLobby(code);
            if (succeeded)
            {
                SceneManager.LoadSceneAsync(LOBBY_SCENE);
            }

        }

        private void OnDisable()
        {
            hostButton.onClick.RemoveListener(OnHostClicked);
            joinButton.onClick.RemoveListener(OnJoinClicked);
            
            submitCodeButton.onClick.RemoveListener(OnSubmitClicked);
        }
        
        private async void OnHostClicked()
        {
            var succeeded = await GameLobbyManager.Instance.CreateLobby();
            if (succeeded)
            {
                SceneManager.LoadSceneAsync(LOBBY_SCENE);
            }
        }
        
        private void OnJoinClicked()
        {
            playMenu.SetActive(false);
            joinMenu.SetActive(true);
        }

        public void BackButton()
        {
            playMenu.SetActive(true);
            joinMenu.SetActive(false);
        }
        private void RemoveNameSubmitListener()
        {
            _nameSubmit.onClick.RemoveListener(OnJoinClicked);
            backButton.onClick.RemoveListener(RemoveNameSubmitListener);
        }

        private void SetUpNamePanelHost()
        {
            _nameSubmit.onClick.AddListener(OnHostClicked);
        }
        private void SetUpNamePanelJoin()
        {
            _nameSubmit.onClick.AddListener(OnJoinClicked);
            backButton.onClick.AddListener(RemoveNameSubmitListener);
        }

        public void SetName()
        {
            CursorManager.Instance.Name = _nameText.text;
        }
        public void QuitButton()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }
    }
}
