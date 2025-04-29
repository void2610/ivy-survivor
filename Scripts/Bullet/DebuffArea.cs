using UnityEngine;

public class DebuffArea : BulletBase
{
    private void Start()
    {
        this.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.7f);
    }
    
    protected override void OnCollideWithEnemy(EnemyBase enemy)
    {
        enemy.DefenseDown().Forget();
        // 固定ダメージ
        enemy.TakeDamage(5);
        // Destroy(this.gameObject);
    }
}
