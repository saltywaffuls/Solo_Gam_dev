using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialgBox;
    [SerializeField] Text dialogText;
    [SerializeField] int letterPerSecond;

    public event Action OnShowDialog;
    public event Action OnCloseDialog;

    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    Dialog dialog;
    Action onDialogfinished;
    int currentLine = 0;
    bool isTyping;

    public bool IsShowing { get; private set; }

    

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !isTyping)
        {
            ++currentLine;
            if(currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                IsShowing = false;
                dialgBox.SetActive(false);
                onDialogfinished?.Invoke();
                OnCloseDialog?.Invoke();
            }
        }
    }

    public IEnumerator ShowDialogText(string text, bool WaitForInput=true)
    {
        IsShowing = true;

        dialgBox.SetActive(true);
        yield return TypeDialog(text);
        if (WaitForInput)
        {
            new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }

        dialgBox.SetActive(false);
        IsShowing = false;
    }

    // ep 27 21:20 fixed ep 30 2:31
    public IEnumerator ShowDialog(Dialog dialog, Action onfinished = null)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();

        IsShowing = true;
        this.dialog = dialog;
        onDialogfinished = onfinished;

        dialgBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }

   

    //animates text
    public IEnumerator TypeDialog(string line)
    {

        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
        isTyping = false;
    }
}
