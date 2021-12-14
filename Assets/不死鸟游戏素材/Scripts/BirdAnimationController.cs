using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdAnimationController : MonoBehaviour
{

    public static float a;
    public HeroMainMove heroScript;
    private void OnEnable()
    {
        heroScript =GameObject.Find("hero").GetComponent<HeroMainMove>(); 
    }
    public void ChangeAttack1()
    {
        if (heroScript.isEnterAttack2)
        {

            heroScript.attack2 = true;
        }
        else
        {
            heroScript.isAttackingBack = true;
            heroScript.attack1 = false;
        }
        heroScript.isEnterAttack2 = false;
    }
    public void CanYouJudgy()
    {
        heroScript.canYouJudgyAttack2=!heroScript.canYouJudgyAttack2;
        heroScript.canYouJudgyAttack3=!heroScript.canYouJudgyAttack3;
    }
    public void ChangeAttack2()
    {
        if (heroScript.isEnterAttack3)
        {
            heroScript.attack3 = true;
        }
        else
        {
            heroScript.attack1 = false;
            heroScript.attack2 = false;
        }
        heroScript.isEnterAttack3 = false;
        heroScript.isEnterWrong = false;

    }
    public void ChangeAttack3()
    {
        heroScript.attack3 = false;
        heroScript.attack2 = false;
        heroScript.attack1 = false;
    }




}
