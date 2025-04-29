using UnityEngine;

public class MeteorBullet : BulletBase
{
    protected override void OnCollideWithEnemy(EnemyBase enemy)
    {
        enemy.TakeDamage(Damage);
        // Destroy(this.gameObject);
    }
}
