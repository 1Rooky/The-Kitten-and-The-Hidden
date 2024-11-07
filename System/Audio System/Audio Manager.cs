using UnityEngine;

public class AudioSystemFmod : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource _masterVolume;

    [SerializeField] private AudioSource _musicSource;
    [SerializeField] internal AudioSource _sFXSource;

    [Header("Audio Clip")]
    [SerializeField] private AudioClip _background;

    [SerializeField] internal AudioClip _walk;
    [SerializeField] internal AudioClip _jump;
    [SerializeField] internal AudioClip _interact;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterService(this, true);

        _masterVolume.volume = PlayerPrefs.GetFloat("MasterVolume", 1);
        _musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1);
        _sFXSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
    }

    private void Start() => PlayBackground();

    internal void PlaySFX(AudioClip clip) => _sFXSource.PlayOneShot(clip);

    internal void PlayBackground()
    {
        _musicSource.clip = _background;
        _musicSource.Play();
    }

    internal void StopAllAudio()
    {
        if (_masterVolume != null)
            _masterVolume.Stop();
        else
            Logging.LogWarning("Master volume AudioSource is null");
    }
}