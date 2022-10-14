using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebsiteOpen : MonoBehaviour
{
    public string link;
    public void Click()
    {
        Application.OpenURL(link);
    }
}
