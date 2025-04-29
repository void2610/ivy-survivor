using R3;

public class AddIvyLength : RelicBase
{
    protected override void SubscribeEffect()
    {
        IvyManager.Instance.MaxIvyLength.Value += 20;
    }
    
    protected override void EffectImpl(Unit _) {} 
}