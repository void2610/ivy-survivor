using R3;
using UnityEngine;

public class HealWhenDefeatEnemyWithSkill : RelicBase
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
            GameManager.Instance.GetPlayer().Heal(1);
        }
    } 
}