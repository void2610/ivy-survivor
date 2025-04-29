using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using R3;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;
    [SerializeField] private Image fadeImage;
    [SerializeField] private RectTransform unMask;
    [SerializeField] private Volume volume;
    [SerializeField] private List<CanvasGroup> canvasGroups;
    [SerializeField] private TMP_Text phaseText;
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private Button endFloweringButton;
    [SerializeField] private Image playerBackground;
    [SerializeField] private Image skillGauge;
    [SerializeField] private Image ivyBody;
    [SerializeField] private Image ivyTip;
    [SerializeField] private Vector2 ivyTipPos;
    [SerializeField] private TMP_Text clearOrGameOverText;
    [SerializeField] private TMP_Text sunPowerText;
    [SerializeField] private TMP_Text sunCostText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private List<Transform> flowerInventory;
    public bool IsPaused { get; private set; }
    public bool IsTutorialOpened { get; set; }

    private readonly Dictionary<string, Sequence> _canvasGroupTween = new();
    private Tween _currentFlowerTween;
    private Tween _skillGaugeTween;
    public void EnableCanvasGroup(string canvasName, bool e) => EnableCanvasGroupAsync(canvasName, e).Forget();

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

        _canvasGroupTween[canvasName] = null;
        cg.interactable = e;
        cg.blocksRaycasts = e;
    }

    public void OnClickPauseButton()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0 : GameManager.Instance.TimeScale;
        EnableCanvasGroup("Pause", IsPaused);
    }

    public void OnClickTutorialButton()
    {
        IsTutorialOpened = !IsTutorialOpened;
        Time.timeScale = IsTutorialOpened ? 0 : GameManager.Instance.TimeScale;
        EnableCanvasGroup("Tutorial", IsTutorialOpened);
    }

    public void OnClickSkillButton()
    {
        GameManager.Instance.GetPlayer().UseSkill().Forget();
    }
    
    public void OnClickRetry() => OnClickRetryAsync().Forget();

    private async UniTaskVoid OnClickRetryAsync()
    {
        await IrisOut();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickTitle() => OnClickTitleAsync().Forget();
    
    private async UniTaskVoid OnClickTitleAsync()
    {
        await IrisOut();
        SceneManager.LoadScene("TitleScene");
    }

    public void TweetScore()
    {
        var score = ScoreManager.Instance.GetScore();
        var text = $"「陽なたのアイビー」でスコア: {score}を獲得しました！\n" +
                   $"#unityroom #陽なたのアイビー \n" +
                   $"https://unityroom.com/games/ivy-survivor";
        var url = "https://twitter.com/intent/tweet?text=" + UnityEngine.Networking.UnityWebRequest.EscapeURL(text);
        Application.OpenURL(url);
    }

    public void SetIsClear(bool isClear) => clearOrGameOverText.text = isClear ? "CLEAR!" : "GAME OVER...";

    public async UniTask Fade(bool e)
    {
        if (e) await fadeImage.DOFade(1, 1f).SetUpdate(true);
        else await fadeImage.DOFade(0, 1f).SetUpdate(true);
    }

    private void SetVignette(float value)
    {
        if (!volume.profile.TryGet(out Vignette vignette)) return;
        vignette.intensity.value = value;
    }

    private void UpdateFlowerInventory(List<FlowerData> flowerDataList)
    {
        var flowerCount = flowerDataList.Count;
        for (var i = 0; i < 5; i++)
        {
            if (i < flowerCount)
            {
                flowerInventory[i].GetChild(0).GetComponent<Image>().enabled = true;
                flowerInventory[i].GetChild(0).GetComponent<Image>().sprite = flowerDataList[i].image;
            }
            else
            {
                flowerInventory[i].GetChild(0).GetComponent<Image>().enabled = false;
            }
        }
    }

    public async UniTask UpdateStageText(int map, int stage)
    {
        stageText.DOFade(0, 0).Forget();
        stageText.text = $"ステージ {map + 1} - {stage + 1}";
        await stageText.DOFade(1, 0.25f);
        await UniTask.Delay(1000);
        await stageText.DOFade(0, 0.25f);
        stageText.text = "";
    }

    public async UniTask UpdatePhaseText(GameManager.GameStateType stateType)
    {
        phaseText.DOScale(0,0).Forget();
        
        phaseText.text = stateType switch
        {
            GameManager.GameStateType.Growing => "成長フェーズ",
            GameManager.GameStateType.Flowering => "開花フェーズ",
            GameManager.GameStateType.Defensing => "防衛フェーズ",
            GameManager.GameStateType.StageClear => "ステージクリア",
            _ => ""
        };
        
        await phaseText.DOScale(1, 0.25f).SetEase(Ease.OutBack);
        await UniTask.Delay(1000);
        await phaseText.DOScale(0, 0.25f).SetEase(Ease.InCubic);
        phaseText.text = "";
    }

    private void SetInventoryClickEvent()
    {
        for (var i = 0; i < flowerInventory.Count; i++)
        {
            var idx = i;
            Utils.AddEventToObject(flowerInventory[i].gameObject,
                () =>
                {
                    if (FlowerManager.Instance.Flowers.Value.Count < idx) return;
                    FlowerManager.Instance.CurrentFlowerIndex.Value = idx;
                }, EventTriggerType.PointerClick);
        }
    }

    private void OnChangeCurrentFlowerIndex(int idx)
    {
        if (FlowerManager.Instance.Flowers.Value.Count < idx) return;
        
        // 元のTweenを削除
        if (_currentFlowerTween != null && _currentFlowerTween.IsActive())
        {
            var g = _currentFlowerTween.target;
            if (g is Transform t) t.localScale = Vector3.one;
            _currentFlowerTween.Kill();
            _currentFlowerTween = null;
        }

        flowerInventory[idx].GetChild(0).DOScale(1, 0.2f);
        // 選択された花は大きさをTweenさせる
        _currentFlowerTween = flowerInventory[idx].GetChild(0).DOScale(1.2f, 0.2f).SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);

        // 選択された花のコストを表示
        var data = FlowerManager.Instance.GetCurrentFlowerData();
        sunCostText.text = data != null ? data.cost.ToString() : "--";
        SeManager.Instance.PlaySe("button");
    }

    private void OnChangePlayerHealth(int currentHealth)
    {
        var p = currentHealth / 100f;
        if (p > 0.5f) return;
        SetVignette(1 - p - 0.5f);
    }

    private void OnChangeIvyLength(int currentLength, int maxLength)
    {
        var p = currentLength / (float)maxLength;
        ivyBody.DOFillAmount(p, 0.2f);

        var rt = ivyTip.GetComponent<RectTransform>();
        var targetX = Mathf.Lerp(ivyTipPos.x, ivyTipPos.y, p);
        rt.DOAnchorPosX(targetX, 0.2f);
    }
    
    private async UniTask IrisOut()
    {
        await unMask.DOScale(0.5f, 0.5f).SetEase(Ease.InCubic).SetUpdate(true);
        await unMask.DOScale(1.5f, 0.3f).SetEase(Ease.OutCubic).SetUpdate(true);
        await unMask.DOScale(0, 0.5f).SetEase(Ease.InCubic).SetUpdate(true);
    }

    private async UniTask IrisIn()
    {
        unMask.DOScale(0, 0).SetUpdate(true).Forget();
        await unMask.DOScale(1.5f, 0.5f).SetEase(Ease.OutCubic).SetUpdate(true);
        await unMask.DOScale(0.5f, 0.3f).SetEase(Ease.InCubic).SetUpdate(true);
        await unMask.DOScale(21, 0.5f).SetEase(Ease.OutCubic).SetUpdate(true);
    }

    protected override void Awake()
    {
        base.Awake();
        bgmSlider.value = PlayerPrefs.GetFloat("BgmVolume", 1.0f);
        seSlider.value = PlayerPrefs.GetFloat("SeVolume", 1.0f);

        foreach (var canvasGroup in canvasGroups)
        {
            _canvasGroupTween.Add(canvasGroup.name, null);
            EnableCanvasGroup(canvasGroup.name, canvasGroup.name == "RelicSelect");
        }
        
        IrisIn().Forget();
    }

    private void OnChangeGameTime(float t)
    {
        if (t < 0) return;
        if (StageManager.Instance.IsBossStage())
        {
            timeText.text = "--";
            return;
        }
        timeText.text = t.ToString("F1");
    }
    
    private void OnChangeSkillGauge(float t)
    {
        skillGauge.fillAmount = t / 60f;

        if (t >= 59.9f)
        {
            // スキルゲージが満タンになったら、スキルボタンをTweenさせる
            _skillGaugeTween = skillGauge.GetComponent<RectTransform>().DOScale(1.2f, 0.3f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        }
        else
        {
            _skillGaugeTween?.Kill();
        }
    }

    public void Start()
    {
        IvyManager.Instance.IvyLength.CombineLatest(IvyManager.Instance.MaxIvyLength, (current, max) => (current, max)).Subscribe(t => OnChangeIvyLength(t.current, t.max)).AddTo(this);
        IvyManager.Instance.SunPower.Subscribe(v => sunPowerText.text = v.ToString()).AddTo(this);
        FlowerManager.Instance.Flowers.Subscribe(UpdateFlowerInventory).AddTo(this);
        FlowerManager.Instance.CurrentFlowerIndex.Subscribe(OnChangeCurrentFlowerIndex).AddTo(this);
        GameManager.Instance.GameState.Subscribe(v => UpdatePhaseText(v).Forget()).AddTo(this);
        GameManager.Instance.GameState.Subscribe(v => endFloweringButton.gameObject.SetActive(v == GameManager.GameStateType.Flowering)).AddTo(this);
        StageManager.Instance.GameTime.Subscribe(OnChangeGameTime).AddTo(this);
        GameManager.Instance.GetPlayer().CurrentHealth.Subscribe(OnChangePlayerHealth).AddTo(this);
        GameManager.Instance.GetPlayer().SkillTime.Subscribe(OnChangeSkillGauge).AddTo(this);
        
        bgmSlider.onValueChanged.AddListener(value =>
        {
            BgmManager.Instance.BgmVolume = value;
        });
        seSlider.onValueChanged.AddListener(value =>
        {
            SeManager.Instance.SeVolume = value;
        });

        var trigger = seSlider.gameObject.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        entry.callback.AddListener(_ => SeManager.Instance.PlaySe("button"));
        trigger.triggers.Add(entry);
        
        SetInventoryClickEvent();
        
        endFloweringButton.onClick.AddListener(() => GameManager.Instance.ChangeState(GameManager.GameStateType.Defensing));

        FlowerManager.Instance.CurrentFlowerIndex.Value = 0;
    }
}
