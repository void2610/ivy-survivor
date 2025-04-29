using R3;

public class AddIvyDamage : RelicBase
{
    protected override void SubscribeEffect()
    {
        IvyManager.Instance.AddIvyDamage();
    }
    
    protected override void EffectImpl(Unit _) {} 
}