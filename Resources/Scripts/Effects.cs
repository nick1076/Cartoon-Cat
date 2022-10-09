using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects : MonoBehaviour
{
    public GameObject scareOrigin;
    public GameObject basicScare;
    public GameObject secondScare;
    public GameObject tershiaryScare;

    public void Scare(int type)
    {
        if (type == 0)
        {
            GameObject obj = Instantiate(basicScare, scareOrigin.transform);
            obj.transform.localPosition = new Vector3(0, 0, 0);
        }
        else if (type == 1)
        {
            GameObject obj = Instantiate(secondScare, scareOrigin.transform);
            obj.transform.localPosition = new Vector3(0, 0, 0);
        }
        else if (type == 2)
        {
            GameObject obj = Instantiate(tershiaryScare, scareOrigin.transform);
            obj.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
}
