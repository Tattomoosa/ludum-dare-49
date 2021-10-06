using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private Text textbox;
    [SerializeField] private string sceneToLoadAtEnd;

    [SerializeField] private AudioSource _audioSource;
    
    private List<string> _textBoxes;

    private bool _endgameDialogue = false;

    public void Start()
    {
        var testDialogue = new List<string>
        {
            "",
            
            "Power's out again. Maybe there's something going on at the nuclear plant...",
            
            "I should really walk over there -- with the <color=cyan>WASD/arrow keys</color> -- and take a look... with <color=cyan>the mouse.</color>",
            "...I think there's some other things I can do that I forgot about. Maybe <color=cyan>reading these signs</color> with the <color=cyan>right mouse button/R</color> will jog my memory.",
            "<color=yellow><b>NOTE: Mouse sensitivity can be adjusted with Tab/Escape once you leave this dialogue.</b></color>",
        };
        ShowDialogue(testDialogue);
    }
    
    public void ShowDialogue(List<string> textBoxes)
    {
        _textBoxes = textBoxes;
        gameObject.SetActive(true);
        GameTime.Pause();
        ShowNextTextBox(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.Return)
            || Input.GetMouseButtonDown(0))
            ShowNextTextBox();
    }

    private void ShowNextTextBox(bool silent = false)
    {
        if (!silent)
            _audioSource.Play();
        if (_textBoxes.Count > 0)
        {
            var text = _textBoxes[0];
            textbox.text = text;
            _textBoxes.Remove(text);
        }
        else
        {
            if (_endgameDialogue)
                SceneManager.LoadScene(sceneToLoadAtEnd);
            else
                EndDialogue();
        }
    }

    public void EndDialogue()
    {
        StartCoroutine(EndDialogueCoroutine());
    }

    public bool IsOpen => gameObject.activeInHierarchy;

    public void WinGameDialogue()
    {
        _endgameDialogue = true;
        var endDialogue = new List<string>
        {
            "That should do it. Now I can get back home and keep watching TV.",
            "What a hassle"
        };
        ShowDialogue(endDialogue);
    }

    private IEnumerator EndDialogueCoroutine()
    {
        yield return null;
        yield return null;
        gameObject.SetActive(false);
        GameTime.Unpause();
    }
}
