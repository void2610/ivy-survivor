using UnityEngine;

public class OrangeFlower : FlowerBase
{
    protected override void Attack()
    {
        var pos = new Vector2(Random.Range(-8f, 8f), Random.Range(-4f, 4f));
        BulletFactory.Instance.CreateBullet("OrangeFlowerBullet", pos, 0, data.bulletDamage, data.bulletSpeed, data.bulletLifeTime);
        SeManager.Instance.PlaySe("orangeFlower", pitch: 1.0f);
    }
}
