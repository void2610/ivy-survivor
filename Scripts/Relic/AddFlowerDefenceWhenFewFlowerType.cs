using R3;
using UnityEngine;

public class AddFlowerDefenceWhenFewFlowerType : RelicBase
{
    protected override void SubscribeEffect()
    {
        EventManager.OnFlowerSpawn.Subscribe(EffectImpl).AddTo(this);
    }

    protected override void EffectImpl(Unit _)
    {
        var f = EventManager.OnFlowerSpawn.GetValue();
        var flowerCount = FlowerManager.Instance.GetFlowerTypeCount();
        // 花の数が1なら2倍、2なら1.8倍、3なら1.6倍、4なら1.4倍、5なら1.2倍
        f.Reinforce((2.2f - 0.2f * flowerCount));
    } 
}