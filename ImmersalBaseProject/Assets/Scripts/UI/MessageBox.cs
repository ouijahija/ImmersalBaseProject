using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    public static MessageBox Instance { get; private set; }

    public Image panel;
    public TextMeshProUGUI message;

    public float letterAppearTime = 0.1f;

    IEnumerator textAppearSequence;

    private void Awake()
    {
        Instance = this;

        HideMessage();
    }

    public static void ShowMessage(string messageText)
    {
        Instance.StartTexCoroutine(messageText);
    }

    public void HideMessage()
    {
        if (textAppearSequence != null)
            StopCoroutine(textAppearSequence);

        panel.gameObject.SetActive(false);
    }

    public void StartTexCoroutine(string messageText)
    {
        if (textAppearSequence != null)
            StopCoroutine(textAppearSequence);

        panel.gameObject.SetActive(true);
        textAppearSequence = StarttextAppearSequence(messageText);
        StartCoroutine(textAppearSequence);
    }

    private IEnumerator StarttextAppearSequence(string messageText)
    {
        message.text = "";
        yield return new WaitForSeconds(letterAppearTime);

        while (message.text != messageText)
        {
            message.text = messageText.Substring(0, message.text.Length + 1);
            yield return new WaitForSeconds(letterAppearTime);
        }
    }
}
