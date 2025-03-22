using System;
using System.Collections;
using UnityEngine;

public class JumpPadController : MonoBehaviour
{
    public float jumpPadCooldown = 5.0f;
    public float jumpPadLaunchForce = 5.0f;
    private bool _isOnCooldown = false;
    
    private readonly Color _jumpPadColorOff = Color.red;
    private readonly Color _jumpPadColorOn = Color.white;

    private SpriteRenderer _jumpPadSprite;

    private void Start()
    {
        _jumpPadSprite = gameObject.GetComponent<SpriteRenderer>();
    }

    public bool LaunchPlayer(Rigidbody2D player)
    {
        if (_isOnCooldown)
        {
            return false;
        }

        if (!player)
        {
            return false;
        }
        
        StartCoroutine(StartCooldown());
        return true;
    }

    private IEnumerator StartCooldown()
    {
        _jumpPadSprite.color = _jumpPadColorOff;
        _isOnCooldown = true;
        
        yield return new WaitForSeconds(jumpPadCooldown);
        
        _isOnCooldown = false;
        _jumpPadSprite.color = _jumpPadColorOn;
    }
}
