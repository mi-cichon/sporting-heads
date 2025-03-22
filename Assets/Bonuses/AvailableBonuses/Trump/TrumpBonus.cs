using System;
using Characters.Scripts;
using UnityEngine;

public class TrumpBonus : BonusBase
{
    public GameObject trumpPrefab;
    public override float Rarity => 0.5f;
    public override BonusType BonusType => BonusType.Common;

    protected override string Name => "The Orange Impostor";

    protected override Action<CharacterBase, CharacterBase> OnPickUp => (_, _) =>
    {
        var trumpInstance = Instantiate(trumpPrefab);
        trumpInstance.transform.position = new Vector3(0f, 0f, -2f);
    };
}
