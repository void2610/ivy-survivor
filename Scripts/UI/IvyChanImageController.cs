using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.UI;

public class IvyChanImageController : MonoBehaviour
{
    public enum IvyChanState
    {
        HighHp,
        MediumHp,
        LowHp,
        Damaged,
        Clear,
        GameOver,
    }

    [Serializable]
    public class IvyChanImageData
    {
        public List<Sprite> sprites;
        public int framesPerSecond;
    }
    
    [SerializeField] private Image ivyChanImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private SerializableDictionary<IvyChanState, IvyChanImageData> ivyChanImages;
    [SerializeField] private SerializableDictionary<IvyChanState, Sprite> backgroundImages;
    
    private float _timer;
    private int _currentFrame;
    private IvyChanState _currentState;
    private IvyChanImageData _currentImageData;
    private int _previousHp = 100;
    
    public void SetState(IvyChanState state)
    {
        if (_currentState == state) return;

        _currentState = state;
        _currentImageData = ivyChanImages[state];
        _currentFrame = 0;
        _timer = 0f;
        
        if (_currentState is not (IvyChanState.Damaged or IvyChanState.Clear or IvyChanState.GameOver))
            backgroundImage.sprite = backgroundImages[_currentState];
    }
    
    private async UniTaskVoid OnChangePlayerHealth(int currentHealth)
    {
        var p = currentHealth / 100f;
        UpdateState(p);
        
        // 0.5秒間だけダメージ状態にする
        if(currentHealth < _previousHp)
            SetState(IvyChanState.Damaged);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        // ダメージ状態から元の状態に戻す
        UpdateState(p);
        _previousHp = currentHealth;
    }

    private void UpdateState(float p)
    {
        switch (p)
        {
            case > 0.5f:
                SetState(IvyChanState.HighHp);
                break;
            case > 0.2f:
                SetState(IvyChanState.MediumHp);
                break;
            default:
                SetState(IvyChanState.LowHp);
                break;
        }
    }

    private void Start()
    {
        SetState(IvyChanState.HighHp);
        
        GameManager.Instance.GetPlayer().CurrentHealth.Subscribe(v => OnChangePlayerHealth(v).Forget()).AddTo(this);
    }
    
    private void Update()
    {
        _currentImageData = ivyChanImages[_currentState];

        // 切り替えタイミングを計算（Time.deltaTimeで経過時間を加算）
        var framesPerSecond = _currentImageData.framesPerSecond;
        var sprites = _currentImageData.sprites;
        _timer += Time.deltaTime;
        // 指定したフレームレートの間隔になったらフレーム更新
        if (_timer >= 1f / framesPerSecond)
        {
            // 経過時間分を引くことでわずかなずれも補正（累積しないように）
            _timer -= 1f / framesPerSecond;
            
            // 次のフレームへ（ループ再生）
            _currentFrame = (_currentFrame + 1) % sprites.Count;
            
            // SpriteRendererのスプライトを更新
            ivyChanImage.sprite = sprites[_currentFrame];
        }


    }
}
