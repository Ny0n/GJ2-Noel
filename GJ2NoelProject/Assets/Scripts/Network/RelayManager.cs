using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Network;

public class RelayManager : GenericSingleton<RelayManager>
{
    private string _joinCode;
    private string _ip;
    private int _port;
    private byte[] _hostKey;
    private byte[] _clientKey;
    private byte[] _connectionData;
    private byte[] _hostConnectionData;
    private System.Guid _allocationId;


    public string Ip { get => _ip; }
    public int Port { get => _port; }
    public byte[] HostKey => _hostKey;
    public byte[] ClientKey => _clientKey;
    public byte[] HostConnectionData => _hostConnectionData;
    public string GetAllocationId()
    {
        return _allocationId.ToString();
    }

    public byte[] GetAllocationIdByte => _allocationId.ToByteArray();

    public string GetConnectionData()
    {
        return _connectionData.ToString();
    }

    public byte[] GetConnectionDataByte => _connectionData;
    
    public async Task<string> CreateRelay(int maxConnection)
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
        _joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        _hostKey = allocation.Key;

        RelayServerEndpoint dtlsEndpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
        _ip = dtlsEndpoint.Host;
        _port = dtlsEndpoint.Port;

        _allocationId = allocation.AllocationId;
        _connectionData = allocation.ConnectionData;
        

        var transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            
        transport.SetHostRelayData(allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData);
            
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(GameLobbyManager.Instance.GetLobbyMapName, LoadSceneMode.Single);
        
        return _joinCode;
    }

    public async Task<bool> JoinRelay(string joinCode)
    {
        _joinCode = joinCode;
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        
        RelayServerEndpoint dtlsEndpoint = allocation.ServerEndpoints.First(conn => conn.ConnectionType == "dtls");
        _ip = dtlsEndpoint.Host;
        _port = dtlsEndpoint.Port;

        _allocationId = allocation.AllocationId;
        _connectionData = allocation.ConnectionData;

        _hostConnectionData = allocation.HostConnectionData;
        _clientKey = allocation.Key;
        
        var transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
            
        transport.SetClientRelayData(allocation.RelayServer.IpV4, 
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData,
            allocation.HostConnectionData);
            
        NetworkManager.Singleton.StartClient();

        return true;
    }
}
