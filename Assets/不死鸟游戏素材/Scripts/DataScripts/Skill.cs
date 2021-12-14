using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public SkillData Skills;
    public bool fire
    {
        get {return Skills.fire;  ; }
        set { Skills.fire = value; }
    }
    public bool wind
    {
        get { return Skills.wind;  }
        set { Skills.wind = value; }
    }
    public bool doubleJump
    {
        get {  return Skills.doubleJump;  }
        set { Skills.doubleJump = value; }
    }
    public bool flash
    {
        get { return Skills.flash;  }
        set { Skills.flash = value; }
    }
}
