using R3;

public class GetOrangeFlower : RelicBase
{
    protected override void SubscribeEffect()
    {
        FlowerManager.Instance.AddFlower("Orange");
    }
    
    protected override void EffectImpl(Unit _) {} 
}