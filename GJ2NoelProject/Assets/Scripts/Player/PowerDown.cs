using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ArcadeKart))]
public class PowerDown : MonoBehaviour
{
    private ArcadeKart kart;
    [SerializeField] private float _powerUpMultiplayer;
    [SerializeField] private float _duration;

    private void Start()
    {
        kart = GetComponent<ArcadeKart>();
    }
    public IEnumerator Stop()
    {
        float currentDuration = _duration;
        while (currentDuration > 0)
        {
            currentDuration -= Time.deltaTime;
            kart.Rigidbody.velocity *= _powerUpMultiplayer;
            yield return null;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!(other.tag == "Trap"))
            return;

        StartCoroutine(Stop());
        Destroy(other.gameObject);
    }
}
