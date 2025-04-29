using R3;

public class ReduceSkillTimeWhenDefeatEnemyWithIvy : RelicBase
{
    protected override void SubscribeEffect()
    {
        EventManager.OnEnemyDefeated.Subscribe(EffectImpl).AddTo(this);
    }

    protected override void EffectImpl(Unit _)
    {
        var e = EventManager.OnEnemyDefeated.GetValue();
        if (e.GetCollideIvyCount() > 0)
        {
            GameManager.Instance.GetPlayer().ReduceSkillTime();
        }
    } 
}