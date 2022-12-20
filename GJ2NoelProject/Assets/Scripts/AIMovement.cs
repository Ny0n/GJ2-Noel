using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AIMovement : MonoBehaviour
{
    private float _currentSpeed;
    private float[] _lastHitRayCastDistance;
    private Rigidbody _rigidbody;

    [SerializeField] private float _length;

    [SerializeField] protected Transform _centerOfMass;
    [SerializeField] protected Transform[] _hoverPoints;

    // Start is called before the first frame update
    void Start()
    {
        _currentSpeed = 0;
        _lastHitRayCastDistance = new float[_hoverPoints.Length];

        for (int i = 0; i < _lastHitRayCastDistance.Length; i++)
        {
            _lastHitRayCastDistance[i] = _length;
        }

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = _centerOfMass.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
