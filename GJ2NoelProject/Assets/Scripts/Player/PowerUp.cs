using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static PowerUpScroll;

public abstract class PowerUp : NetworkBehaviour
{
    [SerializeField] public PowerUpType type;
    public abstract void DoTheThing();
}
