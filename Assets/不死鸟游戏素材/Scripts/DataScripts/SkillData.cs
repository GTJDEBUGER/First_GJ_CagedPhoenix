using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Data", menuName = "Charactor/SkillStore")]
public class SkillData : ScriptableObject
{
    public bool fire;
    public bool wind;
    public bool doubleJump;
    public bool flash;
}
