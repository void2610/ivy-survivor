using UnityEngine;

public class PurpleFlower : FlowerBase
{
    protected override void Attack()
    {
        BulletFactory.Instance.CreateBullet("PurpleFlowerBullet", transform.position, 0, data.bulletDamage, data.bulletSpeed, data.bulletLifeTime);
        // SeManager.Instance.PlaySe("purpleFlower", pitch: 1.0f);
    }
}
