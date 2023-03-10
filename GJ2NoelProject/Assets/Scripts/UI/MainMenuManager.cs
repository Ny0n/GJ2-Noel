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
        [SerializeField] private Button _back;
        [SerializeField] private TextMeshProUGUI _nameText;
        
        [Header("Other")] 
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _sfxToggle;

        private const string LOBBY_SCENE = "Lobby";
        
        private void OnEnable()
        {
            hostButton.onClick.AddListener(SetUpNamePanelHost);
            joinButton.onClick.AddListener(SetUpNamePanelJoin);
            
            submitCodeButton.onClick.AddListener(OnSubmitClicked);
            
            _musicToggle.onValueChanged.AddListener(delegate(bool value) { AudioManager.Instance.SetActiveMusic(value); });
            _sfxToggle.onValueChanged.AddListener(delegate(bool value) { AudioManager.Instance.SetActiveSFX(value); });
        }

        private void Start()
        {
            _musicToggle.SetIsOnWithoutNotify(AudioManager.Instance.IsMusicActive);
            _sfxToggle.SetIsOnWithoutNotify(AudioManager.Instance.IsSFXActive);
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
            
            _musicToggle.onValueChanged.RemoveAllListeners();
            _sfxToggle.onValueChanged.RemoveAllListeners();
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

        public void JoinPseudoRemoveListener()
        {
            _nameSubmit.onClick.RemoveAllListeners();
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
