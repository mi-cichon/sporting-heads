using System;
using System.Collections;
using Characters.Scripts;
using UnityEngine;

public class WitkCharacter : CharacterBase
{
    protected override float Speed => 5.0f;
    
    protected override float JumpForce => 2.5f;
    
    protected override float HandRotationSpeed => 500.0f;
    
    public override string CharacterName => "Philosopher Le'go";

    public override string CharacterDescription =>
        "Ultimately intelligent creature. It's mental abilities are beyond human understanding.";

    public override string SuperPowerDescription =>
        "Shares their undeniable opinion about depths of the universe with its enemy, giving them a stroke.";

    public override float SuperPowerCooldown => 60.0f;

    private const float StunDuration = 2.5f;

    protected override Action UseSuperPower => ApplyStun;
    
    private void ApplyStun()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var player in players)
        {
            if (player != gameObject)
            {
                var instance = player.GetComponent<CharacterBase>();
                instance.FreezeInput(StunDuration);
            }
        }
    }
}
