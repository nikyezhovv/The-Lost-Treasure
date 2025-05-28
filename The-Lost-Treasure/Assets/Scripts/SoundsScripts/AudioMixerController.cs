using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixerController : MonoBehaviour
{
    [Header("Mixer References")]
    [SerializeField] private AudioMixer mainMixer;
    
    [Header("Volume Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundsSlider;
    
    [Header("Exposed Parameters")]
    [SerializeField] private string musicVolumeParam = "MusicVol";
    [SerializeField] private string soundsVolumeParam = "SoundsVol";
    
    private const float MinVolume = -80f;
    private const float MaxVolume = 0f;

    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        soundsSlider.onValueChanged.AddListener(SetSoundsVolume);
    }

    private void Start()
    {
        InitializeSlider(musicSlider, musicVolumeParam);
        InitializeSlider(soundsSlider, soundsVolumeParam);
    }

    private void InitializeSlider(Slider slider, string parameter)
    {
        var savedVolume = PlayerPrefs.GetFloat(parameter, 0.75f);
        slider.value = savedVolume;
        
        if (parameter == musicVolumeParam)
            SetMusicVolume(savedVolume);
        else
            SetSoundsVolume(savedVolume);
    }

    public void SetMusicVolume(float volume)
    {
        SetMixerVolume(musicVolumeParam, volume);
        PlayerPrefs.SetFloat(musicVolumeParam, volume);
    }

    public void SetSoundsVolume(float volume)
    {
        SetMixerVolume(soundsVolumeParam, volume);
        PlayerPrefs.SetFloat(soundsVolumeParam, volume);
    }

    private void SetMixerVolume(string parameter, float normalizedVolume)
    {
        var dbVolume = Mathf.Lerp(MinVolume, MaxVolume, Mathf.Clamp01(normalizedVolume));
        mainMixer.SetFloat(parameter, dbVolume);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}