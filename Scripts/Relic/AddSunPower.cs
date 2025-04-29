using R3;

public class AddSunPower : RelicBase
{
    protected override void SubscribeEffect()
    {
        EventManager.OnGrowingStart.Subscribe(EffectImpl);
    }

    protected override void EffectImpl(Unit _)
    {
        IvyManager.Instance.SunPower.Value += 20;
    } 
}