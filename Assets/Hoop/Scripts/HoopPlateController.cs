using UnityEngine;

public class HoopPlateController : MonoBehaviour
{
    public float slowDownFactor = 0.5f;
    
    private const string BallTag = "Ball";
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(BallTag))
        {
            return;
        }
        
        var rigidBody = collision.rigidbody; // Get the Rigidbody2D of the colliding object

        if (rigidBody == null)
        {
            return;
        }

        rigidBody.linearVelocity *= slowDownFactor;
        rigidBody.angularVelocity *= slowDownFactor;
    }
}
