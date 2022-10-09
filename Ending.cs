using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending : MonoBehaviour
{
    public CartoonCat cat;
    public GameObject whiteFade;
    public FPSControllerLPFP.FpsControllerLPFP player;

    public void CompleteDemo()
    {
        PlayerPrefs.SetInt("complete", 1);
        cat.level = 2;
        whiteFade.SetActive(true);
        player = GameObject.Find("Player").GetComponent<FPSControllerLPFP.FpsControllerLPFP>();
        player.targetVolume = 0.5f;
        player.target = null;
        StartCoroutine(Complete());
    }

    IEnumerator Complete()
    {
        yield return new WaitForSeconds(2.5f);
        player.canMove = false;
        cat.transform.parent.gameObject.SetActive(false);
        yield return new WaitForSeconds(17.5f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(0);

    }
}
