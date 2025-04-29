using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    [SerializeField] private List<Sprite> mapSprites;
    [SerializeField] private SpriteRenderer lastMapSprite;
    [SerializeField] private int mapLength;
    [SerializeField] private int stageLength;
    [SerializeField] private float stageTimeLength = 60f;
    [SerializeField] private SpriteRenderer stageRenderer;
    
    public readonly ReactiveProperty<float> GameTime = new (60f);

    private int _mapCount = 0;
    private int _stageCount = -1;
    private bool _isBossStage = false;
    
    public bool IsBossStage() => _isBossStage;
    public int GetMap() => _mapCount;
    public int GetStage() => _stageCount;
    public bool IsLastStage() => _mapCount == mapLength - 1 && _stageCount == stageLength - 1;
    public void ChangeLastMapSprite() => lastMapSprite.DOFade(1f, 1f);

    public void EndStage()
    {
        GameTime.Value = 0f;
        GameManager.Instance.ChangeState(GameManager.GameStateType.StageClear);
        EnemyManager.Instance.Reset();
        _isBossStage = false;
    }
    
    public async UniTask NextStage()
    {

        
        // 最終マップの最終ステージでクリア
        if(_mapCount == mapLength - 1 && _stageCount == stageLength - 1)
        {
            GameManager.Instance.ChangeState(GameManager.GameStateType.Clear);
        }
        // 最終ステージなら次のマップへ進む
        else if (_stageCount == stageLength - 1)
        {
            await UIManager.Instance.Fade(true);
            _mapCount++;
            _stageCount = -1;
            IvyManager.Instance.DestroyAllIvies();
            FlowerManager.Instance.DestroyAllFlowers();
            stageRenderer.sprite = mapSprites[_mapCount];
        }
        
        // 次が最終ステージならボスステージ
        if (_stageCount == stageLength - 2) _isBossStage = true;

        _stageCount++;
        Debug.Log($"Map: {_mapCount}, Stage: {_stageCount} isBoss: {_isBossStage}");
        
        // 色々リセット
        GameTime.Value = stageTimeLength;
        IvyManager.Instance.Reset();
        SunAreaSpawner.Instance.Reset();
        SunAreaSpawner.Instance.SpawnSunArea();
        BulletFactory.Instance.DestroyAllBullets();
        
        // 演出
        await UIManager.Instance.UpdateStageText(_mapCount, _stageCount);
        await UIManager.Instance.Fade(false);
    }

    private void Update()
    {
        if (GameManager.Instance.GameState.Value == GameManager.GameStateType.Defensing)
        {
            GameTime.Value -= Time.deltaTime * GameManager.Instance.TimeScale;
            if (!_isBossStage && GameTime.Value < 0f)
            {
                EndStage();
            }
            
            // 30秒経ったらボス出現
            if (_isBossStage && GameTime.Value < stageTimeLength - 15f)
            {
                EnemyManager.Instance.SpawnBoss();
            }
        }
    }

}
