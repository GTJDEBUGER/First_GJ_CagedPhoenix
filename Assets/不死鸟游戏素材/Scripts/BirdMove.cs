using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BirdMove : MonoBehaviour
{
    [Header("鸟的相关参数")]
    [SerializeField] float changeX, changeY;
    [SerializeField] float distance, time;
    static GameObject bird;
    float deltaX;
    SpriteRenderer birdRender;
    Coroutine nowMove;
    Animator anim;
    float nowCoolTime = 0;
    float initColliderDirction;
    [Header("获取其他代码信息")]
    [SerializeReference] GameObject hero;
    HeroMainMove heroScripts;
    public static bool isLookAtFireDoor;
    [SerializeField]GameObject cinemation;

    [Header("移动速度")]
    [SerializeField] float moveSpeed;
    float horizontalInput;
    float verticalInput;

    [Header("只有符合要求的情况下你安装这个上去")]
    [SerializeField] GameObject Wind;
    [SerializeField] float speed;
    private void OnEnable()
    {
        cinemation.GetComponent<CinemachineVirtualCamera>().Follow=this.transform;
        bird = this.gameObject;
        hero = GameObject.Find("hero");
        heroScripts = hero.GetComponent<HeroMainMove>();
        heroScripts.GetBird(this.gameObject);
    }
    private void Awake()
    {
        initColliderDirction = this.GetComponent<BoxCollider2D>().offset.x;
        birdRender = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (!Cage.ChangeMoveState)
        {
            cinemation.SetActive(false);
            this.GetComponent<CircleCollider2D>().enabled = false;
            AnimationChange();
            AttackDirection();
            RayTestDoor();
            Shoot();
        }
        else
        {
            cinemation.SetActive(true);
            this.GetComponent<CircleCollider2D>().enabled = true;
            BirdAWSDMove();
        }
    }
    private void FixedUpdate()
    {
        if (Cage.ChangeMoveState)
            PhisicalMove();
        else
            bird.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
    public static void ChangeLayerToBirds()
    {
        bird.layer = LayerMask.NameToLayer("Birds");
    }
    public static void ChangeLayerTo幽灵()
    {
        bird.layer = LayerMask.NameToLayer("幽灵");
    }
    void Shoot()
    {
        if (heroScripts.attack3 && this.gameObject.CompareTag("WindBird"))
        {
            float CoolWindTime = 1;
            GameObject clone;
            if (CoolWindTime + nowCoolTime < Time.time)
            {
                if (birdRender.flipX)
                {
                    clone = Instantiate(Wind, transform.position, Quaternion.identity);
                    clone.GetComponent<Rigidbody2D>().velocity = new Vector2(speed, 0);
                }
                else
                {
                    clone = Instantiate(Wind, transform.position, transform.rotation);
                    clone.GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, 0);
                }

                nowCoolTime = Time.time;
            }
        }

    }
    void AttackDirection()
    {
        if (!heroScripts.isAttackting)
            JudgyDirection();
        else
            birdRender.flipX = false;
        if (heroScripts.isAttackting)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);//由欧拉角直接指定
            if (heroScripts.attackFacing)
            {
                birdRender.flipX = true;
                this.GetComponent<BoxCollider2D>().offset = new Vector2(-initColliderDirction, 0);
            }
            else
            {
                birdRender.flipX = false;
                this.GetComponent<BoxCollider2D>().offset = new Vector2(initColliderDirction, 0);
            }
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);//由欧拉角直接指定
        }
    }
    public IEnumerator FollowHero(Vector2 endPoint)
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
                if(nowMove!=null)
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
    public void ChangeBack()
    {
        heroScripts.isAttackingBack = false;
    }
    void RayTestDoor()
    {
        if (heroScripts.isFaceToRight)
        {
            var a = Physics2D.Raycast(hero.transform.position + new Vector3(0.5f,1,0), Vector2.right);
            if (a.collider.CompareTag("FireDoorLeft")|| a.collider.CompareTag("WindDoorLeft"))
            {
                isLookAtFireDoor = true;
            }
            else
            {
                isLookAtFireDoor = false;
            }
        }
        if (!heroScripts.isFaceToRight)
        {
            var a = Physics2D.Raycast(hero.transform.position - new Vector3(0.5f, -1, 0), Vector2.left);
            if (a.collider.CompareTag("FireDoorRight") || a.collider.CompareTag("WindDoorRight"))
            {
                isLookAtFireDoor = true;
            }
            else
            {
                isLookAtFireDoor = false;
            }
        }
      }

    void BirdAWSDMove()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if (horizontalInput > 0)
            this.GetComponent<SpriteRenderer>().flipX = false;
        if(horizontalInput<0)
            this.GetComponent<SpriteRenderer>().flipX = true;
    }
    void PhisicalMove()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(horizontalInput,verticalInput).normalized*moveSpeed;

    }


}