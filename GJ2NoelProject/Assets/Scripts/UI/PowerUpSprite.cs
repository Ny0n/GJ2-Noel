using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PowerUpSprite : MonoBehaviour
{
    public PowerUpScroll.PowerUpType CurrentType => CurrentPowerUp.Type;
    
    [HideInInspector] public PowerUpScroll.PowerUp CurrentPowerUp;

    public void SetPowerUp(PowerUpScroll.PowerUp PowerUp)
    {
        CurrentPowerUp = PowerUp;
        
        GetComponent<Image>().sprite = CurrentPowerUp.Sprite;
        transform.localScale = Vector3.one * CurrentPowerUp.Scale;
    }
}
