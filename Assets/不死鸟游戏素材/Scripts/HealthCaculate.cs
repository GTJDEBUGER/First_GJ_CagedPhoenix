using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCaculate : MonoBehaviour
{
    [Header("������")]
    public static HealthCaculate instance;
    private void Start()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }
    //a��b����˺�
    public static void Attack(Charactors_Data a,Charactors_Data b)
    {        
        a.currentHealth -= b.attackValue;
        if (a.currentHealth < 0)
            a.currentHealth = 0;
    }
    
}
