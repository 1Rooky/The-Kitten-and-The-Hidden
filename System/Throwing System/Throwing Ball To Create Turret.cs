using UnityEngine;

public class ThrowingBallToCreateTurret : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private GameObject _turretPrefab;
    private ObjectPool objectPool;
    private Rigidbody rb;

    private void Start()
    {
        objectPool = ServiceLocator.Instance.GetService<ObjectPool>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _layerMask) != 0)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            InstantiateTurret();

            gameObject.SetActive(false);
        }
    }

    private void InstantiateTurret()
    {
        GameObject Turret = objectPool.GetPooledObject(_turretPrefab);

        if (Turret != null)
        {
            Turret.SetActive(true);
            Turret.transform.SetPositionAndRotation(gameObject.transform.position, gameObject.transform.rotation);
        }
    }
}