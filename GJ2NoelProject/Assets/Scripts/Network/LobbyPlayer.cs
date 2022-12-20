using System.Collections;
using System.Collections.Generic;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LobbyPlayer : MonoBehaviour
{
    [SerializeField] private TextMeshPro _playerName;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private GlowColor _notReadyColor;
    [SerializeField] private GlowColor _readyColor;

    [Space(10)] 
    [Header("Skins")] 
    [SerializeField] private List<GameObject> _skinsGo;

    private LobbyPlayerData _data;
    
    [System.Serializable]
    private struct GlowColor
    {
        public Color color;
        [ColorUsage(true, true)] public Color emissionColor;
    }
    
    private void ChangeMaterial(GlowColor glowColor)
    {
        _renderer.material.color = glowColor.color;
        _renderer.material.SetColor("_EmissionColor", glowColor.emissionColor);
    }
    public void SetData(LobbyPlayerData data)
    {
        _data = data;
        _playerName.text = _data.Gamertag;

        ChangeSkin(data.SkinIndex);
        
        if (_data.IsReady)
        {
            if (_renderer != null)
            {
                ChangeMaterial(_readyColor);
            }
        }
        else
        {
            if (_renderer != null)
            {
                ChangeMaterial(_notReadyColor);
            }
        }
        
        gameObject.SetActive(true);
    }

    private void ChangeSkin(int skinIndex)
    {
        for (var i = 0; i < _skinsGo.Count; i++)
        {
            _skinsGo[i].SetActive(i == skinIndex);
        }
    }
}
