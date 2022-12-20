using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ClipboardButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = _text.text;
    }
}
