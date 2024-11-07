using UnityEngine;

public class FadingEffect : MonoBehaviour
{
    [SerializeField] private float _fadingSpeed = 0.5f;
    [SerializeField] private Color _fadeColor = Color.black;
    [SerializeField] private AnimationCurve _fadeCurve;

    private float _alpha = 0;
    private Texture2D _texture;
    private int _direction = 0;
    private float _time;

    private void Awake() => ServiceLocator.Instance.RegisterService(this, false);

    private void Start() => _texture = new Texture2D(1, 1);

    public void FadeIn()
    {
        _alpha = 1f;
        _time = 1;
        _direction = 1;
        _texture.SetPixel(0, 0, new Color(_fadeColor.r, _fadeColor.g, _fadeColor.b, _alpha));
        _texture.Apply();
    }

    public void FadeOut()
    {
        _alpha = 0f;
        _time = 1;
        _direction = -1;
        _texture.SetPixel(0, 0, new Color(_fadeColor.r, _fadeColor.g, _fadeColor.b, _alpha));
        _texture.Apply();
    }

    private void OnGUI()
    {
        if (_alpha > 0) GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _texture);
        if (_direction != 0)
        {
            _time += Time.deltaTime * _direction * _fadingSpeed;
            _alpha = _fadeCurve.Evaluate(_time);
            _texture.SetPixel(0, 0, new Color(_fadeColor.r, _fadeColor.g, _fadeColor.b, _alpha));
            _texture.Apply();
            if (_alpha <= 0f || _alpha >= 1f) _direction = 0;
        }
    }
}