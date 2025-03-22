using Characters.Scripts;
using UnityEngine;

public class BasketballController : MonoBehaviour
{
    
    private int _playerLayer;
    private int _playerHandLayer;
    private Rigidbody2D _rigidbody;
    
    private const string PlayerLayerName = "Player";
    private const string PlayerHandLayerName = "PlayerHand";

    public CharacterBase lastTouchCharacter;
    public Vector3 lastTouchPosition;
   
    void Start()
    {
        _playerLayer = LayerMask.NameToLayer(PlayerLayerName);
        _playerHandLayer = LayerMask.NameToLayer(PlayerHandLayerName);
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == _playerLayer)
        {
            lastTouchCharacter = collision.gameObject.GetComponent<CharacterBase>();
            lastTouchPosition = transform.position;
            return;
        }
        
        if (collision.gameObject.layer == _playerHandLayer)
        {
            lastTouchCharacter = collision.gameObject.GetComponentInParent<CharacterBase>();
            lastTouchPosition = transform.position;
            return;
        }
    }

    public void FreezeBall(bool state)
    {
        _rigidbody.simulated = !state;
    }
}
