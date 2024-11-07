using UnityEngine;

public class PresentationSkyboxAnimation : MonoBehaviour
{
    public float rotationSpeed = 1.0f; // Speed of rotation

    // Update is called once per frame
    private void Update()
    {
        // Calculate the rotation around the Y axis
        float rotation = Mathf.Repeat(Time.time * rotationSpeed, 360);
        // Apply the rotation to the skybox
        RenderSettings.skybox.SetFloat("_Rotation", rotation);
    }
}