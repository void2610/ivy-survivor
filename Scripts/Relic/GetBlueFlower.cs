using R3;

public class GetBlueFlower : RelicBase
{
    protected override void SubscribeEffect()
    {
        FlowerManager.Instance.AddFlower("Blue");
    }
    
    protected override void EffectImpl(Unit _) {} 
}