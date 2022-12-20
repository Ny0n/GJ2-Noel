using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCursorFactory : MonoBehaviour
{
    [SerializeField] private GameObject _prefabCursor;
    public static MiniMapCursorFactory Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null) 
        {
            Instance = this;
        }
        else 
        {
            Destroy(this);
        }
    }

    public void GetCursor( Transform toFollow) 
    {
        GameObject go = Instantiate(_prefabCursor,transform.position, Quaternion.identity,transform);
        IconFollower follower = go.GetComponent<IconFollower>();
        follower.SetFollow(toFollow);
    }
}
