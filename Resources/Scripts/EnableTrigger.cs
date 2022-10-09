using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnableTrigger : MonoBehaviour
{

    public enum entranceTypeDate
    {
        OnEnter,
        OnExit
    }

    public entranceTypeDate entranceType;

    public bool enableType;

    public UnityEvent eventToInvoke;

    public List<GameObject> objectsToAlter;

    public bool triggerOnStart;
    public bool withDelay;
    public float delayTime = 0.0f;
    public string task;
    public bool destroyOnUse;

    private void Start()
    {
        if (triggerOnStart)
        {
            if (withDelay)
            {
                if (delayTime == -1)
                {
                    TriggerWithDelay(-1);
                }
                else
                {
                    TriggerWithDelay(delayTime);
                }
            }
            else
            {
                Trigger();
            }
        }
    }

    public void TriggerWithDelay(float time)
    {
        if (time == -1)
        {
            StartCoroutine(waitTrigger(-1));
        }
        else
        {
            StartCoroutine(waitTrigger(time));
        }
    }

    IEnumerator waitTrigger(float time)
    {
        if (time != -1)
        {
            yield return new WaitForSeconds(time);
            Trigger();
        }
        else
        {
            yield return new WaitForSeconds(0.1f);

            if (GameObject.Find("Task Manager").GetComponent<TaskManager>().executedTask != task)
            {
                StartCoroutine(waitTrigger(-1));
            }
            else
            {
                Trigger();
            }
        }
    }

    public void Trigger()
    {
        if (entranceType == entranceTypeDate.OnEnter)
        {
            eventToInvoke.Invoke();
            foreach (GameObject obj in objectsToAlter)
            {
                obj.SetActive(enableType);
            }

            if (destroyOnUse)
            {
                Destroy(this.gameObject);
            }
        }

        if (entranceType == entranceTypeDate.OnExit)
        {
            eventToInvoke.Invoke();
            foreach (GameObject obj in objectsToAlter)
            {
                obj.SetActive(enableType);
            }

            if (destroyOnUse)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (entranceType == entranceTypeDate.OnEnter)
        {
            if (other.tag == "Player")
            {
                eventToInvoke.Invoke();
                foreach (GameObject obj in objectsToAlter)
                {
                    obj.SetActive(enableType);
                }

                if (destroyOnUse)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (entranceType == entranceTypeDate.OnExit)
        {
            if (other.tag == "Player")
            {
                eventToInvoke.Invoke();
                foreach (GameObject obj in objectsToAlter)
                {
                    obj.SetActive(enableType);
                }

                if (destroyOnUse)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
