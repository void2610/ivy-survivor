using R3;
using UnityEngine;

public class ReduceSkillTimeWhenIvyRangeWide  : RelicBase
{
    protected override void SubscribeEffect()
    {
        EventManager.OnFloweringStart.Subscribe(EffectImpl);
    }

    protected override void EffectImpl(Unit _)
    {
        var w = IvyManager.Instance.GetIvyWide() / 17f;
        Debug.Log(1.0f + w);
        GameManager.Instance.GetPlayer().SetSkillTimeSpeed(1.0f + w);
    } 
}