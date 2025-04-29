using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;
using unityroom.Api;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public enum GameStateType
    {
        BeforeStart,
        StageMoving,
        RelicSelect,
        Growing,
        Flowering,
        Defensing,
        GameOver,
        StageClear,
        Clear,
    }
    
    [SerializeField] private Player player;
    [SerializeField] private IvyChanImageController ivyChanImageController;
    [SerializeField] private RelicSelectUI relicSelectUI;
    
    public readonly ReactiveProperty<GameStateType> GameState = new (GameStateType.BeforeStart);
    public float TimeScale { get; private set; } = 1.0f;
    public bool IsGameOver { get; private set; }
    
    public Player GetPlayer() => player;
    public void ChangeState(GameStateType newStateType) => ChangeStateAsync(newStateType).Forget();
    
    private async UniTaskVoid ChangeStateAsync(GameStateType newStateType)
    {
        GameState.Value = newStateType;
        switch (GameState.Value)
        {
            case GameStateType.BeforeStart:
                break;
            case GameStateType.StageMoving:
                UIManager.Instance.EnableCanvasGroup("RelicSelect", false);
                await StageManager.Instance.NextStage();
                ChangeState(GameStateType.Growing);
                break;
            case GameStateType.RelicSelect:
                UIManager.Instance.EnableCanvasGroup("RelicSelect", true);
                relicSelectUI.SetRandomItem(false);
                break;
            case GameStateType.Growing:
                await UniTask.Delay(1000);
                EventManager.OnGrowingStart.Trigger(0);
                IvyManager.Instance.StartGrowing();
                break;
            case GameStateType.Flowering:
                EventManager.OnFloweringStart.Trigger(0);
                break;
            case GameStateType.Defensing:
                EventManager.OnDefensingStart.Trigger(0);
                FlowerManager.Instance.StartAttacking();
                EnemyManager.Instance.StartEnemySpawn();
                SeManager.Instance.PlaySe("whistle", pitch:1.0f, important:true);
                break;
            case GameStateType.GameOver:
                IsGameOver = true;
                UIManager.Instance.SetIsClear(false);
                UIManager.Instance.EnableCanvasGroup("GameOver", true);
                ScoreManager.Instance.ShowScore().Forget();
                ivyChanImageController.SetState(IvyChanImageController.IvyChanState.GameOver);
                SeManager.Instance.PlaySe("gameover", pitch:1.0f, important:true);
                Time.timeScale = 0f;
                break;
            case GameStateType.StageClear:
                SeManager.Instance.PlaySe("clear", pitch:1.0f, important:true);
                await UIManager.Instance.UpdatePhaseText(GameStateType.StageClear);
                await UniTask.Delay(1000);
                if(StageManager.Instance.IsLastStage())
                    ChangeState(GameStateType.Clear);
                else
                    ChangeState(GameStateType.RelicSelect);
                
                break;
            case GameStateType.Clear:
                Time.timeScale = 0f;
                UIManager.Instance.SetIsClear(true);
                UIManager.Instance.EnableCanvasGroup("GameOver", true);
                ScoreManager.Instance.ShowScore().Forget();
                ivyChanImageController.SetState(IvyChanImageController.IvyChanState.Clear);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void ChangeTimeScale()
    {
        if(!Application.isEditor) return;
        
        TimeScale = TimeScale < 10.0f ? 10.0f : 1.0f;
        Time.timeScale = TimeScale;
    }
    
    protected override void Awake()
    {
        base.Awake();
        DOTween.SetTweensCapacity(tweenersCapacity: 800, sequencesCapacity: 800);
        Time.timeScale = 1.0f;
        TimeScale = 1.0f;
    }
    
    private void Start()
    {
        ChangeState(GameStateType.BeforeStart);
        UIManager.Instance.EnableCanvasGroup("RelicSelect", true);
        relicSelectUI.SetRandomItem(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            UIManager.Instance.OnClickPauseButton();
        if (Input.GetKeyDown(KeyCode.T))
            ChangeTimeScale();
        if (Input.GetKeyDown(KeyCode.Q))
            UIManager.Instance.OnClickTutorialButton();
        
        if (GameState.Value == GameStateType.Defensing)
        {
            if (Input.GetKeyDown(KeyCode.Space)) player.UseSkill().Forget();
        }
    }
}
