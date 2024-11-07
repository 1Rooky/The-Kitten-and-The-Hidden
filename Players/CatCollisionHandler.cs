using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatCollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has a Rigidbody component
        Rigidbody collidedRigidbody = collision.collider.GetComponent<Rigidbody>();

        // If the collided object has a Rigidbody, prevent force from being applied
        if (collidedRigidbody != null)
        {
            // Here you can implement logic to prevent force application
            // For example, setting the velocity of the Rigidbody to zero
            collidedRigidbody.velocity = Vector3.zero;

            // Optionally, you can also disable the Rigidbody to completely prevent any physics interactions
            collidedRigidbody.isKinematic = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Check if the collided object has a Rigidbody component
        Rigidbody collidedRigidbody = collision.collider.GetComponent<Rigidbody>();

        // If the collided object has a Rigidbody, prevent force from being applied
        if (collidedRigidbody != null)
        {
            // Optionally, you can also disable the Rigidbody to completely prevent any physics interactions
            collidedRigidbody.isKinematic = false;
        }
    }
}
