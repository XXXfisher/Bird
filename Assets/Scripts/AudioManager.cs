using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("音乐设置")]
    public AudioMixer myMixer;
    public AudioClip bgmClip;

    public AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.playOnAwake = false;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 播放当前场景音乐
        if (bgmClip != null)
        {
            audioSource.clip = bgmClip;
            audioSource.Play();
        }

        //// 从保存数据中恢复音量
        //if (PlayerPrefs.HasKey("MusicVolume"))
        //{
        //    float volume = PlayerPrefs.GetFloat("MusicVolume");
        //    myMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        //}
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

}