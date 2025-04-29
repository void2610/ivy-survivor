using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.LowLevel;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private RectTransform unMask;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;
    [SerializeField] private List<CanvasGroup> canvasGroups;
    
    private readonly Dictionary<string, Sequence> _canvasGroupTween = new();

    private GameObject GetTopCanvasGroup() => canvasGroups.Find(c => c.alpha > 0)?.gameObject;
    
    public ScrollRect GetActiveScrollRect()
    {
        var topCanvas = GetTopCanvasGroup();
        if (topCanvas)
        {
            var scrollRect = topCanvas.GetComponentInChildren<ScrollRect>();
            if (scrollRect)
            {
                return scrollRect;
            }
        }
        return null;
    }
    
    public void EnableCanvasGroup(string canvasName) => EnableCanvasGroupAsync(canvasName, true).Forget();
    public void DisableCanvasGroup(string canvasName) => EnableCanvasGroupAsync(canvasName, false).Forget();

    private async UniTaskVoid EnableCanvasGroupAsync(string canvasName, bool e)
    {
        var cg = canvasGroups.Find(c => c.name == canvasName);
        if (!cg) return;
        if (_canvasGroupTween[canvasName].IsActive()) return;
        
        // アニメーション中は操作をブロック
        cg.interactable = false;
        cg.blocksRaycasts = false;
        
        var seq = DOTween.Sequence();
        seq.SetUpdate(true).Forget();
        if (e)
        {
            seq.Join(cg.transform.DOMoveY(-0.45f, 0).SetRelative(true)).Forget();
            seq.Join(cg.transform.DOMoveY(0.45f, 0.2f).SetRelative(true).SetEase(Ease.OutBack)).Forget();
            seq.Join(cg.DOFade(1, 0.2f)).Forget();
        }
        else
        {
            seq.Join(cg.DOFade(0, 0.2f)).Forget();
        }
        
        _canvasGroupTween[canvasName] = seq;
        
        await seq.AsyncWaitForCompletion();
        
        // inputGuide.UpdateText(IsAnyCanvasGroupEnabled() ? InputGuide.InputGuideType.Navigate : InputGuide.InputGuideType.Merge);
        _canvasGroupTween[canvasName] = null;
        cg.interactable = e;
        cg.blocksRaycasts = e;
    }
    
    public void StartGame() => StartGameAsync().Forget();
    
    private async UniTaskVoid StartGameAsync()
    {
        fadeImage.color = new Color(0, 0, 0, 0);
        await unMask.DOScale(0.5f, 0.5f).SetEase(Ease.InCubic);
        await unMask.DOScale(1.5f, 0.3f).SetEase(Ease.OutCubic);
        await unMask.DOScale(0, 0.5f).SetEase(Ease.InCubic);
        SceneManager.LoadScene("MainScene");
    }
    
    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }
    
    private static void InitPlayerPrefs()
    {
        PlayerPrefs.SetFloat("BgmVolume", 1.0f);
        PlayerPrefs.SetFloat("SeVolume", 1.0f);
    }

    public void ResetSetting()
    {
        PlayerPrefs.SetFloat("BgmVolume", 1.0f);
        PlayerPrefs.SetFloat("SeVolume", 1.0f);
        bgmSlider.value = 1.0f;
        seSlider.value = 1.0f;
    }

    private void Awake()
    {
        Time.timeScale = 1.0f;
        // ScrollRectの初期化
        foreach(var scrollRect in FindObjectsByType<ScrollRect>(FindObjectsSortMode.None))
        {
            var sr = scrollRect.GetComponentInChildren<ScrollRect>();
            if (sr)
            {
                sr.verticalNormalizedPosition = 1.0f;
                sr.horizontalNormalizedPosition = 0.0f;
            }
        }
        
        // CanvasGroupの初期化
        foreach (var canvasGroup in canvasGroups)
        {
            _canvasGroupTween.Add(canvasGroup.name, null);
            EnableCanvasGroupAsync(canvasGroup.name, false).Forget();
        }
        
        // PlayerPrefsの初期化
        if (!PlayerPrefs.HasKey("BgmVolume")) InitPlayerPrefs();
    }

    private void Start()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("BgmVolume", 1.0f);
        seSlider.value = PlayerPrefs.GetFloat("SeVolume", 1.0f);

        bgmSlider.onValueChanged.AddListener(value => BgmManager.Instance.BgmVolume = value);
        seSlider.onValueChanged.AddListener(value => SeManager.Instance.SeVolume = value);

        var trigger = seSlider.gameObject.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        entry.callback.AddListener(_ => SeManager.Instance.PlaySe("button"));
        trigger.triggers.Add(entry);
        
        Time.timeScale = 1.0f;

        fadeImage.color = new Color(0, 0, 0, 1);
        fadeImage.DOFade(0.0f, 1.0f);
        unMask.DOScale(21, 0);
        
        Debug.Log("TitleMenu Start");
        SeManager.Instance.PlaySe("button");
    }
}
