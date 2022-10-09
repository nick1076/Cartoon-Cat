using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialougeManager : MonoBehaviour
{
    public List<string> dialougeQues = new List<string>();
    public GameObject defaultDialouge;
    public Transform dialougeOrigin;

    private string previousMessage;

    private bool showingDialouge = false;

    private void Start()
    {
        StartCoroutine(ProcessDialouges());
    }

    public void QueDialouge(string info)
    {
        if (previousMessage == info)
        {
            return;
        }

        dialougeQues.Add(info);

        previousMessage = info;
    }

    IEnumerator ProcessDialouges()
    {
        yield return new WaitForSeconds(0.1f);

        if (!showingDialouge)
        {
            if (dialougeQues.Count >= 1)
            {
                showingDialouge = true;
                GameObject text = Instantiate(defaultDialouge, dialougeOrigin.transform);
                text.GetComponent<TextMeshProUGUI>().text = dialougeQues[0];
                dialougeQues.Remove(dialougeQues[0]);

                yield return new WaitForSeconds(5.0f);
                showingDialouge = false;
            }
        }

        StartCoroutine(ProcessDialouges());
    }
}
