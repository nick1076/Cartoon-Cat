using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypeText : MonoBehaviour
{

    public float delay = 0.0f;
    public float delayPerLetter = 0.1f;
    public string fullText;
    private string currentText = "";

    public List<GameObject> typingSounds = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        this.GetComponent<TextMeshProUGUI>().text = "";
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(ShowText());
    }

    IEnumerator ShowText()
    {
        for (int i = 0; i < fullText.Length + 1; i++)
        {
            if (typingSounds.Count >= 1)
            {
                int sel = UnityEngine.Random.Range(0, typingSounds.Count);

                Instantiate(typingSounds[sel]);
            }

            currentText = fullText.Substring(0, i);
            this.GetComponent<TextMeshProUGUI>().text = currentText;
            yield return new WaitForSeconds(delayPerLetter);
        }
    }
}
