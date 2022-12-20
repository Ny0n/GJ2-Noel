using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ActivateForOwner :NetworkBehaviour
{
    [SerializeField] private GameObject[] gos;
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner)
            return;

        foreach(GameObject go in gos) 
        {
            go.SetActive(true);
        }
    }
}
