using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="newWalkingMaterial", menuName="New Walking Material")]
public class WalkingMaterialData : ScriptableObject
{
    public List<AudioClip> footstepSounds = new List<AudioClip>();
    public List<float> footstepSoundVolume = new List<float>();
}
