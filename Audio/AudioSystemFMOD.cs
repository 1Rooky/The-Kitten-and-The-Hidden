using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioSystemFMOD : MonoBehaviour
{
    // Reference to the FMOD system
    private FMODEvents fmodSystem;

    // Instance of the music event
    private EventInstance musicEventInstance;

    #region Volume Control

    //Volume Control
    [Header("Volume")]
    // Music volume, can be set in the Unity editor
    [Range(0, 1)]
    [SerializeField] private float masterVolume = 1;

    [Range(0, 1)]
    [SerializeField] private float musicVolume = 1;
    
    // Sound effects volume, can be set in the Unity editor
    [Range(0, 1)]
    [SerializeField] private float sfxVolume = 1;

    // FMOD bus for the music
    private Bus masterBus;
    private Bus musicBus;

    // FMOD bus for the sound effects
    private Bus sfxBus;

    // Property for the music volume
    public float MasterVolume { get => masterVolume; set => masterVolume = value; }
    public float MusicVolume { get => musicVolume; set => musicVolume = value; }

    // Property for the sound effects volume
    public float SfxVolume { get => sfxVolume; set => sfxVolume = value; }

    // Property for the FMOD system
    public FMODEvents FmodSystem => fmodSystem;

    public Bus MasterBus { get => masterBus; set => masterBus = value; }


    #endregion Volume Control

    // Called when the object is first initialized
    private void Awake()
    {
        // Register this service in the ServiceLocator
        ServiceLocator.Instance.RegisterService(this, true);

        // Get the FMOD buses
        MasterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    // Called before the first frame update
    private void Start()
    {
        // Get the FMODEvents service from the ServiceLocator
        fmodSystem = ServiceLocator.Instance.GetService<FMODEvents>();
        // Initialize the music
        InitializeMusic(FmodSystem.MainMenu);
    }

    // Called every frame
    private void Update()
    {
        // Set the volume of the music and sound effects
        MasterBus.setVolume(MasterVolume);
        musicBus.setVolume(MusicVolume);
        sfxBus.setVolume(SfxVolume);
    }

    // Create an event instance for the player's foot steps
    internal EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance PlayereventInstance = RuntimeManager.CreateInstance(eventReference);
        PlayereventInstance.set3DAttributes(transform.position.To3DAttributes()); // Set 3D attributes
        return PlayereventInstance;
    }

    // Play the player's shooting sound
    public void PlayerShooting(EventReference catShoot, Vector3 pos)
    {
        RuntimeManager.PlayOneShot(catShoot, pos);
        Logging.Log("Sound");
    }

    // Initialize the environment music
    internal void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = RuntimeManager.CreateInstance(musicEventReference);
        musicEventInstance.start();
    }

    // Set the music area parameter
    internal void SetMusicArea(MusicArea area) => musicEventInstance.setParameterByName("area", (float)area);
}