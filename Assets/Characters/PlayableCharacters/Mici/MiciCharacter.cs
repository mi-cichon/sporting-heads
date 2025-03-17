using System;
using System.Collections;
using Characters.Scripts;
using UnityEngine;

public class MiciCharacter : CharacterBase
{
    protected override float Speed => 4.0f;
    
    protected override float JumpForce => 5.0f;
    
    protected override float HandRotationSpeed => 400.0f;
    
    public override string CharacterName => "Polish Mountain";
    
    public override string CharacterDescription => "Polish Mountain is a heavy weight player with a planet-like mass.";

    public override string SuperPowerDescription => "Gains his own gravity, pulling nearby objects towards him.";

    public override float SuperPowerCooldown => 60.0f;

    protected override Action UseSuperPower => StartPull;
    
    private Coroutine _pullCoroutine;
    private const float PullDuration = 5f;
    private const float PullForce = 1000f;
    private const float PullRadius = 5f;

    private void StartPull()
    {
        Debug.Log("start pull");
        if (_pullCoroutine != null) StopCoroutine(_pullCoroutine);
        _pullCoroutine = StartCoroutine(PullNearbyObjects());
    }

    private IEnumerator PullNearbyObjects()
    {
        float timer = 0f;

        while (timer < PullDuration)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, PullRadius);

            foreach (var collider in colliders)
            {
                var obj = collider.gameObject;

                if (obj.CompareTag("Player") || obj.CompareTag("Ball"))
                {
                    var rb = obj.GetComponent<Rigidbody2D>();
                    if (rb == null || rb == GetComponent<Rigidbody2D>())
                    {
                        continue;
                    }

                    var direction = (transform.position - (Vector3)rb.position).normalized;
                    rb.AddForce(direction * (PullForce * Time.deltaTime), ForceMode2D.Force);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }
    }

}
