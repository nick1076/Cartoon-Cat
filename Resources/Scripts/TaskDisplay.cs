using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskDisplay : MonoBehaviour
{
    public TypeText taskInfoScript;
    public void Initialize(string taskInfo)
    {
        taskInfoScript.fullText = taskInfo;
    }
}
