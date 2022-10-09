using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="newAmbienceSet", menuName="New Ambience Set")]
public class AmbienceSetData : ScriptableObject
{
    public AudioClip ambientTrack;
    public float volumeToPlayAt = 0.153f;
}
