using Unity.Cinemachine;
using UnityEngine;

public class CM_CamBlending : MonoBehaviour
{
    [Header("Frame Size")]
    [Space(3)]
    [SerializeField] private float _maxDistance = 400.0f;
    [SerializeField] private float _minDistance = 10.0f;

    [Header("Virtual Cameras")]
    [Space(3)]
    [SerializeField] private GameObject _camBlend1;
    [SerializeField] private GameObject _camBlend2;
    [Space(3)]
    [SerializeField] private CinemachineTargetGroup _targetGroup;

    private bool isTopDown = false;

    private void LateUpdate()
    {
        BlendCamera();
    }
    void BlendCamera()
    {

        float averageDistance = CalculateAverageDistance(_targetGroup);
        if (averageDistance >= _minDistance && averageDistance < _maxDistance && !isTopDown)
        {
            _camBlend1.SetActive(false);
            _camBlend2.SetActive(true);
            isTopDown = true;
        }
        if (averageDistance < _minDistance && isTopDown)
        {
            _camBlend1.SetActive(true);
            _camBlend2.SetActive(false);
            isTopDown = false;
        }
    }
    float CalculateAverageDistance(CinemachineTargetGroup targetGroup)
    {
        float totalDistance = 0f;
        int count = 0;
        for (int i = 0; i < targetGroup.Targets.Count; i++)
        {
            for (int j = i + 1; j < targetGroup.Targets.Count; j++)
            {
                totalDistance += Vector3.Distance(targetGroup.Targets[i].Object.position, targetGroup.Targets[j].Object.position);
                count++;
            }
        }
        return count > 0 ? totalDistance / count : 0f;
    }
}
