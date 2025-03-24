using Characters.Scripts;
using System;
using UnityEngine;

public class CarBonus : BonusBase
{
    public CarController LegoCarPrefab;
    public override float Rarity => 0.3f;

    public override BonusType BonusType => BonusType.Common;

    protected override string Name => "Very hard to design!";

    protected override Action<CharacterBase, CharacterBase> OnPickUp => (_, _) => {
        var legoCar = Instantiate(LegoCarPrefab);
        // -1 or 1
        int direction = UnityEngine.Random.Range(0, 2) * 2 - 1;
        legoCar.transform.position = new Vector3(-20f * direction, -3.2f, 0f);
        legoCar.Init(direction);

    };
}
