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

    private const string PickedUpText = "{0} picked up{1} {2}";
    private const string PositiveBonusText = " +";
    private const string NegativeBonusText = " -";
    private const float PickedUpTextDuration = 2.0f;

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
        if (collision.gameObject.layer != _ballLayer || !_ballController.lastTouchCharacter)
        {
            return;
        }

        var touchedBy = _ballController.lastTouchCharacter;
        
        var otherPlayer = _gameController.player1Instance.GetInstanceID() == touchedBy.GetInstanceID()
            ? _gameController.player2Instance
            : _gameController.player1Instance;

        OnPickUp(touchedBy, otherPlayer);

        var stateText = BonusType == BonusType.PlayerBased
            ? IsPositive
                ? PositiveBonusText
                : NegativeBonusText
            : string.Empty;

        var pickedUpText = string.Format(PickedUpText, touchedBy.CharacterName, stateText, Name);
        
        _gameController.ShowText(pickedUpText, PickedUpTextDuration);

        Destroy(gameObject);
    }
}

public enum BonusType
{
    Common,
    PlayerBased
}