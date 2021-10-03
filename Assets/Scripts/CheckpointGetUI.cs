using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointGetUI : MonoBehaviour
{
    public float fadeInSpeed = 0.2f;
    public float fadeOutSpeed = 1.0f;
    public float waitTime = 0.5f;

    private Text _text;
    private AudioSource _audioSource;
    // private RectTransform _textTransform;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _text = GetComponentInChildren<Text>();
        var c = _text.color;
        _text.color = new Color(c.r, c.g, c.b, 0.0f);
    }

    public void Show()
    {
        StartCoroutine(ShowCoroutine());
    }

    private IEnumerator ShowCoroutine()
    {
        _audioSource.Play();
        // fade in
        while (_text.color.a < 1.0f)
        {
            var c = _text.color;
            _text.color = new Color(c.r, c.g, c.b, c.a + fadeInSpeed * Time.deltaTime);
            yield return 0;
        }

        yield return new WaitForSeconds(waitTime);

        // fade out
        while (_text.color.a > 0)
        {
            var c = _text.color;
            _text.color = new Color(c.r, c.g, c.b, c.a - fadeOutSpeed * Time.deltaTime);
            yield return 0;
        }
    }
}
