using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NameOfPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject _canvas;
    [SerializeField] public NetworkVariable<FixedString64Bytes> NameSync;
    [SerializeField] private TextMeshProUGUI _name;
    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
        {
            NameSetServerRpc(CursorManager.Instance.Name);
            _canvas.SetActive(false);
        }
    }

    [ServerRpc]
    public void NameSetServerRpc(string name) 
    {
        NameSync.Value = name;
        _name.text = name;
    }


    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
            return;
        _name.text = NameSync.Value.ToString();
        _canvas.transform.LookAt(Camera.main.transform.position);
    }
}
