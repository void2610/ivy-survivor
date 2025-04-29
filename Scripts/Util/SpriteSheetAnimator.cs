using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetAnimator : MonoBehaviour
{
    // スプライトシートから切り出したスプライトを配列で設定（Inspectorで設定）
    [SerializeField] private List<Sprite> sprites;
    // 切り替え速度（1秒あたりのフレーム数）
    [SerializeField] private float framesPerSecond = 10f;

    // スプライト切り替えのタイマー用変数
    private float _timer;
    // 現在のフレーム番号
    private int _currentFrame;
    // このオブジェクトのSpriteRendererコンポーネントへの参照
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        if (sprites == null || sprites.Count == 0)
            Debug.LogError("SpriteSheetAnimator: spritesが設定されていません");
    }

    private void Update()
    {
        // 切り替えタイミングを計算（Time.deltaTimeで経過時間を加算）
        _timer += Time.deltaTime;
        // 指定したフレームレートの間隔になったらフレーム更新
        if (_timer >= 1f / framesPerSecond)
        {
            // 経過時間分を引くことでわずかなずれも補正（累積しないように）
            _timer -= 1f / framesPerSecond;
            
            // 次のフレームへ（ループ再生）
            _currentFrame = (_currentFrame + 1) % sprites.Count;
            
            // SpriteRendererのスプライトを更新
            _spriteRenderer.sprite = sprites[_currentFrame];
        }
    }
}