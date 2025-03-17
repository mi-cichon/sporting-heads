using System;
using System.Collections;
using Characters.Scripts;
using UnityEngine;

public class ZaldCharacter : CharacterBase
{
    protected override float Speed => 5.0f;
    
    protected override float JumpForce => 5.0f;
    
    protected override float HandRotationSpeed => 500.0f;

    public override string CharacterName => "Olive Oiler";
    
    public override string CharacterDescription => "Olive Oiler is a fast paced player, jumping high, moving quickly.";

    public override string SuperPowerDescription => "Olive Oiler becomes incredibly fast, or his rival is just lazy.";

    public override float SuperPowerCooldown => 60.0f;

    protected override Action UseSuperPower => ApplySlow;
    
    private const float SlowFactor = 0.3f;
    private const float SpeedFactor = 2f;
    private const float SlowDuration = 5f;

    private void ApplySlow()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            if (player != gameObject)  // Avoid slowing down self
            {
                StartCoroutine(SlowDownPlayer(player));
            }
        }
    }

    private IEnumerator SlowDownPlayer(GameObject target)
    {
        var instance = target.GetComponent<CharacterBase>();

        if (instance != null)
        {
            instance.speedModifier = SlowFactor;
        }

        speedModifier = SpeedFactor;
        
        yield return new WaitForSeconds(SlowDuration);
        
        if (instance != null)
        {
            instance.speedModifier = 1.0f;
        }
        
        speedModifier = 1.0f;
    }
}
