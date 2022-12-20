using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnimatingDots : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _tmpObject;    
    [SerializeField] private string _text;
    [Range(0.1f, 10f)] [SerializeField] private float _delay = 0.5f;

    private WaitForSeconds _waitDelay;

    private void Start()
    {
        _waitDelay = new WaitForSeconds(_delay);
        StartCoroutine(AnimationRoutine());
    }

    private IEnumerator AnimationRoutine()
    {
        _tmpObject.text = _text;
        yield return _waitDelay;
        
        _tmpObject.text = _text + ".";
        yield return _waitDelay;
        
        _tmpObject.text = _text + "..";
        yield return _waitDelay;
        
        _tmpObject.text = _text + "...";
        yield return _waitDelay;

        StartCoroutine(AnimationRoutine());
    }
}
