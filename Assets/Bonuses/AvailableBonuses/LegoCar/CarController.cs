using UnityEngine;

public class CarController : MonoBehaviour
{
    public int Direction;

    [SerializeField]
    private const float MoveSpeed = 12f;
    private const float Lifetime = 15f;
    private const float WypierdolenieForce = 30f;

    public void Init(int direction)
    {
        Direction = direction;

        var spriteRenderer = GetComponent<SpriteRenderer>();

        if (Direction < 0)
        {
            spriteRenderer.flipX = true;
        }

        Destroy(gameObject, Lifetime);
    }

    void FixedUpdate()
    {
        transform.Translate(new Vector2(Direction, 0) * Time.deltaTime * MoveSpeed);   
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COLLISON");

        if (collision.gameObject.tag == "Player" || collision.gameObject.layer == 10)
        {
            Vector2 direction = (collision.gameObject.transform.position - gameObject.transform.position).normalized * WypierdolenieForce;

            var rigidbody = collision.gameObject.GetComponent<Rigidbody2D>();

            if (rigidbody == null)
            {
                return;
            }

            rigidbody.AddForce(direction, ForceMode2D.Impulse);
        }
    }
}
