using System;
using System.Collections;
using Characters.Scripts;
using UnityEngine;

public class WelaCharacter : CharacterBase
{
    protected override float Speed => 5.0f;
    
    protected override float JumpForce => 2.5f;
    
    protected override float HandRotationSpeed => 500.0f;
    
    public override string CharacterName => "Senior Fugitive";
    
    public override string CharacterDescription => "Fugitive master of computer science. Expert in hostile takeovers of enemies systems.";

    public override string SuperPowerDescription => "Sabotages enemy control systems";

    public override float SuperPowerCooldown => 45.0f;

    protected override Action UseSuperPower => StartInvert;
    
    private Coroutine _invertCoroutine;
    private const float InvertDuration = 3f;

    private void StartInvert()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            if (player != gameObject)
            {
                StartCoroutine(InvertPlayerControls(player));
            }
        }
    }

    private IEnumerator InvertPlayerControls(GameObject target)
    {
        var instance = target.GetComponent<CharacterBase>();

        instance.speedModifier *= -1;
        
        yield return new WaitForSeconds(InvertDuration);
        
        if (instance != null)
        {
            instance.speedModifier = 1.0f;
        }
    }
}
