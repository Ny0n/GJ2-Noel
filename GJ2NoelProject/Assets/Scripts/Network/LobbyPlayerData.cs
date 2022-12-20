using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Network
{
    public class LobbyPlayerData
    {
        private string _id;
        private string _gamertag;
        private bool _isReady;
        private int _currentSkin;

        public string Id => _id;
        public string Gamertag => _gamertag;

        public int SkinIndex
        {
            get => _currentSkin;
            set => _currentSkin = value;
        }
        
        public bool IsReady
        {
            get => _isReady;
            set => _isReady = value;
        }
        public void Initialize(string id, string gamertag, int skin = 0)
        {
            _id = id;
            _gamertag = gamertag;
            _currentSkin = skin;
        }

        public void Initialize(Dictionary<string, PlayerDataObject> playerData)
        {
            UpdateState(playerData);
        }

        public void UpdateState(Dictionary<string, PlayerDataObject> playerData)
        {
            if (playerData.ContainsKey("Id"))
            {
                _id = playerData["Id"].Value;
            }
            if (playerData.ContainsKey("Gamertag"))
            {
                _gamertag = playerData["Gamertag"].Value;
            }
            if (playerData.ContainsKey("IsReady"))
            {
                _isReady = playerData["IsReady"].Value == "True";
            }
            if (playerData.ContainsKey("Skin"))
            {
                _currentSkin = int.Parse(playerData["Skin"].Value);
            }
        }

        public Dictionary<string, string> Serialize()
        {
            return new Dictionary<string, string>()
            {
                {"Id", _id},
                {"Gamertag", _gamertag},
                {"IsReady", _isReady.ToString()},
                {"Skin", _currentSkin.ToString()}
            };
        }
    }
}
