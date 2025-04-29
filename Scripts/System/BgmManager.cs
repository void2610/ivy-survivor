using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class BgmManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundData
    {
        public AudioClip audioClip;
        public float volume = 1.0f;
    }

    public static BgmManager Instance;
    [SerializeField]
    private bool playOnStart = true;
    [SerializeField]
    private AudioMixerGroup bmgMixerGroup;
    [SerializeField]
    private List<SoundData> bgmList = new List<SoundData>();

    private AudioSource AudioSource => this.GetComponent<AudioSource>();
    private SoundData _currentBGM = null;
    private float _volume = 1.0f;
    private const float FADE_TIME = 1.0f;

    public float BgmVolume
    {
        get => _volume;
        set
        {
            if (value <= 0.0f)
            {
                value = 0.0001f;
            }
            _volume = value;
            PlayerPrefs.SetFloat("BgmVolume", value);
            bmgMixerGroup.audioMixer.SetFloat("BgmVolume", Mathf.Log10(value) * 20);
            AudioSource.volume = _currentBGM?.volume ?? 1;
        }
    }

    public void Resume()
    {
        if (_currentBGM == null) return;

        AudioSource.Play();
        AudioSource.DOFade(_currentBGM.volume, FADE_TIME).SetUpdate(true).SetEase(Ease.InQuad).Forget();
    }

    public void Pause()
    {
        AudioSource.DOFade(0, FADE_TIME).SetUpdate(true).SetEase(Ease.InQuad).OnComplete(() => AudioSource.Stop()).Forget();
    }
    
    public async UniTaskVoid Stop()
    {
        await AudioSource.DOFade(0, FADE_TIME).SetUpdate(true).SetEase(Ease.InQuad).OnComplete(() => AudioSource.Stop());
        _currentBGM = null;
    }

    public async UniTaskVoid PlayRandomBGM()
    {
        if (bgmList.Count == 0) return;

        if (_currentBGM != null)
            await AudioSource.DOFade(0, FADE_TIME).SetUpdate(true).SetEase(Ease.InQuad).OnComplete(() => AudioSource.Stop());

        _currentBGM = bgmList[Random.Range(0, bgmList.Count)];
        AudioSource.clip = _currentBGM.audioClip;
        AudioSource.volume = _currentBGM.volume * _volume;

        AudioSource.Play();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        _currentBGM = null;
        _volume = PlayerPrefs.GetFloat("BgmVolume", 1.0f);
        bmgMixerGroup.audioMixer.SetFloat("BgmVolume", Mathf.Log10(_volume) * 20);
        AudioSource.volume = 0;
        AudioSource.outputAudioMixerGroup = bmgMixerGroup;
        if (playOnStart)
        {
            PlayRandomBGM().Forget();
        }
    }
}
