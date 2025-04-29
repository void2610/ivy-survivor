using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class FlowerBase : MonoBehaviour, IDamageable
{
    public FlowerData data;
    private float _range;
    private float _interval;
    protected int MaxHealth;
    private int _currentHealth;
    private bool _isDead;
    
    public int GetCurrentHealth() => _currentHealth;
    public int GetMaxHealth() => MaxHealth;
    public bool IsDead() => _isDead;
    
    public bool IsInSunArea()
    {
        var col = GetComponent<Collider2D>();
        if (!col) return false;
        var overlappingColliders = new List<Collider2D>();
        var filter = new ContactFilter2D
        {
            useTriggers   = true,
            useLayerMask  = true,
            layerMask     = LayerMask.GetMask("SunArea")
        };

        // Collider2D.Overlap で指定レイヤーのみ取得
        col.Overlap(filter, overlappingColliders);
        return overlappingColliders.Count > 0;
    }
    
    public void Init(FlowerData d)
    {
        data = d;
        _range = d.range;
        _interval = d.interval;
        MaxHealth = d.maxHealth;
        
        _currentHealth = MaxHealth;
        _isDead = false;
    }
    
    public void Reinforce(float m)
    {
        MaxHealth = Mathf.FloorToInt(MaxHealth * m);
        _interval /= m;
        _range *= m;
        _currentHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;
        
        _currentHealth -= damage;
        
        // 演出
        var m = this.GetComponent<SpriteRenderer>().material;
        m.DOColor(new Color(0.7960784314f, 0.168627451f, 0.168627451f), 0).OnComplete(() =>
        {
            m.DOColor(Color.white, 0.3f);
        });
        
        if (_currentHealth <= 0)
        {
            Die();
        } 
    }
    
    public void Heal(int healAmount)
    {
        _currentHealth += healAmount;
        if (_currentHealth > MaxHealth)
        {
            _currentHealth = MaxHealth;
        }
    }
    
    public void Die()
    {
        _isDead = true;
        this.transform.DOScale(0, 0.5f).SetEase(Ease.OutBounce).OnComplete(() => { Destroy(gameObject); });
    }

    public void StartAttack() => StartAttackLoop().Forget();
    
    private async UniTaskVoid StartAttackLoop()
    {
        var cancellationToken = this.GetCancellationTokenOnDestroy();
        while (true)
        {
            await UniTask.WaitUntil(IsEnemyInRange, cancellationToken: cancellationToken);
            Attack();
            await UniTask.Delay((int)(_interval * 1000), cancellationToken: cancellationToken);
        }
    }
    
    protected bool IsEnemyInRange()
    {
        var enemy = EnemyManager.Instance.GetNearestEnemy(this.transform.position, _range);
        if (!enemy) return false;
        return Vector2.Distance(this.transform.position, enemy.transform.position) < _range;
    }
    
    protected float GetNearestEnemyAngle()
    {
        var enemy = EnemyManager.Instance.GetNearestEnemy(this.transform.position, _range);
        return Vector2.SignedAngle(Vector2.up, enemy.transform.position - transform.position);
    }

    protected abstract void Attack();

    private void Start()
    {
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale(1.5f, 0.5f).SetEase(Ease.OutBounce);
    }
}
