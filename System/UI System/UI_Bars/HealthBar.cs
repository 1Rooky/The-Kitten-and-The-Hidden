using UnityEngine;
using UnityEngine.UI;

// This class represents a health bar in the game.
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _image; // The image component of the health bar.

    [SerializeField] private HealthSystem _healthSystem; // The health system that the health bar represents.

    // At the start of the game, set the fill amount of the health bar image to the health percentage of the health system.
    private void Start()
    {
        _healthSystem.OnHealthChanged.AddListener(UpdateHealthBar);
        _image.fillAmount = _healthSystem.HealthPrecentage;
    }

    // Method to update the health bar.
    private void UpdateHealthBar()
    {
        // If the health system is not null, set the fill amount of the health bar image to the health percentage of the health system.
        if (_healthSystem != null)
            _image.fillAmount = _healthSystem.HealthPrecentage;
    }

    // When the health bar is disabled, remove the UpdateHealthBar method as a listener to the OnHealthChanged event.
    private void OnDisable() => _healthSystem.OnHealthChanged.RemoveListener(UpdateHealthBar);
}