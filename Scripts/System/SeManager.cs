using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class SeManager : MonoBehaviour
{
    [System.Serializable]
    public class SoundData
    {
        public string name;
        public AudioClip audioClip;
        public float volume = 1.0f;
    }

    [SerializeField] private AudioMixerGroup seMixerGroup;
    [SerializeField] private SoundData[] soundDatas;

    public static SeManager Instance;
    private readonly AudioSource[] _seAudioSourceList = new AudioSource[20];
    private float _seVolume = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        for (var i = 0; i < _seAudioSourceList.Length; ++i)
        {
            _seAudioSourceList[i] = gameObject.AddComponent<AudioSource>();
            _seAudioSourceList[i].outputAudioMixerGroup = seMixerGroup;
        }
    }

    public float SeVolume
    {
        get => _seVolume;
        set
        {
            _seVolume = value;
            if (value <= 0.0f)
            {
                value = 0.0001f;
            }
            seMixerGroup.audioMixer.SetFloat("SeVolume", Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat("SeVolume", value);
        }
    }

    public void PlaySe(AudioClip clip, float volume = 1.0f, float pitch = 1.0f, bool important = false)
    {
        if (clip == null)
        {
            Debug.LogError("AudioClip could not be found.");
            return;
        }

        // まず空いている AudioSource を探す
        var audioSource = GetUnusedAudioSource();

        // プールが全て埋まっていて取得できなかった場合
        if (audioSource == null)
        {
            if (!important) return;
            // important な SE は強制的に上書き
            audioSource = _seAudioSourceList[0];
            audioSource.Stop();
        }

        // クリップ・ボリューム・ピッチをセットして再生
        audioSource.clip   = clip;
        audioSource.volume = volume;
        audioSource.pitch  = pitch;
        audioSource.Play();
    }

    public void PlaySe(string seName, float volume = 1.0f, float pitch = -1.0f, bool important = false)
    {
        var soundData = soundDatas.FirstOrDefault(t => t.name == seName);
        if (soundData == null)
            return;

        // 空いている AudioSource を探す
        var audioSource = GetUnusedAudioSource();
        if (!audioSource)
        {
            if (!important) return;
            // important な SE は強制的に上書き
            audioSource = _seAudioSourceList[0];
            audioSource.Stop();
        }

        audioSource.clip   = soundData.audioClip;
        audioSource.volume = soundData.volume * volume;
        // pitch 引数が負ならランダムピッチ
        if (pitch < 0f)
            pitch = Random.Range(0.9f, 1.1f);
        audioSource.pitch = pitch;

        audioSource.Play();
    }
    
    public void WaitAndPlaySe(string seName, float time, float volume = 1.0f, float pitch = 1.0f)
    {
        WaitAndPlaySeAsync(seName, time, volume, pitch).Forget();
    }
    
    private async UniTaskVoid WaitAndPlaySeAsync(string seName, float time, float volume = 1.0f, float pitch = 1.0f)
    {
        await UniTask.Delay((int)(time * 1000));
        PlaySe(seName, volume, pitch);
    }

    private AudioSource GetUnusedAudioSource() => _seAudioSourceList.FirstOrDefault(t => t.isPlaying == false);

    private void Start()
    {
        SeVolume = PlayerPrefs.GetFloat("SeVolume", 1.0f);
        seMixerGroup.audioMixer.SetFloat("SeVolume", Mathf.Log10(_seVolume) * 20);
    }
}
