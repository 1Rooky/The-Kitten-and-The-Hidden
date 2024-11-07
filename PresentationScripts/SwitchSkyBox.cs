using DG.Tweening;
using System.Collections;
using UnityEngine;

public class SwitchSkyBox : MonoBehaviour
{
    [SerializeField] private Material _DaySkyMat;
    [SerializeField] private Material _nightSkyMat;
    [SerializeField] private float _transitionDuration = 2.0f; // Duration of the transition

    public void SwitchToNight()
    {
        StartCoroutine(SwitchSkyBoxRoutine(_nightSkyMat));
    }

    public void SwitchToDay()
    {
        StartCoroutine(SwitchSkyBoxRoutine(_DaySkyMat));
    }

    private IEnumerator SwitchSkyBoxRoutine(Material targetSkybox)
    {
        // Store the current skybox material
        Material currentSkybox = RenderSettings.skybox;
        // Create a temporary material to use for the fade transition
        Material tempSkybox = new Material(currentSkybox);
        RenderSettings.skybox = tempSkybox;

        // Fade out the current skybox
        yield return tempSkybox.DOColor(Color.black, "_Tint", _transitionDuration).WaitForCompletion();
        // Switch to the target skybox
        tempSkybox.CopyPropertiesFromMaterial(targetSkybox);
        // Fade in the target skybox
        tempSkybox.color = Color.black; // Ensure it's faded out
        tempSkybox.DOColor(Color.white, "_Tint", _transitionDuration);

        // Clean up: Optionally, after the transition, set RenderSettings.skybox to the targetSkybox directly
        // and destroy the temporary material if you won't need it anymore.
    }
}