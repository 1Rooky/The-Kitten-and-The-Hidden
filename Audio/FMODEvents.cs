using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    [Header("Player Steps")]
    [field: SerializeField] public EventReference PlayerSteps { get; private set; }

    [Header("Cat Shoot")]
    [field: SerializeField] public EventReference CatShoot { get; private set; }

    [Header("Turret Shoot")]
    [field: SerializeField] public EventReference TurretShoot { get; private set; }

    [Header("MainMenu")]
    [field: SerializeField] public EventReference MainMenu { get; private set; }
    
    [Header("Puzzle")]
    [field: SerializeField] public EventReference Puzzle { get; private set; }

    [Header("Arena")]
    [field: SerializeField] public EventReference Arena { get; private set; }

    private void Awake() => ServiceLocator.Instance.RegisterService(this, true);
}