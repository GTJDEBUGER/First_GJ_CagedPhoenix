using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charactors_Data : MonoBehaviour
{
   
    public Charactors Charactor;
    public int maxHealth
    { 
        get { if (Charactor.maxHealth != null) return Charactor.maxHealth; else return 0; }
        set { Charactor.maxHealth = value; }
    }
    public int currentHealth
    {
        get { if (Charactor.currentHealth != null) return Charactor.currentHealth; else return 0; }
        set { Charactor.currentHealth = value; }
    }
    public int attackValue
    {
        get { if (Charactor.attack != null) return Charactor.attack; else return 0; }
        set { Charactor.attack = value; }
    }
    public Vector2 position
    {
        get { if (Charactor.position != null) return Charactor.position; else return Vector2.zero; }
        set { Charactor.position = value; }
    }
}
