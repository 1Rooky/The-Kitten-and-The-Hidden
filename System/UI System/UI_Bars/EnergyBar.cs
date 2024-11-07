using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    [SerializeField] private Image _image;

    [SerializeField] private EnergySystem _energySystem;

    private void Start()
    {
        _energySystem.OnEnergyChanged.AddListener(UpdateEnergyBar);
        _image.fillAmount = _energySystem.EnergyPrecentage;
    }

    private void UpdateEnergyBar()
    {
        if (_energySystem != null)
            _image.fillAmount = _energySystem.EnergyPrecentage;
    }
    private void OnDisable() => _energySystem.OnEnergyChanged.RemoveListener(UpdateEnergyBar);

    private void OnDestroy() => _energySystem.OnEnergyChanged.RemoveListener(UpdateEnergyBar);
}