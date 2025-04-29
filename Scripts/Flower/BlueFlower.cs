using UnityEngine;

public class BlueFlower : FlowerBase
{
    protected override void Attack()
    {
        var angle = GetNearestEnemyAngle();
        BulletFactory.Instance.CreateBullet("BlueFlowerBullet", transform.position, angle, data.bulletDamage, data.bulletSpeed, data.bulletLifeTime);
        SeManager.Instance.PlaySe("blueFlower", pitch: 1.0f);
    }
}
