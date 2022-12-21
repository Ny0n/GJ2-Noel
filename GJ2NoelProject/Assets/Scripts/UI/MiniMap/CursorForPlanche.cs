using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorForPlanche : MonoBehaviour
{

    IEnumerator Start()
    {
        while (MiniMapCursorFactory.Instance == null) yield return null;

        MiniMapCursorFactory.Instance.GetCursor(transform);
    }


}
