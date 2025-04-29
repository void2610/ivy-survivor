using UnityEngine;
using DG.Tweening;

public class CameraMove : SingletonMonoBehaviour<CameraMove>
{
    private Vector3 _initPosition;
    
    private void Start()
    {
        // 初期位置を保存
        _initPosition = this.transform.position;
    }

    public void ShakeCamera(float duration, float strength)
    {
        var s = Mathf.Min(7.5f, strength);
        this.transform.DOShakePosition(duration, s, 10, 0, false).OnComplete(() =>
        {
            this.transform.position = _initPosition;
        });
    }
}
