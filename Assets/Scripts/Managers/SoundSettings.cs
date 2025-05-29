using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    float mastervolume, musicvolume, sfxvolume;

    void Start()
    {
        masterSlider.minValue = 0.0001f;
        musicSlider.minValue = 0.0001f;
        sfxSlider.minValue = 0.0001f;

        audioMixer.GetFloat("Master Volume", out mastervolume);
        audioMixer.GetFloat("Music Volume", out musicvolume);
        audioMixer.GetFloat("SFX Volume", out sfxvolume);

        masterSlider.value = Mathf.Pow(10, mastervolume / 20f);
        musicSlider.value = Mathf.Pow(10, musicvolume / 20f);
        sfxSlider.value = Mathf.Pow(10, sfxvolume / 20f);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        Debug.Log(volume);
        audioMixer.SetFloat("Master Volume", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("Music Volume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX Volume", Mathf.Log10(volume) * 20);
    }
}