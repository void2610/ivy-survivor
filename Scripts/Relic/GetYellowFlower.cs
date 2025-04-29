using R3;

public class GetYellowFlower : RelicBase
{
    protected override void SubscribeEffect()
    {
        FlowerManager.Instance.AddFlower("Yellow");
    }
    
    protected override void EffectImpl(Unit _) {} 
}