using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using unityroom.Api;

public class ScoreManager : SingletonMonoBehaviour<ScoreManager>
{
    [SerializeField] private TMP_Text enemyCountText;
    [SerializeField] private TMP_Text remainHealthText;
    [SerializeField] private TMP_Text spentTimeText;
    [SerializeField] private TMP_Text totalScoreText;

    private const float ENEMY_COEFFICIENT = 1f;
    private const float HEALTH_COEFFICIENT = 10f;
    private const float TIME_COEFFICIENT = 1f;
    private const float DURATION = 0.2f;
    
    private int _enemyCount;

    public void AddEnemyCount() => _enemyCount++;

    public int GetScore()
    {
        var total = 0f;
        total += _enemyCount * ENEMY_COEFFICIENT;
        total += GameManager.Instance.GetPlayer().GetCurrentHealth() * HEALTH_COEFFICIENT;
        // total += (int)GameManager.Instance.GameTime.Value * TIME_COEFFICIENT;

        return (int)total;
    }
    
    public async UniTaskVoid ShowScore()
    {
        ResetTransforms();
        
        var health = GameManager.Instance.GetPlayer().GetCurrentHealth();
        // var time = GameManager.Instance.GameTime.Value;
        
        var total = GetScore();
        UnityroomApiClient.Instance.SendScore(1, total, ScoreboardWriteMode.HighScoreDesc);
        
        AnimateText(enemyCountText, "Enemy:", _enemyCount, ENEMY_COEFFICIENT);
        await enemyCountText.DOScale(1, DURATION).SetEase(Ease.OutBack).SetUpdate(true);

        await UniTask.Delay(500, DelayType.UnscaledDeltaTime);

        AnimateText(remainHealthText, "Health:", health, HEALTH_COEFFICIENT);
        await remainHealthText.DOScale(1, DURATION).SetEase(Ease.OutBack).SetUpdate(true);
        
        await UniTask.Delay(500, DelayType.UnscaledDeltaTime);
        
        // AnimateText(spentTimeText, "Time:", time, TIME_COEFFICIENT);
        // await spentTimeText.DOScale(1, DURATION).SetEase(Ease.OutBack).SetUpdate(true);

        await UniTask.Delay(500, DelayType.UnscaledDeltaTime);

        AnimateText(totalScoreText, "Total:", total, 1);
        await totalScoreText.DOScale(1, DURATION).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void ResetTransforms()
    {
        enemyCountText.transform.localScale = Vector3.zero;
        remainHealthText.transform.localScale = Vector3.zero;
        spentTimeText.transform.localScale = Vector3.zero;
        totalScoreText.transform.localScale = Vector3.zero;
    }
    
    private static void AnimateText(TMP_Text text, string header, float count, float coefficient)
    {
        var currentValue = 0f;
        DOTween.To(() => currentValue, x => currentValue = x, count, 0.5f)
            .OnUpdate(() =>
            {
                text.text = $"{header,-1} {currentValue:F1} x {coefficient:F1} = {(int)(currentValue * coefficient),1}";
            }).SetUpdate(true);
    }
}
