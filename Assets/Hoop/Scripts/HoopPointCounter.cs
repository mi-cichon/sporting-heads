using Characters.Scripts;
using UnityEngine;

public class HoopPointCounter : MonoBehaviour
{
    public bool isPlayer2Hoop = false;
    
    private const string BallTag = "Ball";
    private const string PlayerTag = "Player";
    private const string GameControllerTag = "GameController";
    
    private GameController GameControllerInstance;
    
    void Start()
    {
        GameControllerInstance = GameObject.FindWithTag(GameControllerTag).GetComponent<GameController>();
    }

    void Update()
    {
        
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(BallTag))
        {
            GameControllerInstance.PointScored(isPlayer2Hoop);
            return;
        }

        if (other.gameObject.CompareTag(PlayerTag))
        {
            other.gameObject.GetComponent<CharacterBase>().MoveToDefaultPosition();
        }
    }
}
