using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] private Text textbox;
    // [SerializeField] private FadeInOut topBar;
    // [SerializeField] private FadeInOut bottomBar;
    
    private List<string> _textBoxes;

    public void Start()
    {
        var testDialogue = new List<string>();
        testDialogue.Add("You: I knew that damn nuclear plant was a bad idea.");
        testDialogue.Add("It's always breaking down. So unstable.");
        testDialogue.Add("I should really walk over there -- with the WASD/arrow keys -- and take a look... with the mouse. Maybe I can fix it.");
        testDialogue.Add("Also it would be good to read signs by looking at them and clicking the right mouse button.");
        testDialogue.Add("Of course I'm a regular here, and I know what they say. But it's just good reading practice, you know?");
        ShowDialogue(testDialogue);
    }
    
    public void ShowDialogue(List<string> textBoxes)
    {
        _textBoxes = textBoxes;
        gameObject.SetActive(true);
        GameTime.Pause();
        ShowNextTextBox();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.Return)
            || Input.GetMouseButtonDown(0))
            ShowNextTextBox();
    }

    private void ShowNextTextBox()
    {
        if (_textBoxes.Count > 0)
        {
            var text = _textBoxes[0];
            textbox.text = text;
            _textBoxes.Remove(text);
        }
        else
            EndDialogue();
    }

    public void EndDialogue()
    {
        StartCoroutine(EndDialogueCoroutine());
    }

    private IEnumerator EndDialogueCoroutine()
    {
        yield return null;
        yield return null;
        gameObject.SetActive(false);
        GameTime.Unpause();
    }
}
