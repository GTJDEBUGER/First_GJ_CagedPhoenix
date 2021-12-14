using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Data", menuName = "Charactor/CharactorInfomation")]
public class Charactors : ScriptableObject
{  
    public int maxHealth;
    public int currentHealth;
    public int attack;
    public Vector2 position;
}
