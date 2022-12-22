using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkBehviourActive : NetworkBehaviour
{
    [SerializeField] private MonoBehaviour _mbehaviour;
    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
            _mbehaviour.enabled = true;
    }
}
