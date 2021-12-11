using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMainMove : MonoBehaviour
{
    [Header("�������")]
    Rigidbody2D hero;//������ɫ����
    Collider2D coll;//������ɫ��ײ��

    [Header("�������")]
    //ˮƽ�ƶ�
    [SerializeField] float movingSpeed;//�ƶ��ٶ�
    bool isFaceToRight;//��Ƭ�����򣨳�ʼΪtrue��
    float movingDirection;//�����ƶ������С
    [SerializeField]public Vector2 heroPosition;//����λ��

    //��Ծ
    [SerializeField] LayerMask Ground;//ʵ������ж�
    bool isOnGround;//�Ƿ��ڵ�����
    bool isJump, isKeepJumping, jumpPressed, isJumpHold;//�ܷ���Ծ
    int jumpTimes = 1, maxJumpTimes = 1;//��Ծ����
    float jumpBeginTime;//��Ծ��ʼʱ��
    public bool canDoubleJump;
    [SerializeField] float jumpKeepTime;//�ɳ�����Ծʱ�䣻
    [SerializeField] float jumpForce, doubleJumpForce, keepJumpForce;//˲ʱ��Ծ���ͳ�����Ծ����С

    //��ǽ��
    bool isTouchingWall1, isTouchingWall2, isTouchingWall3;//ˮƽ1��ˮƽ2,��ֱ3������ֱ�жϷ�
    bool canOverWallJump;//��ǽ�����Ĵ�С
    [SerializeField] float overWallForce;//����ǽ����
    bool OneTimeOverWall;

    //����ճ��
    [HideInInspector] public bool isPlacingEgg,canYouPlacingEgg;
    public float maxDistance;
    GameObject point;//�������ĵ�ʵ��
    [Header("�������")]
    Animator anim;

    //��ͨ����
    [HideInInspector] public bool attack1, attack2, attack3,isAttackingBack,isAttackting;
    //����J��������1��true�����ҿ�ʼ��ʱ
    //�������ʱ�䣬��ָ�����Ϊfalse��״��
    //���û�г���ʱ�䣬���ٴΰ���ʱ����2��true�����ҿ�ʼ�ٴμ�ʱ
    //����������μ�ʱʱ���С�����δ���ʱ�䣬���ȫ��FALSE
    //�����һ��ʱ���ڰ��£��򹥻�3��������󹥻�ȫ����FALSE��
    [SerializeField] float waitForNextattack1,waitForNextattack2;//�ɵȴ�ʱ��
    float attackBeginTime;



    [Header("ͼ�β�")]
    SpriteRenderer heroSprite;//��ȡheroͼƬ��Ϣ
    [SerializeField] float heroXoffset1, heroXoffset2;
    [SerializeField] float heroYOffset, lengthDown1;//Y���� ���������߳���
    [SerializeField] float rightRayY1, rightRayY2, lengthHorizontal;//����ƫ�ƾ�������ҷ������߳���
    [SerializeField] float downRayX, rightRayY3, lengthDown2;//X���꣬Y���꣬����

    [Header("Ԥ����")]
    public GameObject egg;//�ѵ���ʵ���������ȥ
    public GameObject detectPoint;
    public GameObject bird;

    [Header("�����ļ�")]
    Charactors_Data data;

    private void Awake()
    {
        data = GetComponent<Charactors_Data>();
        hero = GetComponent<Rigidbody2D>();
        anim=GetComponent<Animator>();
        heroSprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
        this.gameObject.transform.position = data.position;
    }
    // Start is called before the first frame update
    void Start()
    {
        isFaceToRight = true;
    }
    void Update()
    {
        DoPhiscalJudgy();
        DoAnimationChangeAndJugdy();
    }
    private void FixedUpdate()
    {
        DoMoving();
    }
    void DoPhiscalJudgy()//���������жϣ�(update)
    { 
        heroPosition = this.transform.position;//��ǰ��ɫλ��
        
        //�ƶ��ͳ����о�
        {
            movingDirection = Input.GetAxis("Horizontal");
            if (movingDirection > 0)
            {
                isFaceToRight = true;
                heroSprite.flipX = false;
            }
            if (movingDirection < 0)
            {
                isFaceToRight = false;
                heroSprite.flipX = true;
            }
        }
        //��Ծ�о�
        {
            if(isOnGround&&jumpPressed!=true)
            {
                jumpTimes = maxJumpTimes;
            }

            if (Input.GetButtonDown("Jump")&&isOnGround)//������ʼ��
            {
                jumpBeginTime = Time.time;
               
                jumpPressed = true;
            }
            else if(jumpBeginTime + jumpKeepTime > Time.time && isJumpHold)
            {
                hero.AddForce(Vector2.up * keepJumpForce*Time.deltaTime, ForceMode2D.Impulse);
            }
            if (canDoubleJump)
            {
                if (Input.GetButtonDown("Jump") && !isOnGround && jumpTimes > 0)//���ж����
                {
                    jumpPressed = true;
                }
            }//�������ſ����ã�
            isJumpHold = Input.GetButton("Jump");
        }
        //��ǽ�ı�
        {
            if (isTouchingWall3 && !isTouchingWall2 && isTouchingWall1&&!OneTimeOverWall)
                canOverWallJump = true;
            else
                canOverWallJump = false;
            if (movingDirection > 0 && isFaceToRight)
            {
                if (canOverWallJump && Input.GetButtonDown("Jump"))
                {
                    hero.AddForce(Vector2.up * overWallForce, ForceMode2D.Impulse);
                    OneTimeOverWall = true;
                }
            }
            if (movingDirection < 0 && !isFaceToRight)
            {
                if (canOverWallJump && Input.GetButtonDown("Jump"))
                { 
                    hero.AddForce(Vector2.up * overWallForce, ForceMode2D.Impulse);
                    OneTimeOverWall = true;
                }
            }
            if (isOnGround)
            {
                OneTimeOverWall = false;
            }
        }
        //����ճ��
        {
          
            if (!isPlacingEgg)//ֻ���ܷŵ�������²��ܴ����ж�ʵ��
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    isPlacingEgg = true;
                    point = GameObject.Instantiate(detectPoint);
                    point.transform.position = this.transform.position;
                    point.transform.SetParent(this.transform);
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Q) && point != null)
                {
                    Destroy(point.gameObject);
                    isPlacingEgg = false;
                }
            }


        }
        //�񹥻�
        {
            Debug.Log(isAttackingBack);
            isAttackting = attack1 || attack2 || attack3 || isAttackingBack;
            if (!attack1)
            {
                if (Input.GetKeyDown(KeyCode.J))//һ�ι����ж�
                {
                    attack1 = true;
                    if(isFaceToRight)
                        bird.transform.position = new Vector2(hero.position.x+5,heroPosition.y);
                    if(!isFaceToRight)
                        bird.transform.position = new Vector2(hero.position.x-5, heroPosition.y);
                    attackBeginTime = Time.time;
                }
            }
           /* if (attackBeginTime + waitForNextattack1 > Time.time && attackBeginTime + waitForNextattack2 < Time.time)
            {
                Debug.Log("����J");
                attack2 = true;
                attackBeginTime = Time.time;
            }*/
        }
    }
    void DoMoving()//�����ж��仯��(fixedupdate)
    {
        //�����ж�
        {
            isOnGround = Physics2D.Raycast(new Vector2(heroPosition.x+heroXoffset1, heroPosition.y - heroYOffset), Vector2.down, lengthDown1, Ground)||
                Physics2D.Raycast(new Vector2(heroPosition.x+heroXoffset2, heroPosition.y - heroYOffset), Vector2.down, lengthDown1, Ground);//�Ƿ񴥵�

            if (isFaceToRight)
            {
                isTouchingWall1 = Physics2D.Raycast(heroPosition - new Vector2(0, rightRayY1), Vector2.right, lengthHorizontal, Ground);//�Ƿ�ǽ
                isTouchingWall2 = Physics2D.Raycast(heroPosition - new Vector2(0, rightRayY2), Vector2.right, lengthHorizontal, Ground);//�Ƿ�ǽ
                isTouchingWall3 = Physics2D.Raycast(heroPosition - new Vector2(downRayX, rightRayY3), Vector2.down, lengthDown2, Ground);//�Ƿ�ǽ
            }
            else
            {
                isTouchingWall1 = Physics2D.Raycast(heroPosition - new Vector2(0, rightRayY1), Vector2.left, lengthHorizontal, Ground);//�Ƿ�ǽ
                isTouchingWall2 = Physics2D.Raycast(heroPosition - new Vector2(0, rightRayY2), Vector2.left, lengthHorizontal, Ground);//�Ƿ�ǽ
                isTouchingWall3 = Physics2D.Raycast(heroPosition - new Vector2(-downRayX, rightRayY3), Vector2.down, lengthDown2, Ground);//�Ƿ�ǽ
            }
        }
        //�ƶ��ı�
        {
            hero.velocity = new Vector2(movingDirection * movingSpeed, hero.velocity.y);
        }
        //��Ծ�ı�
        {
            if (isOnGround&&jumpPressed)//�ڵ�������Ծʱ�����ж�
            {
                hero.velocity = new Vector2(hero.velocity.x, jumpForce);
                jumpTimes--;
                jumpPressed = false;
            }
            if (canDoubleJump)
            {
                if (jumpPressed && !isOnGround && jumpTimes > 0)//���ж���������ж�
                {
                    hero.velocity = new Vector2(hero.velocity.x, doubleJumpForce);
                    jumpTimes--;
                    jumpPressed = false;
                }
            }
        }
        //
    }
    void DoAnimationChangeAndJugdy()//�����ı䣡
    {
        anim.SetFloat("Speed",Mathf.Abs(movingDirection));
        anim.SetFloat("HorizontalSpeed", hero.velocity.y);
        anim.SetBool("isOnGround", isOnGround);
    }


    private void OnDrawGizmos()
    {
        //�ж��Ƿ��ڵ���������
        {
            if (isOnGround)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }
            Gizmos.DrawRay(heroPosition - new Vector2(heroXoffset1, heroYOffset), Vector2.down * lengthDown1);
            Gizmos.DrawRay(heroPosition - new Vector2(heroXoffset2, heroYOffset), Vector2.down * lengthDown1);
        }
        //�ж���ǰ���Ƿ�������
        {

            if (isFaceToRight)
            {
                if (isTouchingWall1)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;
                Gizmos.DrawRay(heroPosition - new Vector2(0, rightRayY1), Vector2.right * lengthHorizontal);
                if (isTouchingWall2)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;
                Gizmos.DrawRay(heroPosition - new Vector2(0, rightRayY2), Vector2.right * lengthHorizontal);
            }
            if (!isFaceToRight)
            {
                if (isTouchingWall1)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;
                Gizmos.DrawRay(heroPosition - new Vector2(0, rightRayY1), Vector2.left * lengthHorizontal);
                if (isTouchingWall2)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;
                Gizmos.DrawRay(heroPosition - new Vector2(0, rightRayY2), Vector2.left * lengthHorizontal);
            }
        }
        //��Խǽ�����������ж�
        {

            if (isFaceToRight)
            {
                if (isTouchingWall3)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;
                Gizmos.DrawRay(heroPosition - new Vector2(downRayX, rightRayY3), Vector2.down * lengthDown2);
            }
            if (!isFaceToRight)
            {
                if (isTouchingWall3)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;
                Gizmos.DrawRay(heroPosition - new Vector2(-downRayX, rightRayY3), Vector2.down * lengthDown2);
            }

        }
        //��Զ���������
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(heroPosition, maxDistance);
        }
    }
}
