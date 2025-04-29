using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillCutInImage : MonoBehaviour
{
    [SerializeField] private Ease inEase = Ease.OutSine;
    [SerializeField] private Ease outEase = Ease.InSine;
    [SerializeField] private RectTransform image;
    
    private CanvasGroup _canvasGroup;

    public async UniTask StartCutIn()
    {
        _canvasGroup.alpha = 1;
        // imageを中央に移動させる
        await image.DOAnchorPosX(0, 0.6f).SetEase(inEase);
        // imageを右に移動させる
        await image.DOAnchorPosX(1000, 0.6f).SetEase(outEase);

        // imageを元の位置に戻す
        image.anchoredPosition = new Vector2(-1000, 0);
        _canvasGroup.alpha = 0;
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }
}
