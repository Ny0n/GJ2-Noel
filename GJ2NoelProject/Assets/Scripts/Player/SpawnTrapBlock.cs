using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnTrapBlock : PowerUp
{
    [SerializeField] private GameObject _trapBlockPrefab;
    [SerializeField] private Transform _spawnLocation;
    private bool _spawned;

    public override void DoTheThing()
    {
        SpawnServerRpc(_spawnLocation.position);
        Instantiate(_trapBlockPrefab,_spawnLocation.transform.position,Quaternion.identity);
        _spawned=true;
    }

    [ServerRpc(RequireOwnership =false)]
    public void SpawnServerRpc(Vector3 pos) 
    {
        SpawnClientRpc(pos);
    }

    [ClientRpc]
    public void SpawnClientRpc(Vector3 pos)
    {
        if (_spawned)
            return;

        Instantiate(_trapBlockPrefab, pos, Quaternion.identity);
    }
}
