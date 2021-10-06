using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FadeInOut : MonoBehaviour
{
    public float fadeSpeed = 0.5f;
    private Image _image;

    public bool IsFading { get; private set; } = false;

    private void Start()
    {
        _image = GetComponent<Image>();
    }

    public void InstantFadeIn()
    {
        if (!_image)
            _image = GetComponent<Image>();
        var c = _image.color;
        _image.color = new Color(c.r, c.g, c.b, 1.0f);
    }
    
    public void InstantFadeOut()
    {
        if (!_image)
            _image = GetComponent<Image>();
        var c = _image.color;
        _image.color = new Color(c.r, c.g, c.b, 0.0f);
    }

    public Coroutine FadeOut()
    {
        return IsFading ? null : StartCoroutine(FadeOutCoroutine());
    }

    public Coroutine FadeIn()
    {
        return IsFading ? null : StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        IsFading = true;
        while (_image.color.a > 0)
        {
            var c = _image.color;
            _image.color = new Color(c.r, c.g, c.b, c.a - fadeSpeed * Time.deltaTime);
            yield return 0;
        }
        IsFading = false;
    }

    private IEnumerator FadeInCoroutine()
    {
        IsFading = true;
        while (_image.color.a < 1.0f)
        {
            var c = _image.color;
            _image.color = new Color(c.r, c.g, c.b, c.a + fadeSpeed * Time.deltaTime);
            yield return 0;
        }
        IsFading = false;
    }
}
