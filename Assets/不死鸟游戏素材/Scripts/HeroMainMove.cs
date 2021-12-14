using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMainMove : MonoBehaviour
{
    [Header("物理组件")]
    Rigidbody2D hero;//声明角色刚体
    Collider2D coll;//声明角色碰撞体

    [Header("物理参数")]
    #region"水平移动"
    //水平移动
    [SerializeField] float maxYSpeed;
    [SerializeField] float movingSpeed;//移动速度
    [HideInInspector]public bool isFaceToRight,attackFacing;//瓦片朝向方向（初始为true）
    float movingDirection;//人物移动方向大小
    [SerializeField]public Vector2 heroPosition;//所在位置
    #endregion
    #region"跳跃"
    //跳跃
    [SerializeField] LayerMask Ground;//实体地面判断
    bool isOnGround;//是否在地面上
    bool isJump, isKeepJumping, jumpPressed, isJumpHold;//能否跳跃
    int jumpTimes = 1, maxJumpTimes = 1;//跳跃次数
    float jumpBeginTime;//跳跃开始时间
    public bool canDoubleJump;
    [SerializeField] float jumpKeepTime;//可持续跳跃时间；
    [SerializeField] float jumpForce, doubleJumpForce, keepJumpForce;//瞬时跳跃力和持续跳跃力大小
    #endregion
    #region"翻墙跳"
    //翻墙跳
    bool isTouchingWall1, isTouchingWall2, isTouchingWall3;//水平1，水平2,竖直3，三垂直判断法
    bool canOverWallJump;//翻墙的力的大小
    [SerializeField] float overWallForce;//翻过墙的力
    bool OneTimeOverWall;
    [SerializeField] float antiForce;
    #endregion
    #region"发射粘蛋"
    //发射粘蛋
    [HideInInspector] public bool isPlacingEgg,canYouPlacingEgg;
    public float maxDistance;
    GameObject point;//创建出的蛋实例
    [Header("动画组件")]
    Animator anim;
    #endregion
    #region "普通攻击"
    //普通攻击
    bool heroAttack;//英雄攻击动作
    [HideInInspector] public bool attack1, attack2, attack3,isAttackingBack,isAttackting,isMovingToAttack;
    //按下J键，人物比出攻击动作，初始帧会调回鸟，接近结束时攻击1变true，计时并开始攻击
    //如果超过时间，则恢复到都为false的状况
    //如果没有超过时间，则再次按下时攻击2变true。并且开始再次计时
    //如果超过二次计时时间或小于三段触发时间，则均全变FALSE
    //如果在一定时间内按下，则攻击3触发，最后攻击全部归FALSE；
    [HideInInspector] public Vector2 birdAttackPostion;
    [HideInInspector] public bool canYouJudgyAttack2,isEnterAttack2;
    [HideInInspector] public bool canYouJudgyAttack3, isEnterAttack3,isEnterWrong;
    [SerializeField] float attackCoolTime;
    float attackBeginTime;
    [SerializeField] float attackDistance;

    #endregion
    //开门
    bool isMeetLeft, isMeetRight;
    [SerializeField]float 最大攻击前移动时间;
    public static bool isTouchingFireDoor;


    [Header("图形层")]
    SpriteRenderer heroSprite;//获取hero图片信息
    [SerializeField] float heroXoffset1, heroXoffset2;
    [SerializeField] float heroYOffset, lengthDown1;//Y坐标 和向下射线长度
    [SerializeField] float rightRayY1, rightRayY2, lengthHorizontal;//上下偏移距离和向右发出射线长度
    [SerializeField] float downRayX, rightRayY3, lengthDown2;//X坐标，Y坐标，长度

    [Header("预制体")]
    public GameObject egg;//把蛋的实例物体放上去
    public GameObject detectPoint;
    GameObject bird;

    [Header("储存文件")]
    Charactors_Data data;
    Coroutine move;
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
        if (!Cage.ChangeMoveState)
        {
            DoPhiscalJudgy();
            DoAnimationChangeAndJugdy();
        }
        else
        {
            anim.SetFloat("Speed",0);
            anim.SetFloat("HorizontalSpeed", hero.velocity.y);
            anim.SetBool("isOnGround",true);
        }
    }

    private void FixedUpdate()
    {
        if (!Cage.ChangeMoveState)
        {
            DoMoving();
        }
        else
        {
            hero.velocity = new Vector2(0,hero.velocity.y);
        }
    }
    void DoPhiscalJudgy()//物理输入判断！(update)
    { 
        heroPosition = this.transform.position;//当前角色位置
        if (!heroAttack)
        {
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
                if (isOnGround && jumpPressed != true)
                {
                    jumpTimes = maxJumpTimes;
                }

                if (Input.GetButtonDown("Jump") && isOnGround)//地面起始跳
                {
                    jumpBeginTime = Time.time;

                    jumpPressed = true;
                }
                else if (jumpBeginTime + jumpKeepTime > Time.time && isJumpHold)
                {
                    hero.AddForce(Vector2.up * keepJumpForce * Time.deltaTime, ForceMode2D.Impulse);
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
                if (isTouchingWall3 && !isTouchingWall2 && isTouchingWall1 && !OneTimeOverWall)
                    canOverWallJump = true;
                else
                    canOverWallJump = false;
                if (canOverWallJump)
                {
                    hero.AddForce(Vector2.up * antiForce, ForceMode2D.Force);
                }

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
                    if (OneTimeOverWall)
                        hero.velocity = new Vector2(hero.velocity.x, 0);
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
        }
        else 
        {
            hero.velocity = Vector2.zero;
        }
        //鸟攻击
        {
           /* Debug.Log("attack1 is" + attack1);
            Debug.Log("attack2 is" + attack2);
            Debug.Log("attack3 is" + attack3);
            Debug.Log("isAttackingBack is" + isAttackingBack);
            Debug.Log("isMovingToAttack is" + isMovingToAttack);*/
            isAttackting = attack1 || attack2 || attack3 || isAttackingBack || isMovingToAttack;
            if (!attack1 && attackBeginTime + attackCoolTime < Time.time)
            {
                if (Input.GetKeyDown(KeyCode.J))//一段攻击判断
                {
                    if (isFaceToRight)
                        birdAttackPostion = new Vector2(heroPosition.x + attackDistance, heroPosition.y);
                    if(!isFaceToRight)
                        birdAttackPostion = new Vector2(heroPosition.x - attackDistance, heroPosition.y);
                    heroAttack = true;
                    attackFacing = isFaceToRight;
                    isMovingToAttack = true;
                    attackBeginTime = Time.time;       
                }
            }
            if (isMovingToAttack)//移动到攻击目标攻击
            { 
                    bird.transform.position = Vector2.Lerp(bird.transform.position,birdAttackPostion,20*Time.deltaTime);
            
                if (Vector2.Distance(bird.transform.position, birdAttackPostion) < 0.05f)
                {
                    attack1 = true;
                    isMovingToAttack = false;
                }   
                if (attackBeginTime + 最大攻击前移动时间 < Time.time)
                {
                    attack1 = true;
                    isMovingToAttack = false;
                }
            }
            if (canYouJudgyAttack2)
            {
                if (Input.GetKeyDown(KeyCode.J))
                {
                    isEnterAttack2 = true;
                    attackBeginTime = Time.time;
                }
            }
            if (attack2 && attack1)
            {
                if (!canYouJudgyAttack3)
                {
                    if (Input.GetKeyDown(KeyCode.J))
                    {
                        attackBeginTime = Time.time;
                        isEnterAttack3 = false;
                        isEnterWrong = true;
                        Debug.Log("判定失败");
                    }
                }

                if (canYouJudgyAttack3)
                {
                    if (!isEnterWrong)
                    {
                        if (Input.GetKeyDown(KeyCode.J))
                        {
                            attackBeginTime = Time.time;
                            Debug.Log("判定成功");
                            isEnterAttack3 = true;
                        }
                    }
                }

            }

        }
        
    }
    void DoMoving()//物理行动变化！(fixedupdate)
    {
        if (Mathf.Abs( hero.velocity.y) > maxYSpeed)
        {
            if (hero.velocity.y > 0)
                hero.velocity = new Vector2(hero.velocity.x,maxYSpeed);
            if (hero.velocity.y < 0)
                hero.velocity = new Vector2(hero.velocity.x,-maxYSpeed);
        }
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
        if (!heroAttack)
        {
            {
                hero.velocity = new Vector2(movingDirection * movingSpeed, hero.velocity.y);
            }
            
        }
        else
        {
            hero.velocity = Vector2.zero;
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
        anim.SetBool("Attack", heroAttack);
    }

    public void ChangeAttack()
    {
        heroAttack = false;
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
        /*//最远放置球距离
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(heroPosition, maxDistance);
        }*/
    }
    public void GetBird(GameObject nowBird)
    {
        bird = nowBird;
        attack1 = false;
        attack2 = false;
        attack3 = false;
        isAttackingBack = false;
        isMovingToAttack = false;
        if (move != null)
            StopCoroutine(move);
    }
}
