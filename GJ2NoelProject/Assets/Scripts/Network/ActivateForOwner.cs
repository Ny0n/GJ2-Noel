using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ActivateForOwner :NetworkBehaviour
{
    [SerializeField] private GameObject go;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner)
        {
            go.SetActive(false);
        }
        else
            go.SetActive(true);
    }
}
