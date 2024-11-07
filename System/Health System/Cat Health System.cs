public class CatHealthSystem : HealthSystem
{
    private void OnEnable()
    {
        HealthCrystal._onHealthCrystalCollected.AddListener(Heal);
        _onDeath.AddListener(SpawnToLastCheckPoint);
    }
    private void OnDisable()
    {
        HealthCrystal._onHealthCrystalCollected.RemoveListener(Heal);
        _onDeath.RemoveListener(SpawnToLastCheckPoint);
    }
    private void SpawnToLastCheckPoint()
    {
        ServiceLocator.Instance.GetService<SpawnSystem>().SpawnAtLastCheckPoint();
        _health = 0.5f * _healthMax;
    }
}