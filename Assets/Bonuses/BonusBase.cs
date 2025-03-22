using System;
using System.Collections;
using Characters.Scripts;
using UnityEngine;

public abstract class BonusBase : MonoBehaviour
{
    public abstract float Rarity { get; }
    
    public abstract BonusType BonusType { get; }
    
    protected abstract string Name { get; }
    
    protected abstract Action<CharacterBase, CharacterBase> OnPickUp { get; }
    
    protected bool IsPositive;
    
    private const float BonusLifespan = 15.0f;

    private const string BallLayerName = "Ball";
    
    private int _ballLayer;
    
    private BasketballController _ballController;
    private const string BallTag = "Ball";
    
    private GameController _gameController;
    private const string GameControllerTag = "GameController";

    void Start()
    {
        _ballLayer = LayerMask.NameToLayer(BallLayerName);
        _ballController = GameObject.FindWithTag(BallTag).GetComponent<BasketballController>();
        _gameController = GameObject.FindWithTag(GameControllerTag).GetComponent<GameController>();
    }

    public void StartBonusInstance(Vector3 position, bool isPositive)
    {
        transform.position = position;
        IsPositive = isPositive;
        StartCoroutine(DestroyBonus());
    }
    
    private IEnumerator DestroyBonus()
    {
        yield return new WaitForSeconds(BonusLifespan);
        if (gameObject)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != _ballLayer)
        {
            return;
        }

        var touchedBy = _ballController.lastTouchCharacter;
        
        var otherPlayer = _gameController.player1Instance.GetInstanceID() == touchedBy.GetInstanceID()
            ? _gameController.player2Instance
            : _gameController.player1Instance;

        OnPickUp(touchedBy, otherPlayer);

        Destroy(gameObject);
    }
}

public enum BonusType
{
    Common,
    PlayerBased
}