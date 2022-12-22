using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static ArcadeKart;

[RequireComponent(typeof(ArcadeKart))]
public class SpeedBoost : PowerUp
{
    private ArcadeKart kart;
    [SerializeField] private float _powerUpMultiplayer;
    [SerializeField] private float _duration;
    private bool _poweredUp;
    // Start is called before the first frame update
    void Start()
    {
        kart = GetComponent<ArcadeKart>();
    }

    public IEnumerator PowerUp() 
    {
        if (_poweredUp)
            yield break;
        _poweredUp = true;

        PowerUpStatServerRpc();

        yield return new WaitForSeconds(_duration);

        PowerBaseStatServerRpc();

    }

    public override void DoTheThing()
    {
        StartCoroutine(PowerUp());
    }

    [ServerRpc]
    public void PowerUpStatServerRpc()
    {
        Stats stat = kart.baseStats;
        stat.TopSpeed *= _powerUpMultiplayer;
        stat.AccelerationCurve *= _powerUpMultiplayer;
        stat.Acceleration *= _powerUpMultiplayer;
        kart.baseStats = stat;
    }

    [ServerRpc]
    public void PowerBaseStatServerRpc()
    {
        Stats stat = kart.baseStats;
        stat = kart.baseStats;
        stat.TopSpeed /= _powerUpMultiplayer;
        stat.AccelerationCurve /= _powerUpMultiplayer;
        stat.Acceleration /= _powerUpMultiplayer;
        kart.baseStats = stat;
        _poweredUp = false;
    }
}   
