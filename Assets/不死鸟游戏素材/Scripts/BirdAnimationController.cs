using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdAnimationController : MonoBehaviour
{
    public HeroMainMove heroScript;
    private void Update()
    {
        //hangeAttack1();
    }
    public void ChangeAttack1()
    {
       if(!heroScript.attack2)
            heroScript.attack1 = false;
        Debug.Log("我已经修改了！");
    }
    public void ChangeAttack2()
    {
        if (!heroScript.attack3)
        {
            heroScript.attack1 = false;
            heroScript.attack2 = false;
        }
    }
        public void ChangeAttack3()
    {
        heroScript.attack3 = false;
    }

}
