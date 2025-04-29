using R3;

public class GetPurpleFlower : RelicBase
{
    protected override void SubscribeEffect()
    {
        FlowerManager.Instance.AddFlower("Purple");
    }
    
    protected override void EffectImpl(Unit _) {} 
}