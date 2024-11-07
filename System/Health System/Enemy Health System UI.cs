using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthSystemUI : MonoBehaviour
{
    [SerializeField] private HealthSystem enemyHealthSystem; // Reference to the enemy's HealthSystem
    [SerializeField] private Slider healthSlider; // Reference to the UI Slider
    private Camera mainCamera; // Reference to the main camera

    private void Start()
    {
        // If enemyHealthSystem is not manually assigned, try to get it from the parent GameObject
        if (enemyHealthSystem == null)
            enemyHealthSystem = GetComponentInParent<HealthSystem>();

        // If healthSlider is not manually assigned, try to get it from the child GameObjects
        if (healthSlider == null)
            healthSlider = GetComponentInChildren<Slider>();

        if (enemyHealthSystem != null)
        {
            // Initialize the slider's max value to 1 as we will be using a percentage (0 to 1)
            healthSlider.maxValue = 1;

            // Subscribe to the OnHealthChanged event to update the UI whenever the enemy's health changes
            enemyHealthSystem.OnHealthChanged.AddListener(UpdateHealthUI);

            // Initial UI update
            UpdateHealthUI();
        }

        mainCamera = ServiceLocator.Instance.GetService<CameraInstance>().Camera;
    }

    private void Update()
    {
        // Make the health slider always face the camera
        if (mainCamera != null)
            healthSlider.transform.LookAt(healthSlider.transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }

    private void UpdateHealthUI()
    {
        if (enemyHealthSystem != null && healthSlider != null)
            healthSlider.value = enemyHealthSystem.HealthPrecentage; // Update the slider's value to the current health percentage of the enemy
    }
}