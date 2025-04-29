using R3;

public class GetRedFlower : RelicBase
{
    protected override void SubscribeEffect()
    {
        FlowerManager.Instance.AddFlower("Red");
    }
    
    protected override void EffectImpl(Unit _) {} 
}