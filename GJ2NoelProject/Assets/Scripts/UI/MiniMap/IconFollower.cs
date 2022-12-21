using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconFollower : MonoBehaviour
{
    private Transform _toFollow;

    public void SetFollow(Transform target) 
    {
        _toFollow = target;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = _toFollow.position;
        pos = _toFollow.position;
        pos.y = transform.position.y;
        transform.position = pos;

        Vector3 rotation = transform.eulerAngles;
        rotation.z = _toFollow.eulerAngles.z;
        transform.LookAt(_toFollow.position + _toFollow.forward, Vector3.up);
    }
}
