using UnityEngine;

public class RedFlower : FlowerBase
{
    protected override void Attack()
    {
        var angle = GetNearestEnemyAngle();
        BulletFactory.Instance.CreateBullet("RedFlowerBullet", transform.position, angle, data.bulletDamage, data.bulletSpeed, data.bulletLifeTime);
        SeManager.Instance.PlaySe("redFlower");
    }
}
