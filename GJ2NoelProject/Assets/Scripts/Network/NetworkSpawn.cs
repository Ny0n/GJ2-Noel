using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class NetworkSpawn : NetworkBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform[] spawnLocation;
    private int indexSpawned;
    private bool _bjobDone;

    void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }

    private void Awake()
    {
        _bjobDone = false;
    }

    private IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);

        //while (!_bjobDone)
        //{
        //    yield return new WaitForSeconds(5);
        //}

    }

    [ServerRpc(RequireOwnership = false)] //server owns this object but client can request a spawn
    public void SpawnPlayerServerRpc(ulong clientId)
    {
        // Guard
        NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client);
        if (client.OwnedObjects.FirstOrDefault(i => i.GetComponent<PlayerMovement>() != null) != null) return;

        GameObject newPlayer;
        newPlayer = Instantiate(_playerPrefab, spawnLocation[indexSpawned].position, spawnLocation[indexSpawned].rotation);
        NetworkObject netObj = newPlayer.GetComponent<NetworkObject>();
        newPlayer.SetActive(true);
        netObj.SpawnAsPlayerObject(clientId, true);

        //Debug.Log(netObj.name);

        indexSpawned++;
        if (NetworkManager.Singleton.IsServer && clientId == NetworkManager.Singleton.LocalClientId)
        {
            _bjobDone = true;
            netObj = null;
            return;
        }
        SpawnedClientRpc(clientId);
    }

    [ClientRpc]
    public void SpawnedClientRpc(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            _bjobDone = true;
        }
    }
}
