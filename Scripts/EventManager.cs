using System;
using R3;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class EventManager
{
    // 成長フェーズ開始時: なし
    public static readonly GameEvent<int> OnGrowingStart = new (0);
    // 開花フェーズ開始時: なし
    public static readonly GameEvent<int> OnFloweringStart = new (0);
    // 防衛フェーズ開始時: なし
    public static readonly GameEvent<int> OnDefensingStart = new (0);
    // 花のスポーン時: 花
    public static readonly GameEvent<FlowerBase> OnFlowerSpawn = new (null);
    // 敵撃破時: 敵 
    public static readonly GameEvent<EnemyBase> OnEnemyDefeated = new (null);
    // 敵が2つ以上のIvyに侵入したとき: 敵
    public static readonly GameEvent<EnemyBase> OnEnemyInDoubleIvy = new (null);

    // ゲーム開始時にイベントをリセット
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetEventManager()
    {
        foreach(var p in typeof(EventManager).GetFields())
        {
            if(p.FieldType == typeof(GameEvent<int>))
            {
                var e = (GameEvent<int>)p.GetValue(null);
                e.ResetAll();
            }
            else if(p.FieldType == typeof(GameEvent<float>))
            {
                var e = (GameEvent<float>)p.GetValue(null);
                e.ResetAll();
            }
            else if(p.FieldType == typeof(GameEvent<bool>))
            {
                var e = (GameEvent<bool>)p.GetValue(null);
                e.ResetAll();
            }
        }
    }
}