using Characters.Scripts;
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CarBonus : BonusBase
{
    public CarController legoCarPrefab;
    
    public override float Rarity => 0.3f;

    public override BonusType BonusType => BonusType.Common;

    protected override string Name => "Very hard to design!";

    private Vector3 _initialPosition = new(-20f, -3.2f, 0f);

    protected override Action<CharacterBase, CharacterBase> OnPickUp => (_, _) => {
        var legoCar = Instantiate(legoCarPrefab);
        var direction = UnityEngine.Random.Range(0, 2) * 2 - 1; //-1 or 1
        _initialPosition.x *= direction;
        legoCar.transform.position = _initialPosition;
        legoCar.Init(direction);

    };
}
