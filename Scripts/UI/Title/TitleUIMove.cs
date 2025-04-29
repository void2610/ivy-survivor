using System;
using UnityEngine;
using System.Collections.Generic;

public class TitleUIMove : MonoBehaviour
{
    [Serializable]
    public class ParallaxElement
    {
        public RectTransform target;
        public float intensity;
    }
    
    [SerializeField] private float multiplier = 1f;
    [SerializeField] private List<ParallaxElement> elements;
    [SerializeField] private float smoothSpeed = 5f;

    // 各要素の初期localPositionを記録
    private Vector3[] _basePositions;

    private void Awake()
    {
        // elements の数だけ配列を確保し、初期位置をキャッシュ
        _basePositions = new Vector3[elements.Count];
        for (var i = 0; i < elements.Count; i++)
        {
            var e = elements[i];
            if (e.target != null)
                _basePositions[i] = e.target.localPosition;
            else
                _basePositions[i] = Vector3.zero;
        }
    }

    private void Update()
    {
        // マウス位置を正規化（中心が 0、範囲 [-0.5, +0.5]）
        Vector2 m = Input.mousePosition;
        var nx = (m.x / Screen.width)  - 0.5f;
        var ny = (m.y / Screen.height) - 0.5f;

        // マウスと逆方向に動かすベクトル
        var offsetDir = new Vector3(-nx, -ny, 0f);

        // 各要素をループしてスムーズに移動
        for (int i = 0; i < elements.Count; i++)
        {
            var e = elements[i];
            if (!e.target) continue;

            // オフセット量 = 逆方向ベクトル × 強さ
            var offset = offsetDir * (e.intensity * multiplier);

            // 目標位置 = 初期位置 + オフセット
            var targetPos = _basePositions[i] + offset;

            // 滑らかに補間
            e.target.localPosition = Vector3.Lerp(
                e.target.localPosition,
                targetPos,
                Time.deltaTime * smoothSpeed
            );
        }
    }
}