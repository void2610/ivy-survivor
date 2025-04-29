using R3;

public class ReinforceIvyPowerWhenManyFlowerType : RelicBase
{
    protected override void SubscribeEffect()
    {
        EventManager.OnFlowerSpawn.Subscribe(EffectImpl).AddTo(this);
    }

    protected override void EffectImpl(Unit _)
    {
        var flowerCount = FlowerManager.Instance.GetFlowerTypeCount();
        // 花の数が1なら1.2倍、2なら1.4倍、3なら1.6倍、4なら1.8倍、5なら2倍
        var m = 1.2f + 0.2f * flowerCount;
        IvyManager.Instance.Reinforce(m);
    } 
}