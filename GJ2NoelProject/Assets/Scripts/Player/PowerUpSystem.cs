using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PowerUpSystem :NetworkBehaviour
{
    [SerializeField] private PowerUp[] _possiblePowerUP;
    private PowerUpScroll _powerUpScroll;

    private void Start()
    {
        _powerUpScroll = PowerUpScroll.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Gift")
            return;
        if(IsOwner)
            GetPowerUp();

        other.gameObject.SetActive(false);
        StartCoroutine(ReActivate(other.gameObject));
    }

    public void GetPowerUp()
    {
        AudioManager.Instance.Play("box_hit");
        _powerUpScroll.StartSelectRandomItem();
    }

    public void UsePowerUp() 
    {
        PowerUpScroll.PowerUpType type = _powerUpScroll.GetAndDestroyCurrentItem();
        if (type == PowerUpScroll.PowerUpType.None)
            return;
        foreach(PowerUp power in _possiblePowerUP) 
        {
            if (power.type == type)
                power.DoTheThing();
        }
    }

    IEnumerator ReActivate(GameObject go) 
    {
        yield return new WaitForSeconds(5f);

        go.SetActive(true);
    }
}
