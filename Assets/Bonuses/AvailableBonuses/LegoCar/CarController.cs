using UnityEngine;

public class CarController : MonoBehaviour
{
    private int _direction;
    
    private const float MoveSpeed = 12f;
    private const float Lifetime = 15f;
    private const float WypierdolenieForce = 30f;

    private const string PlayerTag = "Player";
    private const string PlayerLayerName = "Player";
    
    private int _playerLayer;

    public void Init(int direction)
    {
        _direction = direction;

        var spriteRenderer = GetComponent<SpriteRenderer>();

        if (_direction < 0)
        {
            spriteRenderer.flipX = true;
        }

        _playerLayer = LayerMask.NameToLayer(PlayerLayerName);

        Destroy(gameObject, Lifetime);
    }

    void FixedUpdate()
    {
        transform.Translate(new Vector2(_direction, 0) * (Time.deltaTime * MoveSpeed));   
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(PlayerTag) && collision.gameObject.layer != _playerLayer)
        {
            return;
        }
        
        var direction = (collision.gameObject.transform.position - gameObject.transform.position).normalized 
                        * WypierdolenieForce;

        var collisionRigidBody = collision.gameObject.GetComponent<Rigidbody2D>();

        if (!collisionRigidBody)
        {
            return;
        }

        collisionRigidBody.AddForce(direction, ForceMode2D.Impulse);
    }
}
