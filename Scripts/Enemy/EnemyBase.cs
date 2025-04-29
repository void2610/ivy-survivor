using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [SerializeField] private bool isBossEnemy;
    [SerializeField] private int maxHealth;
    [SerializeField] private int attack;
    [SerializeField] private float interval;
    [SerializeField] private float speed;
    
    private int _currentHealth;
    private bool _isDead;
    private bool _isDefenceDown;
    private Collider2D _collider;
    private bool _isSkillUsed;
    private float _defaultScale;
    
    public int GetCurrentHealth() => _currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsDead() => _isDead;
    public bool IsDefenceDown() => _isDefenceDown;
    public void SetSkillUsed() => _isSkillUsed = true;
    public bool IsSkillUsed() => _isSkillUsed;
    
    public int GetCollideIvyCount()
    {
        if (!_collider) _collider = GetComponent<Collider2D>();
        var overlappingColliders = new List<Collider2D>();
        var filter = new ContactFilter2D
        {
            useTriggers   = true,
            useLayerMask  = true,
            layerMask     = LayerMask.GetMask("Ivy")
        };

        // Collider2D.Overlap で指定レイヤーのみ取得
        _collider.Overlap(filter, overlappingColliders);
        return overlappingColliders.Count;
    }
    
    public async UniTaskVoid Restrain()
    {
        var tmp = speed;
        speed = 0;
        await UniTask.Delay(3000);
        speed = tmp;
    }
    
    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;
        
        if (_isDefenceDown) damage *= 2;
        _currentHealth -= damage;
        
        // 演出
        var m = this.GetComponent<SpriteRenderer>().material;
        m.DOColor(new Color(0.7960784314f, 0.168627451f, 0.168627451f), 0).OnComplete(() =>
        {
            m.DOColor(Color.white, 0.3f);
        });
        SeManager.Instance.PlaySe("enemyDamage");
        
        if (_currentHealth <= 0)
        {
            Die();
        }
        _isSkillUsed = false;
    }
    
    public void Heal(int healAmount)
    {
        _currentHealth += healAmount;
        if (_currentHealth > maxHealth)
        {
            _currentHealth = maxHealth;
        }
    }
    
    public void Die()
    {
        if (_isDead) return;
        
        _isDead = true;
        ScoreManager.Instance.AddEnemyCount();
        EventManager.OnEnemyDefeated.Trigger(this);
        
        if(isBossEnemy) StageManager.Instance.EndStage();
        
        Destroy(gameObject);
    }

    public async UniTaskVoid SpeedDown(float speedDown)
    {
        var tmp = speed;
        speed = speed * speedDown * 0.7f;
        await UniTask.Delay(2000);
        speed = tmp;
    }

    public async UniTaskVoid DefenseDown()
    {
        if (_isDefenceDown) return;
        _isDefenceDown = true;
        await UniTask.Delay(2000);
        _isDefenceDown = false;
    }
    
    private async UniTaskVoid StartAttackLoop()
    {
        var cancellationToken = this.GetCancellationTokenOnDestroy();
        while (true)
        {
            Attack();
            await UniTask.Delay((int)(interval * 1000), cancellationToken: cancellationToken);
        }
    }
    
    protected virtual void Attack()
    {
        var t = GetNearestTarget();
        t?.TakeDamage(attack);
    }

    [CanBeNull]
    private IDamageable GetNearestTarget()
    {
        if (_collider == null) _collider = GetComponent<Collider2D>();
        var overlappingColliders = new List<Collider2D>();
        var filter = new ContactFilter2D
        {
            useTriggers   = true,
            useLayerMask  = true,
            layerMask     = LayerMask.GetMask("Flower", "Player")
        };

        // Collider2D.Overlap で指定レイヤーのみ取得
        _collider.Overlap(filter, overlappingColliders);
        foreach (var col in overlappingColliders)
            if (col.TryGetComponent<IDamageable>(out var id) && !id.IsDead()) return id;
        return null;
    }

    protected void Start()
    {
        _currentHealth = maxHealth;
        _defaultScale = transform.localScale.x;
        StartAttackLoop().Forget();
    }

    protected void FixedUpdate()
    {
        var target = FlowerManager.Instance.GetNearestFlower(transform.position);
        var dir = target - this.transform.position;
        if(dir.magnitude < 0.1f) return;
        
        dir.Normalize();
        this.transform.Translate(dir * (speed * Time.fixedDeltaTime * GameManager.Instance.TimeScale * 0.5f));
        
        // 移動速度によってスプライトを反転
        transform.localScale = dir.x > 0 ? 
            new Vector3(-_defaultScale, transform.localScale.y, transform.localScale.z) :
            new Vector3(_defaultScale, transform.localScale.y, transform.localScale.z);
    }
}
