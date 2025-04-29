using UnityEngine;

public class YellowFlower : FlowerBase
{
    protected override void Attack()
    { 
        BulletFactory.Instance.CreateBullet("YellowFlowerBullet", transform.position, 0f, data.bulletDamage, data.bulletSpeed, data.bulletLifeTime);
        SeManager.Instance.PlaySe("yellowFlower", pitch:1.0f);
    }
}
