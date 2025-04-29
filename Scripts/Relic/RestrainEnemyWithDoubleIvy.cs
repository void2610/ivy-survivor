using UnityEngine;
using R3;

public class RestrainEnemyWithDoubleIvy : RelicBase
{
    protected override void SubscribeEffect()
    {
        EventManager.OnEnemyInDoubleIvy.Subscribe(EffectImpl).AddTo(this);
        IvyManager.Instance.MaxIvyLength.Value -= 10;
    }

    protected override void EffectImpl(Unit _)
    {
        var e = EventManager.OnEnemyInDoubleIvy.GetValue();
        e.Restrain().Forget();
    } 
}