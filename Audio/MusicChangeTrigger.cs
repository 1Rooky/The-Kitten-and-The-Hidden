using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicArea
{
    Puzzle = 0,
    Arena = 1,
}
public class MusicChangeTrigger : MonoBehaviour
{
    [Header("Area")]
    [SerializeField] private MusicArea area;

    private AudioSystemFMOD _audioSystem;

    private void Start()
    {
        _audioSystem = ServiceLocator.Instance.GetService<AudioSystemFMOD>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _audioSystem.SetMusicArea(area);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

        }
    }
}
