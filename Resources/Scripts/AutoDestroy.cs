using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float timeToDestroy;

    private void Start()
    {
        StartCoroutine(destroy());
    }

    IEnumerator destroy()
    {
        yield return new WaitForSeconds(timeToDestroy);
        Destroy(this.gameObject);
    }
}
