using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMove : MonoBehaviour
{
    [Header("鸟的相关参数")]
    [SerializeField] float changeX,changeY;
    [SerializeField] float distance,time;
    float deltaX;
    SpriteRenderer birdRender;
    Coroutine nowMove;
    Animator anim;
    [Header("获取其他代码信息")]
    [SerializeReference]GameObject hero;
    HeroMainMove heroScripts;
    
    private void Awake()
    {
        birdRender = GetComponent<SpriteRenderer>();
        heroScripts = hero.GetComponent<HeroMainMove>();
        anim = GetComponent<Animator>();    
    }
 
    private void Update()
    {
        JudgyDirection();
        AnimationChange();
        if (heroScripts.isAttackting)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);//由欧拉角直接指定
            //this.GetComponent<BoxCollider2D>().offset = new Vector2(-this.GetComponent<BoxCollider2D>().offset.x, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);//由欧拉角直接指定
        }
        /*if (heroScripts.isAttackingBack)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);//由欧拉角直接指定
            this.GetComponent<BoxCollider2D>().offset = new Vector2(-this.GetComponent<BoxCollider2D>().offset.x,0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 180, 0);//由欧拉角直接指定
            this.GetComponent<BoxCollider2D>().offset = new Vector2(-this.GetComponent<BoxCollider2D>().offset.x, 0);
        }*/
    }


    IEnumerator FollowHero(Vector2 endPoint)
    {
            transform.position = Vector2.Lerp(transform.position, endPoint, time * Time.deltaTime);
            yield return null;
    }
    void JudgyDirection()//鸟的跟踪与图片反转
    {
        deltaX = hero.transform.position.x - transform.position.x;
        if (deltaX > 0)
        {
            birdRender.flipX =false;
        }
        if (deltaX < 0)
        {
            birdRender.flipX = true;
        }
        if (!heroScripts.isAttackting)
        {
            if (Vector2.Distance(transform.position, hero.transform.position) > 1.5f)
            {
                if (deltaX > 0)
                    nowMove = StartCoroutine(FollowHero(new Vector2(hero.transform.position.x + changeX, hero.transform.position.y + changeY)));
                if (deltaX < 0)
                    nowMove = StartCoroutine(FollowHero(new Vector2(hero.transform.position.x - changeX, hero.transform.position.y + changeY)));
            }
            else
            {
                StopCoroutine(nowMove);
            }
        }

    }
    void AnimationChange()
    {
        anim.SetBool("Attack1", heroScripts.attack1);
        anim.SetBool("Attack2", heroScripts.attack2);
        anim.SetBool("Attack3", heroScripts.attack3);
        anim.SetBool("isAttackBack",heroScripts.isAttackingBack);
    }
    public void ChangeAttack1()
    {
        if (!heroScripts.attack2)
        {
            heroScripts.attack1 = false;
            heroScripts.isAttackingBack = true;
        }
    }
    public void ChangeBack()
    {
        heroScripts.isAttackingBack = false;
    }

}