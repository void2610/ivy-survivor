using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class IvyManager : SingletonMonoBehaviour<IvyManager>
{
    [SerializeField] private Sprite ivyLeafLSprite;
    [SerializeField] private Sprite ivyLeafRSprite;
    [SerializeField] private GameObject ivyLeafPrefab;
    [SerializeField] private GameObject ivyTipPrefab;
    [SerializeField] private Transform ivyContainer;
    [SerializeField] private float ivyLeafInterval = 0.5f;
    [SerializeField] private float ivySpeed = 1.0f;
    [SerializeField] private int maxIvyLength = 100;
    
    public readonly ReactiveProperty<int> SunPower = new(0);
    public readonly ReactiveProperty<int> MaxIvyLength = new(100);
    public readonly ReactiveProperty<int> IvyLength = new(0);
    
    private Vector2 _currentDirection;
    private Vector2 _currentPosition;
    private Vector2 _lastLeafPosition;
    private GameObject _ivyTip;
    private Collider2D _ivyTipCollider;
    private int _ivyDamage = 0;
    private bool _isGrowing = false;
    private float _ivyReinforce = 1.0f;
    
    // TODO: 多分ここもバグってる
    public int GetIvyDamage() => (int)((float)_ivyDamage * _ivyReinforce);
    public void AddIvyDamage() => _ivyDamage+= 1;
    
    public void Reinforce(float m) => _ivyReinforce = m;
    
    public float GetIvySpeedDown()
    {
        return 1 / _ivyReinforce;
    }

    public void DestroyAllIvies()
    {
        foreach (var leaf in ivyContainer.GetComponentsInChildren<IvyObject>())
        {
            Destroy(leaf.gameObject);
        }
    }
    
    public float GetIvyWide()
    {
        // 葉が2つ未満の場合は0を返す
        if (ivyContainer.childCount < 2) return 0;
        
        // 一番x座標が大きい葉を取得
        var maxLeaf = ivyContainer.GetComponentsInChildren<IvyObject>()
            .OrderByDescending(leaf => leaf.transform.position.x)
            .FirstOrDefault();
        // 一番x座標が小さい葉を取得
        var minLeaf = ivyContainer.GetComponentsInChildren<IvyObject>()
            .OrderBy(leaf => leaf.transform.position.x)
            .FirstOrDefault();
        
        if (!maxLeaf || !minLeaf) return 0;
        return maxLeaf.transform.position.x - minLeaf.transform.position.x;
    }
    
    public void Reset()
    {
        _ivyTip.transform.position = new Vector3(9999, 9999, 0);
        _currentDirection = Vector2.zero;
        _currentPosition = Vector2.zero;
        _lastLeafPosition = _currentPosition;
        IvyLength.Value = 0;
        // SunPower.Value = 1;
        _isGrowing = false;
    }

    public void StartGrowing()
    {
        _currentDirection = Vector2.up;
        // 成長開始時の位置を初期化（必要に応じて transform.position などを使う）
        _currentPosition = Vector2.zero;
        _lastLeafPosition = _currentPosition;
        _isGrowing = true;
    }

    private void EndGrowing()
    {
        _currentDirection = Vector2.zero;
        GameManager.Instance.ChangeState(GameManager.GameStateType.Flowering);
        _isGrowing = false;
    }

    private bool IsSunArea()
    {
        // 重なっているCollider2Dを格納するリスト
        var overlappingColliders = new List<Collider2D>();

        // フィルタ。ここでフィルター条件を設定することも可能（例: 特定レイヤーのみなど）。
        var filter = new ContactFilter2D { useTriggers = true };
        _ivyTipCollider.Overlap(filter, overlappingColliders);
        return overlappingColliders.Any(col => col.CompareTag("SunArea"));
    }
    
    /// <summary>
    /// Ivyの葉を生成する処理
    /// </summary>
    private void CreateIvyLeaf(Vector2 position, Vector2 direction)
    {
        // 右か左の葉Prefabをランダムで選択
        var isRight = Random.Range(0, 2) == 0;
        var leafSprite =  isRight? ivyLeafRSprite : ivyLeafLSprite;
        var leaf = Instantiate(ivyLeafPrefab, position, Quaternion.identity, ivyContainer);
        leaf.GetComponent<SpriteRenderer>().sprite = leafSprite;
        // ローカル座標に変換して配置
        leaf.transform.localPosition = new Vector3(position.x, position.y, 0);
        // 方向に応じた回転を設定（90度オフセット）
        leaf.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90);
        leaf.transform.localScale = Vector3.one;
        
        leaf.transform.localScale = Vector3.zero;
        leaf.transform.DOScale(1, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
        
        IvyLength.Value++;
    }
    
    protected override void Awake()
    {
        base.Awake();
        _ivyTip = Instantiate(ivyTipPrefab, Vector3.zero, Quaternion.identity);
        _ivyTipCollider = _ivyTip.GetComponent<Collider2D>();
        
        MaxIvyLength.Value = maxIvyLength;
        SunPower.Value = 0;
    }
    
    private void Start()
    {
        _ivyTip.transform.position = new Vector3(9999, 9999, 0);
    }

    private void FixedUpdate()
    { 
        
        if (!_isGrowing) return;
    
        // マウス位置をワールド座標に変換
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // 現在位置との差分から進行方向を計算
        var inputDir = mousePos - _currentPosition;
        if (inputDir.sqrMagnitude > 0.001f)
        {
            _currentDirection = inputDir.normalized;
        }
        
        // 現在位置を更新
        _currentPosition += _currentDirection * (ivySpeed * Time.fixedDeltaTime);
        _ivyTip.transform.localPosition = new Vector3(_currentPosition.x, _currentPosition.y, 0);
        _ivyTip.transform.localRotation = Quaternion.Lerp(
            _ivyTip.transform.localRotation,
            Quaternion.Euler(0, 0, Mathf.Atan2(_currentDirection.y, _currentDirection.x) * Mathf.Rad2Deg - 90),
            Time.fixedDeltaTime * 10f
        );
        
        // 最後の葉生成位置からの移動距離が ivyLeafInterval 以上になったら葉を生成
        if (Vector2.Distance(_currentPosition, _lastLeafPosition) >= ivyLeafInterval)
        {
            CreateIvyLeaf(_currentPosition, _currentDirection);
            _lastLeafPosition = _currentPosition;
            // 日光エリアにいる場合、SunPowerを増加
            if (IsSunArea()) SunPower.Value++;
        }
        
        if (IvyLength.Value >= MaxIvyLength.Value) EndGrowing();
    }



}
