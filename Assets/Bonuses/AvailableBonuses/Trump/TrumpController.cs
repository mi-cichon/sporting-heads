using System.Collections;
using UnityEngine;

public class TrumpController : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private float _currentDirection = 1f;
    private bool _isGrounded = true;

    private const float MoveSpeed = 3f;
    private const float JumpForce = 5f;
    private const float MinWaitTime = 1f;
    private const float MaxWaitTime = 2f;

    private const float Lifetime = 15f;

    private int _jumpableLayer;
    private const string JumpableLayerName = "Jumpable";

    private const string BackHoopTag = "BackHoop";

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _jumpableLayer = LayerMask.NameToLayer(JumpableLayerName);
        StartCoroutine(JumpRoutine());
        Destroy(gameObject, Lifetime);
    }

    private void FixedUpdate()
    {
        _rigidbody.linearVelocity = new Vector2(_currentDirection * MoveSpeed, _rigidbody.linearVelocity.y);
    }

    IEnumerator JumpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(MinWaitTime, MaxWaitTime));

            if (!_isGrounded)
            {
                continue;
            }
            
            _rigidbody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            _isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == _jumpableLayer)
        {
            _isGrounded = true;
            return;
        }

        if (collision.gameObject.CompareTag(BackHoopTag))
        {
            _currentDirection *= -1;
        }
    }
}
