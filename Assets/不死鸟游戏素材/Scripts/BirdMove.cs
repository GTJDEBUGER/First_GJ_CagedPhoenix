using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BirdMove : MonoBehaviour
{
    [Header("�����ز���")]
    [SerializeField] float changeX, changeY;
    [SerializeField] float distance, time;
    static GameObject bird;
    float deltaX;
    SpriteRenderer birdRender;
    Coroutine nowMove;
    Animator anim;
    float nowCoolTime = 0;
    float initColliderDirction;
    [Header("��ȡ����������Ϣ")]
    [SerializeReference] GameObject hero;
    HeroMainMove heroScripts;
    public static bool isLookAtFireDoor;
    [SerializeField]GameObject cinemation;

    [Header("�ƶ��ٶ�")]
    [SerializeField] float moveSpeed;
    float horizontalInput;
    float verticalInput;

    [Header("ֻ�з���Ҫ���������㰲װ�����ȥ")]
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
    public static void ChangeLayerTo����()
    {
        bird.layer = LayerMask.NameToLayer("����");
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
            transform.eulerAngles = new Vector3(0, 180, 0);//��ŷ����ֱ��ָ��
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
            transform.eulerAngles = new Vector3(0, 0, 0);//��ŷ����ֱ��ָ��
        }
    }
    public IEnumerator FollowHero(Vector2 endPoint)
    {
            transform.position = Vector2.Lerp(transform.position, endPoint, time * Time.deltaTime);
            yield return null;
    }
    void JudgyDirection()//��ĸ�����ͼƬ��ת
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