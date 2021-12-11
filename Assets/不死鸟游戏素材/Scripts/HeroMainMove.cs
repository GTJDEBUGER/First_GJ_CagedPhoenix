using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMainMove : MonoBehaviour
{
    [Header("物理组件")]
    Rigidbody2D hero;//声明角色刚体
    Collider2D coll;//声明角色碰撞体

    [Header("物理参数")]
    //水平移动
    [SerializeField] float movingSpeed;//移动速度
    bool isFaceToRight;//瓦片朝向方向（初始为true）
    float movingDirection;//人物移动方向大小
    [SerializeField]public Vector2 heroPosition;//所在位置

    //跳跃
    [SerializeField] LayerMask Ground;//实体地面判断
    bool isOnGround;//是否在地面上
    bool isJump, isKeepJumping, jumpPressed, isJumpHold;//能否跳跃
    int jumpTimes = 1, maxJumpTimes = 1;//跳跃次数
    float jumpBeginTime;//跳跃开始时间
    public bool canDoubleJump;
    [SerializeField] float jumpKeepTime;//可持续跳跃时间；
    [SerializeField] float jumpForce, doubleJumpForce, keepJumpForce;//瞬时跳跃力和持续跳跃力大小

    //翻墙跳
    bool isTouchingWall1, isTouchingWall2, isTouchingWall3;//水平1，水平2,竖直3，三垂直判断法
    bool canOverWallJump;//翻墙的力的大小
    [SerializeField] float overWallForce;//翻过墙的力
    bool OneTimeOverWall;

    //发射粘蛋
    [HideInInspector] public bool isPlacingEgg,canYouPlacingEgg;
    public float maxDistance;
    GameObject point;//创建出的蛋实例
    [Header("动画组件")]
    Animator anim;

    //普通攻击
    [HideInInspector] public bool attack1, attack2, attack3,isAttackingBack,isAttackting;
    //按下J键，攻击1变true，并且开始计时
    //如果超过时间，则恢复到都为false的状况
    //如果没有超过时间，则再次按下时攻击2变true。并且开始再次计时
    //如果超过二次计时时间或小于三段触发时间，则均全变FALSE
    //如果在一定时间内按下，则攻击3触发，最后攻击全部归FALSE；
    [SerializeField] float waitForNextattack1,waitForNextattack2;//可等待时间
    float attackBeginTime;



    [Header("图形层")]
    SpriteRenderer heroSprite;//获取hero图片信息
    [SerializeField] float heroXoffset1, heroXoffset2;
    [SerializeField] float heroYOffset, lengthDown1;//Y坐标 和向下射线长度
    [SerializeField] float rightRayY1, rightRayY2, lengthHorizontal;//上下偏移距离和向右发出射线长度
    [SerializeField] float downRayX, rightRayY3, lengthDown2;//X坐标，Y坐标，长度

    [Header("预制体")]
    public GameObject egg;//把蛋的实例物体放上去
    public GameObject detectPoint;
    public GameObject bird;

    [Header("储存文件")]
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
    void DoPhiscalJudgy()//物理输入判断！(update)
    { 
        heroPosition = this.transform.position;//当前角色位置
        
        //移动和朝向判据
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
        //跳跃判据
        {
            if(isOnGround&&jumpPressed!=true)
            {
                jumpTimes = maxJumpTimes;
            }

            if (Input.GetButtonDown("Jump")&&isOnGround)//地面起始跳
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
                if (Input.GetButtonDown("Jump") && !isOnGround && jumpTimes > 0)//空中多段跳
                {
                    jumpPressed = true;
                }
            }//二段跳才可以用！
            isJumpHold = Input.GetButton("Jump");
        }
        //翻墙改变
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
        //发射粘蛋
        {
          
            if (!isPlacingEgg)//只有能放蛋的情况下才能创建判断实例
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
        //鸟攻击
        {
            Debug.Log(isAttackingBack);
            isAttackting = attack1 || attack2 || attack3 || isAttackingBack;
            if (!attack1)
            {
                if (Input.GetKeyDown(KeyCode.J))//一段攻击判断
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
                Debug.Log("按下J");
                attack2 = true;
                attackBeginTime = Time.time;
            }*/
        }
    }
    void DoMoving()//物理行动变化！(fixedupdate)
    {
        //射线判断
        {
            isOnGround = Physics2D.Raycast(new Vector2(heroPosition.x+heroXoffset1, heroPosition.y - heroYOffset), Vector2.down, lengthDown1, Ground)||
                Physics2D.Raycast(new Vector2(heroPosition.x+heroXoffset2, heroPosition.y - heroYOffset), Vector2.down, lengthDown1, Ground);//是否触底

            if (isFaceToRight)
            {
                isTouchingWall1 = Physics2D.Raycast(heroPosition - new Vector2(0, rightRayY1), Vector2.right, lengthHorizontal, Ground);//是否触墙
                isTouchingWall2 = Physics2D.Raycast(heroPosition - new Vector2(0, rightRayY2), Vector2.right, lengthHorizontal, Ground);//是否触墙
                isTouchingWall3 = Physics2D.Raycast(heroPosition - new Vector2(downRayX, rightRayY3), Vector2.down, lengthDown2, Ground);//是否触墙
            }
            else
            {
                isTouchingWall1 = Physics2D.Raycast(heroPosition - new Vector2(0, rightRayY1), Vector2.left, lengthHorizontal, Ground);//是否触墙
                isTouchingWall2 = Physics2D.Raycast(heroPosition - new Vector2(0, rightRayY2), Vector2.left, lengthHorizontal, Ground);//是否触墙
                isTouchingWall3 = Physics2D.Raycast(heroPosition - new Vector2(-downRayX, rightRayY3), Vector2.down, lengthDown2, Ground);//是否触墙
            }
        }
        //移动改变
        {
            hero.velocity = new Vector2(movingDirection * movingSpeed, hero.velocity.y);
        }
        //跳跃改变
        {
            if (isOnGround&&jumpPressed)//在地面上跳跃时这样判定
            {
                hero.velocity = new Vector2(hero.velocity.x, jumpForce);
                jumpTimes--;
                jumpPressed = false;
            }
            if (canDoubleJump)
            {
                if (jumpPressed && !isOnGround && jumpTimes > 0)//空中多段跳这样判定
                {
                    hero.velocity = new Vector2(hero.velocity.x, doubleJumpForce);
                    jumpTimes--;
                    jumpPressed = false;
                }
            }
        }
        //
    }
    void DoAnimationChangeAndJugdy()//动画改变！
    {
        anim.SetFloat("Speed",Mathf.Abs(movingDirection));
        anim.SetFloat("HorizontalSpeed", hero.velocity.y);
        anim.SetBool("isOnGround", isOnGround);
    }


    private void OnDrawGizmos()
    {
        //判断是否在地面上射线
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
        //判断正前方是否有物体
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
        //翻越墙的向下射线判断
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
        //最远放置球距离
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(heroPosition, maxDistance);
        }
    }
}
