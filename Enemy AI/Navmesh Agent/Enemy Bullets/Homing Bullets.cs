using System.Collections;
using UnityEngine;

public class HomingBullet : Bullet
{
    [SerializeField] private float _yOffset;
    [SerializeField] private AnimationCurve _positionCurve;
    [SerializeField] private AnimationCurve _noiseCurve;
    [SerializeField] private Vector2 _minNoise = new(-3f, -0.25f);
    [SerializeField] private Vector2 _maxNoise = new(3f, 1f);

    private Coroutine _homingCoroutine;

    internal override void Spawn(Vector3 _forward, int _damage, Transform _target)
    {
        Damage = _damage;
        this._target = _target;

        if (_homingCoroutine != null)
            StopCoroutine(_homingCoroutine);

        _homingCoroutine = StartCoroutine(FindTarget());
    }

    private IEnumerator FindTarget()
    {
        Vector3 _startPosition = transform.position;
        Vector2 _noise = new(Random.Range(_minNoise.x, _maxNoise.x), Random.Range(_minNoise.y, _maxNoise.y));
        Vector3 _targetPosition = _target.position;
        Vector3 _bulletDirectionVector = new Vector3(_targetPosition.x, _targetPosition.y + _yOffset, _targetPosition.z) - _startPosition;
        Vector3 _horizontalNoiseVector = Vector3.Cross(_bulletDirectionVector, Vector3.up).normalized;

        float _time = 0f;

        while (_time < 1f)
        {
            float _noisePosition = _noiseCurve.Evaluate(_time);
            transform.position = Vector3.Lerp(_startPosition, _targetPosition + Vector3.up * _yOffset, _positionCurve.Evaluate(_time)) + _noise.x * _noisePosition * _horizontalNoiseVector + _noise.y * _noisePosition * Vector3.up;
            transform.LookAt(_targetPosition + Vector3.up * _yOffset);

            _time += Time.fixedDeltaTime * Speed;

            yield return null;
        }
    }
}