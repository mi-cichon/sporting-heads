using System;
using Characters.Scripts;

public class ShrinkBonus : BonusBase
{
    public override float Rarity => 1.0f;

    public override BonusType BonusType => BonusType.PlayerBased;
    
    protected override string Name => "Shrink";

    private const float ShrinkTime = 10.0f;
    private const float ShrinkFactor = 0.5f;

    protected override Action<CharacterBase, CharacterBase> OnPickUp => (by, against) =>
    {
        var playerToShrink = IsPositive ? against : by;
        playerToShrink.ChangeSize(ShrinkFactor, ShrinkTime);
    };
}
