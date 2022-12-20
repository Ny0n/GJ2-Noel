using System;
using System.Collections.Generic;
using DelegateToolBox;
using UnityEngine;

public class KonamiCodes : MonoBehaviour
{
    [Serializable]
    private struct CodeData // Create codes in the unity editor
    {
        public string Name;
        public List<KeyCode> Inputs;
    }
    
    private class Code
    {
        public Code(CodeData codeData)
        {
            _codeData = codeData;
        }
        
        private readonly CodeData _codeData;
        private int _progress;

        public void CheckForNextKey()
        {
            if (_codeData.Inputs.Count <= 0)
                return;
            
            if (_progress >= _codeData.Inputs.Count)
            {
                OnFinished();
                return;
            }
            
            if (Input.GetKeyDown(_codeData.Inputs[_progress]))
            {
                OnGoodInput();
                
                if (_progress == _codeData.Inputs.Count)
                    OnFinished();
            }
            else
            {
                if (_progress > 0)
                    OnBreak();
                
                OnBadInput();
            }
        }

        private void OnGoodInput()
        {
            _progress++;
            
            print("Progress KonamiCode: \"" + _codeData.Name + "\", " + _progress + "/" + _codeData.Inputs.Count);
        }
        
        private void OnFinished()
        {
            _progress = 0;
            
            print("Finished KonamiCode: \"" + _codeData.Name + "\"");
            
            OnCodeCompleted?.Invoke(_codeData.Name);
        }
        
        private void OnBadInput()
        {
            _progress = 0;
        }
        
        private void OnBreak()
        {
            print("Broke KonamiCode: \"" + _codeData.Name + "\"");
        }
    }
    
    // ****************************************** //
    
    public static Consumer<string> OnCodeCompleted; // <== Bind actions to this event
    /*
     
        -- Example: --
        
        OnCodeCompleted += delegate(string codeName)
        {
            if (codeName == "...")
            {
                // do stuff
            }
        };
            
     */

    [SerializeField] private List<CodeData> Codes;
    private List<Code> _codes = new();
    
    private void Start()
    {
        foreach (var code in Codes)
        {
            _codes.Add(new Code(code));
        }
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            CheckCodes();
        }
    }
    
    private void CheckCodes()
    {
        foreach (var code in _codes)
        {
            code.CheckForNextKey();
        }
    }
    
}