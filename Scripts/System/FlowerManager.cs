using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class FlowerManager : SingletonMonoBehaviour<FlowerManager>
{
    [SerializeField] private SerializableDictionary<FlowerData, GameObject> flowerPrefabs;
    [SerializeField] private Transform flowerContainer;
    
    [SerializeField] private FlowerData redFlower;
    [SerializeField] private FlowerData blueFlower;
    [SerializeField] private FlowerData yellowFlower;
    [SerializeField] private FlowerData orangeFlower;
    [SerializeField] private FlowerData purpleFlower;
    
    public readonly ReactiveProperty<List<FlowerData>> Flowers = new ();
    public readonly ReactiveProperty<int> CurrentFlowerIndex = new (0);
    
    public FlowerData GetCurrentFlowerData()
    {
        if (Flowers.Value.Count == 0) return null;
        return Flowers.Value[CurrentFlowerIndex.Value];
    }
    
    public int GetFlowerTypeCount()
    {
        var count = 0;
        var flowerTypes = new HashSet<string>();
        foreach (var flower in flowerContainer.GetComponentsInChildren<FlowerBase>())
        {
            if (!flowerTypes.Contains(flower.name))
            {
                flowerTypes.Add(flower.name);
                count++;
            }
        }
        return count;
    }
    
    public void DestroyAllFlowers()
    {
        foreach (var flower in flowerContainer.GetComponentsInChildren<FlowerBase>())
        {
            Destroy(flower.gameObject);
        }
    }

    public void AddFlower(string flowerColor)
    {
        var flowerData = GetFlowerDataFromColor(flowerColor);
        
        if (Flowers.Value.Contains(flowerData))
            return;
        
        Flowers.Value.Add(flowerData);
        Flowers.ForceNotify();
        CurrentFlowerIndex.ForceNotify();
    }
    
    public void StartAttacking()
    {
        foreach (var flower in flowerContainer.GetComponentsInChildren<FlowerBase>())
        {
            flower.StartAttack();
        }
    }

    public void SpawnRandomFlower(Vector2 position)
    {
        var flowerData = Flowers.Value[UnityEngine.Random.Range(0, Flowers.Value.Count)];
        SpawnFlower(position, flowerData, false);
    }
    
    private void SpawnFlower(Vector2 position, FlowerData flowerData, bool useCost = true)
    {
        if(useCost && IvyManager.Instance.SunPower.Value < flowerData.cost)
        {
            Debug.Log("SunPowerが足りません: " + flowerData.cost);
            return;
        }
        if (useCost) IvyManager.Instance.SunPower.Value -= flowerData.cost;
        
        var g = Instantiate(flowerPrefabs[flowerData], position, Quaternion.identity, flowerContainer);
        g.name = flowerData.className;

        var type = Type.GetType(flowerData.className);
        var f = g.AddComponent(type) as FlowerBase;
        if (!f)
        {
            Debug.LogError("指定されたクラスは存在しません: " + flowerData.className);
            return;
        }
        
        f.Init(flowerData);
        EventManager.OnFlowerSpawn.Trigger(f);
        
        // 防衛フェーズ中にスポーンしたら攻撃を開始
        if (GameManager.Instance.GameState.Value == GameManager.GameStateType.Defensing) 
            f.StartAttack();
        
        SeManager.Instance.PlaySe("putFlower");

        var minCost = Flowers.Value.Min(x => x.cost);
        if (IvyManager.Instance.SunPower.Value < minCost && GameManager.Instance.GameState.Value == GameManager.GameStateType.Flowering)
            GameManager.Instance.ChangeState(GameManager.GameStateType.Defensing);
    }
    
    private FlowerData GetFlowerDataFromColor(string color)
    {
        return color switch
        {
            "Red" => redFlower,
            "Blue" => blueFlower,
            "Yellow" => yellowFlower,
            "Orange" => orangeFlower,
            "Purple" => purpleFlower,
            _ => throw new ArgumentException("Invalid flower color: " + color)
        };
    }
    
    protected override void Awake()
    {
        base.Awake();
        Flowers.Value = new List<FlowerData> ();
    }

    private void Update()
    {
        if (GameManager.Instance.GameState.Value is not (GameManager.GameStateType.Flowering)) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            // ① マウス位置をワールド座標に変換
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    
            // ② Ivy レイヤーだけを対象に OverlapPoint で当たり判定
            //    Raycast よりもクリック位置ピンポイントに Collider2D を拾える OverlapPoint のほうが扱いやすいです。
            var ivyMask = LayerMask.GetMask("Ivy");
            var ivyHit = Physics2D.OverlapPoint(worldPoint, ivyMask);
    
            // ③ IvyObject を持つものだけに反応
            if (ivyHit && ivyHit.TryGetComponent<IvyObject>(out _))
            {
                SpawnFlower(worldPoint, Flowers.Value[CurrentFlowerIndex.Value]);
            }
        }
    }


    public Vector3 GetNearestFlower(Vector3 transformPosition)
    {
        var nearestFlower = flowerContainer.GetComponentsInChildren<FlowerBase>()
            .Where(f => f.IsInSunArea())
            .OrderBy(f => Vector3.Distance(transformPosition, f.transform.position))
            .FirstOrDefault();
        
        // 近くに花がなければ Vector3.zero を返す
        if (!nearestFlower) return Vector3.zero;
        return nearestFlower.transform.position;
    }
}
