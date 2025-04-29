using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : SingletonMonoBehaviour<EnemyManager>
{
    [Serializable]
    public class StageEnemyData
    {
        public List<GameObject> enemies;
        public GameObject boss;
        public float spawnInterval;
    }
    
    [SerializeField] private List<StageEnemyData> enemyList;
    [SerializeField] private Transform enemyContainer;
    [SerializeField] private Vector2 enemySpawnPosition;
    
    private bool _isBossSpawned;
    private CancellationTokenSource _spawnCts;
    
    public void Reset()
    {
        _isBossSpawned = false;
        foreach (var enemy in enemyContainer.GetComponentsInChildren<EnemyBase>())
        {
            Destroy(enemy.gameObject);
        }
        // 敵スポーンループをキャンセル
        _spawnCts?.Cancel();
    }
    
    public void DamageAllEnemies(int damage)
    {
        foreach (var enemy in enemyContainer.GetComponentsInChildren<EnemyBase>())
        {
            enemy.SetSkillUsed();
            enemy.TakeDamage(damage);
        }
    }

    public EnemyBase GetNearestEnemy(Vector2 origin, float range)
    {
        EnemyBase nearestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (var enemy in enemyContainer.GetComponentsInChildren<EnemyBase>())
        {
            var distance = Vector2.Distance(origin, enemy.transform.position);
            if (distance < range && distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
    
    public void SpawnBoss()
    {
        if (_isBossSpawned) return;
        
        var angle = Random.Range(0, 360);
        var pos = new Vector2(
            enemySpawnPosition.x * Mathf.Cos(angle * Mathf.Deg2Rad),
            enemySpawnPosition.y * Mathf.Sin(angle * Mathf.Deg2Rad)
        );
        var map = StageManager.Instance.GetMap();
        var g = Instantiate(enemyList[map].boss, pos, Quaternion.identity, enemyContainer);
        g.name = "Boss";
        _isBossSpawned = true;
        
        // 最終ステージでは振動させる演出
        if (StageManager.Instance.IsLastStage())
        {
            StageManager.Instance.ChangeLastMapSprite();
            CameraMove.Instance.ShakeCamera(3f, 0.25f);
            SeManager.Instance.PlaySe("bossSpawn", important: true);
        }
    }

    public void StartEnemySpawn()
    {
        // 既存のループが動いていればキャンセル＆破棄
        _spawnCts?.Cancel();
        _spawnCts?.Dispose();
        
        // 新しい CancellationTokenSource を作成
        _spawnCts = new CancellationTokenSource();
        StartEnemySpawnLoop(_spawnCts.Token).Forget();
    }
    
    private async UniTaskVoid StartEnemySpawnLoop(CancellationToken token)
    {
        // ステージによってスポーン間隔を調整
        var m = StageManager.Instance.GetStage() switch
        {
            0 => 1.2f,
            1 => 1.0f,
            2 => 0.8f,
            _ => 1.0f
        };
        var interval = enemyList[StageManager.Instance.GetMap()].spawnInterval * m;
        
        try
        {
            while (!token.IsCancellationRequested)
            {
                var angle = Random.Range(0f, 360f);
                var pos = new Vector2(
                    enemySpawnPosition.x * Mathf.Cos(angle * Mathf.Deg2Rad),
                    enemySpawnPosition.y * Mathf.Sin(angle * Mathf.Deg2Rad)
                );
                SpawnEnemy(pos);

                // キャンセル対応付き Delay
                await UniTask.Delay(
                    TimeSpan.FromSeconds(interval),
                    cancellationToken: token
                );
            }
        }
        catch (OperationCanceledException)
        {
            // キャンセルによる例外は無視
        }
    }
    
    private void SpawnEnemy(Vector2 position)
    {
        var map = StageManager.Instance.GetMap();
        var e = enemyList[map].enemies[Random.Range(0, enemyList[map].enemies.Count)];
        var g = Instantiate(e, position, Quaternion.identity, enemyContainer);
        g.name = "Enemy";
    }
    
    protected override void Awake()
    {
        base.Awake();
    }

    private void OnDestroy()
    {
        // スポーンループをキャンセル
        _spawnCts?.Cancel();
        _spawnCts?.Dispose();
    }
}
