using R3;

public class EnlargeSunArea : RelicBase
{
    protected override void SubscribeEffect()
    {
        SunAreaSpawner.Instance.EnlargeSunArea();
    }
    
    protected override void EffectImpl(Unit _) {} 
}