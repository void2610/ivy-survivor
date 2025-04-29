using R3;

public class ReinforceFlowerInSunArea : RelicBase
{
    protected override void SubscribeEffect()
    {
        EventManager.OnFlowerSpawn.Subscribe(EffectImpl).AddTo(this);
    }

    protected override void EffectImpl(Unit _)
    {
        var f = EventManager.OnFlowerSpawn.GetValue();
        if(f.IsInSunArea()) f.Reinforce(1.5f);
    } 
}