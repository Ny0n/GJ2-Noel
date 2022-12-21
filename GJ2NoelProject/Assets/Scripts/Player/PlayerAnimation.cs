using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_rb.velocity.magnitude > 1)
        {
            _animator.SetBool("WheelSpinning", true);
        }
        else
        {
            _animator.SetBool("WheelSpinning", false);
        }
    }
}
