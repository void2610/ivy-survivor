using R3;

public class HealWhenDefeatEnemyInIvy : RelicBase
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
            GameManager.Instance.GetPlayer().Heal(1);
        }
    } 
}