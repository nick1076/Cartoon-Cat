using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{

    public List<GameObject> suroundingPoints = new List<GameObject>();

    public float distanceFromPlayer = 0.0f;

    void Update()
    {
        if (GameObject.FindWithTag("Cartoon Cat") != null)
        {
            distanceFromPlayer = Vector3.Distance(this.transform.position, GameObject.FindWithTag("Cartoon Cat").transform.position);
        }
    }

    public GameObject CalculateNextPoint(GameObject previousPoint)
    {
        if (suroundingPoints.Count == 2)
        {
            if (suroundingPoints[0] == previousPoint)
            {
                return suroundingPoints[1];
            }
            else
            {
                return suroundingPoints[0];
            }
        }
        else
        {
            List<GameObject> newPoints = new List<GameObject>();
            newPoints = suroundingPoints;

            GameObject oldPoint = new GameObject();

            foreach (GameObject obj in newPoints)
            {
                if (obj == previousPoint)
                {
                    oldPoint = obj;
                    break;
                }
            }

            newPoints.Remove(oldPoint);

            GameObject finalPoint = newPoints[Random.Range(0, newPoints.Count)];

            return finalPoint;
        }
    }
}
