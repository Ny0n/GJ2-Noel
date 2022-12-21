using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorForPlanche : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MiniMapCursorFactory.Instance.GetCursor(transform);
    }

}
