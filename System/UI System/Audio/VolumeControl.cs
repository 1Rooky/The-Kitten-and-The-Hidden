using UnityEngine;
using UnityEngine.UI;

public enum VolumeType
{
    Master,
    Music,
    SFX,
}

public class VolumeControl : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] private VolumeType volumeType;

    private Slider volumeSlider;
    private AudioSystemFMOD _audioSystem;

    private void Awake() => volumeSlider = GetComponent<Slider>();

    private void Start() => _audioSystem = ServiceLocator.Instance.GetService<AudioSystemFMOD>();

    private void Update()
    {
        switch (volumeType)
        {
            case VolumeType.Master:
                volumeSlider.value = _audioSystem.MasterVolume;
                break;

            case VolumeType.Music:
                volumeSlider.value = _audioSystem.MusicVolume;
                break;

            case VolumeType.SFX:
                volumeSlider.value = _audioSystem.SfxVolume;
                break;

            default:
                Logging.Log("Invalid Type !");
                break;
        }
    }

    public void OnSliderValueChanged()
    {
        switch (volumeType)
        {
            case VolumeType.Master:
                _audioSystem.MasterVolume = volumeSlider.value;
                break;

            case VolumeType.Music:
                _audioSystem.MusicVolume = volumeSlider.value;
                break;

            case VolumeType.SFX:
                _audioSystem.SfxVolume = volumeSlider.value;
                break;

            default:
                Logging.Log("Invalid Type !");
                break;
        }
    }
}