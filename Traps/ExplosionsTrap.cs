using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionsTrap : Interactable
{
    //[SerializeField] private GameObject Explosion;
    protected override void Interact(ObjectType _objectType)
    {
        base.Interact(_objectType);
        if (_objectType.gameObject.TryGetComponent<HealthSystem>(out HealthSystem Damaged))
        {
            Logging.Log(Damaged.name);
            //Explosion.SetActive(true);
            Damaged.TakeDamage(10f);
        }
    }
}
