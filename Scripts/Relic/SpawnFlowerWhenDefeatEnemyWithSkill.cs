using System;
using R3;

public class SpawnFlowerWhenDefeatEnemyWithSkill : RelicBase
{
    protected override void SubscribeEffect()
    {
        EventManager.OnEnemyDefeated.Subscribe(EffectImpl).AddTo(this);
    }

    protected override void EffectImpl(Unit _)
    {
        var e = EventManager.OnEnemyDefeated.GetValue();
        if (e.IsSkillUsed())
        {
            if(UnityEngine.Random.Range(0f, 1f) > 0.3f) return;
            
            // 30%の確率で花を出現させる
            FlowerManager.Instance.SpawnRandomFlower(e.transform.position);
        }
    } 
}