using UnityEngine;

public class BulletBase : MonoBehaviour
{
    protected int Damage;
    protected float Speed;
    
    public virtual void Init(int damage, float speed, float lifeTime)
    {
        Damage = damage;
        Speed = speed;
        
        Destroy(this.gameObject, lifeTime);
    }
    
    protected virtual void OnCollideWithEnemy(EnemyBase enemy)
    {
        enemy.TakeDamage(Damage);
        Destroy(this.gameObject);
    }
    
    private void Update()
    {
        transform.Translate(Vector2.up * (Speed * Time.deltaTime));
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<EnemyBase>(out var enemy))
        {
            OnCollideWithEnemy(enemy);
        }
    }
}
