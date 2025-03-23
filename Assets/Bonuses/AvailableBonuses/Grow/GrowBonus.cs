using System;
using Characters.Scripts;

public class GrowBonus : BonusBase
{
    public override float Rarity => 1.0f;

    public override BonusType BonusType => BonusType.PlayerBased;
    
    protected override string Name => "Grow";

    private const float GrowTime = 10.0f;
    private const float GrowFactor = 2.0f;

    protected override Action<CharacterBase, CharacterBase> OnPickUp => (by, against) =>
    {
        var playerToGrow = IsPositive ? by : against;
        playerToGrow.ChangeSize(GrowFactor, GrowTime);
    };
}
