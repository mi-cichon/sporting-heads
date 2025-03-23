using System;
using Characters.Scripts;

public class FreezeBonus : BonusBase
{
    public override float Rarity => 1.0f;

    public override BonusType BonusType => BonusType.PlayerBased;
    
    protected override string Name => "Freeze";

    private const float FreezeTime = 3.0f;

    protected override Action<CharacterBase, CharacterBase> OnPickUp => (by, against) =>
    {
        var playerToFreeze = IsPositive ? against : by;
        
        playerToFreeze.FreezeInput(FreezeTime, true);
    };
}
