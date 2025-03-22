using UnityEngine;

public class HandBounce : MonoBehaviour
{
    public float bounceForce = 1.2f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            var ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (ballRb != null)
            {
                var bounceDirection = (collision.transform.position - transform.position).normalized;
                ballRb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
            }
        }
    }
}
