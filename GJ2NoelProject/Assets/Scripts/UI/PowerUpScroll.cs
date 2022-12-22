using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PowerUpScroll : GenericSingleton<PowerUpScroll>
{
    public enum PowerUpType
    {
        None,
        Mushroom,
        Snowball,
        bad_gift,
    }

    [Serializable]
    public struct PowerUp
    {
        public PowerUpType Type;
        public Sprite Sprite;
        public float Scale;
    }

    [SerializeField] private List<PowerUp> PowerUps;
    [SerializeField] private GameObject ItemPrefab;
    [SerializeField] private Transform ItemPrefabParent;

    [Header("Scrolling")]
    [SerializeField] private float _startVelocity = 2000.0f;
    [SerializeField] private float _minVelocity = 400.0f;
    [SerializeField] private float _velocityDecreaseSpeed = 400f;
    [SerializeField] private float _velocityInputSlowAmount = 200f;
    [SerializeField] private float _offset = 80;
    
    private bool _hasItem => _currentSprite != null;
    private PowerUpType _currentItem => _hasItem ? _currentSprite.CurrentType : PowerUpType.None;
    private PowerUpSprite _currentSprite;
    
    private bool _isSelectingItem = false;
    private List<PowerUpSprite> _sprites = new List<PowerUpSprite>();
    private int _currentRollCount = 0;

    private float _currentVelocity;
    private float _CurrentVelocity
    {
        get => _currentVelocity;
        set => _currentVelocity = Mathf.Clamp(value, _minVelocity, _startVelocity);
    }
    
    private bool _startWaitingForCenterItem = false;
    private PowerUpSprite _centerItem;

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.I))
        // {
        //     DestroyCurrentItem();
        //     StartSelectRandomItem();
        // }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SlowDownScrollSpeed();
        }
    }

    public void SlowDownScrollSpeed()
    {
        _CurrentVelocity -= _velocityInputSlowAmount;
    }

    public PowerUpType GetAndDestroyCurrentItem()
    {
        if (_hasItem)
        {
            PowerUpType toReturn = _currentItem;
            DestroyCurrentItem();
            return toReturn;
        }

        return PowerUpType.None;
    }

    public bool DestroyCurrentItem()
    {
        if (_hasItem)
        {
            Destroy(_currentSprite.gameObject);
            _currentSprite = null;
            return true;
        }

        return false;
    }

    public void StartSelectRandomItem()
    {
        if (_isSelectingItem || _hasItem)
            return;
        
        _sprites.Clear();
        foreach (var item in PowerUps)
        {
            PowerUpSprite sprite = GenerateSpriteGO(item);
            if (sprite != null)
                _sprites.Add(sprite);
        }
        _currentRollCount = _sprites.Count;

        if (_currentRollCount <= 0)
            return;

        if (_currentRollCount < 2)
        {
            _sprites.Add(GenerateSpriteGO(_sprites[0].CurrentPowerUp));
        }
        
        // shuffle
        _sprites = _sprites.OrderBy(x => Random.value).ToList();

        _isSelectingItem = true;
        
        AudioManager.Instance.Play("box_scroll");
        
        StartCoroutine(nameof(SelectItemCoroutine));
        StartCoroutine(nameof(ScrollCoroutine));
    }
    
    private IEnumerator SelectItemCoroutine()
    {
        _CurrentVelocity = _startVelocity;

        while (_CurrentVelocity > _minVelocity)
        {
            _CurrentVelocity -= _velocityDecreaseSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        _startWaitingForCenterItem = true;
    }

    private void NewCenterItem(PowerUpSprite sprite)
    {
        _centerItem = sprite;

        if (_startWaitingForCenterItem)
        {
            _currentSprite = _centerItem;
            EndSelectRandomItem();
        }
    }
    
    private IEnumerator ScrollCoroutine()
    {
        // init positions of sprites
        for (int i = 0; i < _sprites.Count; i++)
        {
            Transform spriteTransform = _sprites[i].transform;
            spriteTransform.localPosition = new Vector3(0, _offset*i, 0);
        }
        
        while (true)
        {
            // scroll them down, go back up then too low
            foreach (var sprite in _sprites)
            {
                Transform spriteTransform = sprite.transform;
                float prevPos = spriteTransform.localPosition.y;
                spriteTransform.localPosition += new Vector3(0, -_CurrentVelocity*Time.deltaTime, 0);

                if (prevPos > 0 && spriteTransform.localPosition.y < 0)
                {
                    NewCenterItem(sprite);
                }

                float delta = spriteTransform.localPosition.y + _offset;
                if (delta <= 0)
                {
                    Vector3 position = spriteTransform.localPosition;
                    position.y = _offset * (_currentRollCount - 1) + delta;
                    spriteTransform.localPosition = position;
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void EndSelectRandomItem()
    {
        _startWaitingForCenterItem = false;
        StopCoroutine(nameof(ScrollCoroutine));
        
        AudioManager.Instance.StopPlaying("box_scroll");
        AudioManager.Instance.Play("item_obtain");
        
        foreach (var sprite in _sprites)
        {
            if (sprite != _currentSprite)
                Destroy(sprite.gameObject);
        }
        
        _currentSprite.transform.localPosition = Vector3.zero;
        
        _isSelectingItem = false;
        
        print("Item: " + _currentItem);
    }
    
    private PowerUpSprite GenerateSpriteGO(PowerUp Item)
    {
        if (Item.Type != PowerUpType.None)
        {
            PowerUpSprite sprite = Instantiate(ItemPrefab, ItemPrefabParent).GetComponent<PowerUpSprite>();
            if (sprite)
            {
                sprite.SetPowerUp(Item);
                return sprite;
            }
        }

        return null;
    }
}
