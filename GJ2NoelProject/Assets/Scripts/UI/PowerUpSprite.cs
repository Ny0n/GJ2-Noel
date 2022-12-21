using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PowerUpSprite : MonoBehaviour
{
    [HideInInspector] public PowerUpScroll.PowerUpType CurrentType = PowerUpScroll.PowerUpType.None;
    [HideInInspector] public Image Image;

    private void Start()
    {
        Image = GetComponent<Image>();
    }

    public void SetPowerUp(PowerUpScroll.PowerUp PowerUp)
    {
        CurrentType = PowerUp.Type;
        GetComponent<Image>().sprite = PowerUp.Sprite;
        transform.localScale = Vector3.one * PowerUp.Scale;
    }
}
