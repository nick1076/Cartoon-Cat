using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject black;
    public GameObject madeFade;
    public AudioSource cartoonCatAudio;

    public GameObject fadeIn;
    public GameObject fadeOut;

    public GameObject aboutMenu;

    public List<GameObject> toggleOffForAbout = new List<GameObject>();

    public GameObject completionMedal;

    public Transform canvas;

    public AudioSource cartoonCatSource;

    private void Start()
    {
        if (PlayerPrefs.GetInt("complete") == 1)
        {
            completionMedal.SetActive(true);
        }
        else
        {
            completionMedal.SetActive(false);
        }

        StartCoroutine(StartMenu());
    }

    public void StartGame()
    {
        StopAllCoroutines();
        Instantiate(fadeOut, canvas.transform);
        StartCoroutine(fadeOutAudio());
        StartCoroutine(waitExecute(0));
    }

    IEnumerator StartMenu()
    {
        black.SetActive(true);
        madeFade.SetActive(false);
        cartoonCatAudio.gameObject.SetActive(false);
        yield return new WaitForSeconds(2.0f);
        cartoonCatAudio.volume = 0;
        cartoonCatAudio.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(fadeInAudio());
        madeFade.SetActive(true);
        black.SetActive(false);
    }

    public void About(bool on)
    {
        if (on)
        {
            aboutMenu.SetActive(true);
            foreach (GameObject obj in toggleOffForAbout)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            aboutMenu.SetActive(false);
            foreach (GameObject obj in toggleOffForAbout)
            {
                obj.SetActive(true);
            }
        }
    }

    public void Quit()
    {
        StopAllCoroutines();
        Instantiate(fadeOut, canvas.transform);
        StartCoroutine(fadeOutAudio());
        StartCoroutine(waitExecute(1));
    }

    IEnumerator fadeOutAudio()
    {
        yield return new WaitForSeconds(0.01f);
        cartoonCatSource.volume -= 0.01f;
        StartCoroutine(fadeOutAudio());
    }    
    
    IEnumerator fadeInAudio()
    {
        yield return new WaitForSeconds(0.01f);
        cartoonCatAudio.volume += 0.01f;
        StartCoroutine(fadeInAudio());
    }

    IEnumerator waitExecute(int id)
    {
        yield return new WaitForSeconds(0.5f);
        if (id == 0)
        {
            yield return new WaitForSeconds(2.0f);
            SceneManager.LoadScene(1);
        }        
        else if (id == 1)
        {
            yield return new WaitForSeconds(2.0f);
            Application.Quit();
        }
    }
}
