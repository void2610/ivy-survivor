using R3;

public class EnlargeBullet : RelicBase
{
    protected override void SubscribeEffect()
    {
        BulletFactory.Instance.EnlargeBullet();
        IvyManager.Instance.MaxIvyLength.Value -= 10;
    }
    
    protected override void EffectImpl(Unit _) {} 
}