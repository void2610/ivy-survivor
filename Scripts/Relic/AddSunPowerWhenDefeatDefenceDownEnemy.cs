using R3;
using UnityEngine;

public class AddSunPowerWhenDefeatDefenceDownEnemy : RelicBase
{
    protected override void SubscribeEffect()
    {
        EventManager.OnEnemyDefeated.Subscribe(EffectImpl).AddTo(this);
    }

    protected override void EffectImpl(Unit _)
    {
        var e = EventManager.OnEnemyDefeated.GetValue();
        if (e.IsDefenceDown())
        {
            Debug.Log($"AddSunPowerWhenDefeatDefenceDownEnemy: {e.name}");
            IvyManager.Instance.SunPower.Value += 1;
        }
    } 
}