using System;
using UnityEngine;
using System.Collections.Generic;
using R3;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class RelicManager : SingletonMonoBehaviour<RelicManager>
{
    [SerializeField] private GameObject relicPrefab;
    [SerializeField] private Transform relicContainer;
    [SerializeField] private Vector3 relicGridPosition;
    [SerializeField] private Vector2 relicOffset;
    [SerializeField] private float relicUISize;
    
    [SerializeField] private List<RelicData> initialFlowerRelics;
    
    [SerializeField] private List<RelicData> allRelics;
    [SerializeField] private List<RelicData> testRelics;
    
    private readonly List<RelicData> _relics = new();
    private readonly List<RelicUI> _relicUIs = new();

    public (RelicData, RelicData, RelicData) GetRandomRelic(bool onlyFlower)
    {
        if (onlyFlower)
        {
            return (initialFlowerRelics[0], initialFlowerRelics[1], initialFlowerRelics[2]);
        }
        
        // 持っていないレリックの中から被り無しでランダムに選ぶ
        var relics = new List<RelicData>();
        
        foreach (var relic in allRelics)
        {
            if (_relics.Contains(relic)) continue;
            relics.Add(relic);
        }
        if (relics.Count < 3)
        {
            Debug.LogError("レリックの数が足りません");
            return (null, null, null);
        }
        var randomRelics = new List<RelicData>();
        for (int i = 0; i < 3; i++)
        {
            var randomIndex = UnityEngine.Random.Range(0, relics.Count);
            randomRelics.Add(relics[randomIndex]);
            relics.RemoveAt(randomIndex);
        }
        return (randomRelics[0], randomRelics[1], randomRelics[2]);
    }
    
    public void AddRelic(RelicData relic)
    {
        _relics.Add(relic);
        CreateRelicUI(relic);
        ApplyEffect(relic);
    }

    private void CreateRelicUI(RelicData r)
    {
        var go = Instantiate(relicPrefab, relicContainer);
        go.transform.localPosition = relicGridPosition + new Vector3(relicOffset.x * _relicUIs.Count, relicOffset.y * _relicUIs.Count, 0);
        go.transform.localScale = new Vector3(relicUISize, relicUISize, 1);
        var relicUI = go.GetComponent<RelicUI>();
        relicUI.SetRelicData(r);
        _relicUIs.Add(relicUI);
    }
    
    private void ApplyEffect(RelicData r)
    {
        var type = System.Type.GetType(r.className);
        var behaviour = gameObject.AddComponent(type) as RelicBase;
        if(!behaviour)
        {
            Debug.LogError("指定されたクラスは存在しません: " + r.className);
            return;
        }
        
        behaviour.Init();
    }

    private void Start()
    {
        if (!Application.isEditor) return;
        foreach (var r in testRelics)
        {
            AddRelic(r);
        }
    }
}
