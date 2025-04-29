using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private SkillCutInImage skillCutInImage;
    [SerializeField] private int maxHealth;
    [SerializeField] private float skillNeedTime = 60f;
    [SerializeField] private Transform skillIvy;
    
    public readonly ReactiveProperty<int> CurrentHealth = new(100);
    public readonly ReactiveProperty<float> SkillTime = new(0f);
    private float skillTimeSpeed = 1.0f;
    private bool _isDead;
    
    public int GetCurrentHealth() => CurrentHealth.Value;
    public int GetMaxHealth() => maxHealth;
    public bool IsDead() => _isDead;
    
    public void ReduceSkillTime() => SkillTime.Value += 3f;
    public void SetSkillTimeSpeed(float f) => skillTimeSpeed = f;
    
    public void TakeDamage(int damage)
    {
        CurrentHealth.Value -= damage;
        SkillTime.Value += damage;
        // 演出
        CameraMove.Instance.ShakeCamera(0.1f, 0.1f);
        var m = this.GetComponent<SpriteRenderer>().material;
        m.DOColor(new Color(0.7960784314f, 0.168627451f, 0.168627451f), 0).OnComplete(() =>
        {
            m.DOColor(Color.white, 0.3f);
        });
        SeManager.Instance.PlaySe("playerDamage");

        if (CurrentHealth.Value <= 0)
        {
            CurrentHealth.Value = 0;
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        CurrentHealth.Value += healAmount;
        if (CurrentHealth.Value > maxHealth)
        {
            CurrentHealth.Value = maxHealth;
        }
    }
    
    public void Die()
    {
        _isDead = true;
        GameManager.Instance.ChangeState(GameManager.GameStateType.GameOver);
    }
    
    public async UniTaskVoid UseSkill()
    {
        if (SkillTime.Value < skillNeedTime) return;
        SkillTime.Value = 0f;
        // スキル発動
        SeManager.Instance.PlaySe("skill", important:true);
        await skillCutInImage.StartCutIn();
        SkillIvyAnimation().Forget();
        await UniTask.Delay(1000);
        EnemyManager.Instance.DamageAllEnemies(70);
    }
    
    private async UniTaskVoid SkillIvyAnimation()
    {
        skillIvy.gameObject.SetActive(true);
        skillIvy.localScale = Vector3.zero;
        await skillIvy.DOScale(1, 0.5f).SetEase(Ease.OutCubic);
        await skillIvy.DORotate(new Vector3(0, 0, 360), 1.3f, RotateMode.FastBeyond360).SetEase(Ease.InOutCubic);
        await skillIvy.DOScale(0, 0.5f).SetEase(Ease.OutCubic);
        skillIvy.gameObject.SetActive(false);
        skillIvy.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private void Awake()
    {
        CurrentHealth.Value = maxHealth;
        SkillTime.Value = 0f;
        skillIvy.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        if(GameManager.Instance.GameState.Value != GameManager.GameStateType.Defensing) return;
        
        if (SkillTime.Value < skillNeedTime)
        {
            SkillTime.Value += Time.deltaTime * GameManager.Instance.TimeScale * skillTimeSpeed;
        }
    }
}
