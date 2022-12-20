using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : GenericSingleton<CursorManager>
{
    [SerializeField] private Texture2D _menuCursor;

    private string _name;

    public string Name
    {
        get => _name;
        set => _name = value;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetMenuCursor();
    }

    public void SetMenuCursor()
    {
        Cursor.SetCursor(_menuCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    public void SetCursorVisibility(bool visible)
    {
        Cursor.visible = visible;
    }
}
