using UnityEngine;

public class OrbitBullet : BulletBase
{
    [SerializeField] private float orbitRadius = 1f;
    private Vector2 _originPos;
    private float _angle;

    public override void Init(int damage, float speed, float lifeTime)
    {
        base.Init(damage, speed, lifeTime);
        _originPos = transform.position;
    }
    
    protected override void OnCollideWithEnemy(EnemyBase enemy)
    {
        enemy.TakeDamage(Damage);
        // Destroy(this.gameObject);
    }
    
    private void Update()
    {
        _angle += Speed * Time.deltaTime * 2f;
        var offset = new Vector2(Mathf.Cos(_angle), Mathf.Sin(_angle));
        transform.position = _originPos + offset * orbitRadius;
    }
}
