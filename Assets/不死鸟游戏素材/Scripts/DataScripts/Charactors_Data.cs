using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charactors_Data : MonoBehaviour
{
   
    public Charactors Charactor;
    public int maxHealth
    { 
        get {  return Charactor.maxHealth;}
        set { Charactor.maxHealth = value; }
    }
    public int currentHealth
    {
        get {return Charactor.currentHealth; }
        set { Charactor.currentHealth = value; }
    }
    public int attackValue
    {
        get { return Charactor.attack; }
        set { Charactor.attack = value; }
    }
    public Vector2 position
    {
        get { return Charactor.position;  }
        set { Charactor.position = value; }
    }
}
