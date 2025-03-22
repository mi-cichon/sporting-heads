using System.Collections;
using System.Linq;
using UnityEngine;

public class BonusController : MonoBehaviour
{
    public BonusBase[] bonuses;
    
    private RectTransform _rectTransform;
    
    private const float MinBonusCooldown = 10.0f;
    private const float MaxBonusCooldown = 30.0f;
    
    private void Start()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
        StartCoroutine(StartCreatingBonuses());
    }
    
    private IEnumerator StartCreatingBonuses()
    {
        while (true)
        {
            var randomTime = Random.Range(MinBonusCooldown, MaxBonusCooldown);
            yield return new WaitForSeconds(randomTime);
            CreateRandomBonus();
        }
    }
    
    void CreateRandomBonus()
    {
        var bonus = GetRandomBonus();
        var location = GetRandomBonusLocation();

        var isPositive = bonus.BonusType != BonusType.PlayerBased
                         || Random.Range(0f, 1f) >= 0.5f;

        var bonusInstance = Instantiate(bonus);
        bonusInstance.StartBonusInstance(location, isPositive);
    }
        
    private Vector3 GetRandomBonusLocation()
    {
        var corners = new Vector3[4];
        _rectTransform.GetWorldCorners(corners);
        
        var minX = corners[0].x;
        var maxX = corners[2].x;
        var minY = corners[0].y;
        var maxY = corners[2].y;

        var randomX = Random.Range(minX, maxX);
        var randomY = Random.Range(minY, maxY);

        var localPoint = _rectTransform.InverseTransformPoint(new Vector3(randomX, randomY, -10));
        return localPoint;
    }
    
    private BonusBase GetRandomBonus()
    {
        var totalWeight = bonuses.Sum(x => x.Rarity);

        var randomValue = Random.Range(0f, totalWeight);
        
        var cumulativeWeight = 0f;
        
        foreach (var bonus in bonuses)
        {
            cumulativeWeight += bonus.Rarity;

            if (randomValue <= cumulativeWeight)
            {
                return bonus;
            }
        }

        return bonuses.Last();
    }
}
