using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggT : MonoBehaviour
{
    SpriteRenderer ballSprite;
    Transform Hero;
    bool istouching;
    float maxDistance;
    [SerializeField] LayerMask Ground;
    [SerializeField] float distance;
    Vector2 point;

    [Header("Prefab")]
    public GameObject Egg;
    private void Awake()
    {
     
        ballSprite = GetComponent<SpriteRenderer>();
        ballSprite.color = Color.yellow;
    }
    private void Start()
    {
        Hero = this.transform.parent;
        maxDistance = Hero.GetComponent<HeroMainMove>().maxDistance;
    }
    private void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mousePosOnScreen = Input.mousePosition;
        mousePosOnScreen.z = screenPos.z;
        Vector3 mousePosInWorld = Camera.main.ScreenToWorldPoint(mousePosOnScreen);
        Vector3 heroToMouse = mousePosInWorld - Hero.transform.position;
     
            
        if (Physics2D.Raycast(Hero.position, heroToMouse, maxDistance, Ground))
        {
            point = Physics2D.Raycast(Hero.position, heroToMouse, maxDistance, Ground).point;//获得触墙的点
            this.transform.position = point;
        }
        else
        { 
            if (Vector2.Distance(mousePosInWorld, Hero.position) < maxDistance)
            {
                this.transform.position = mousePosInWorld;
            }
            else
            {
                this.transform.position = Hero.position + heroToMouse.normalized * maxDistance;
            }
        }
        //Hero.GetComponent<HeroMainMove>().canYouPlacingEgg=istouching;
        if (istouching)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var egg=GameObject.Instantiate(Egg);
                egg.transform.position = this.transform.position;

                var normal = Physics2D.Raycast(Hero.position, heroToMouse, maxDistance, Ground).normal;

                Vector3 dir = egg.transform.position - Hero.position;
                float angle =Vector3.SignedAngle(Vector3.up, normal, Vector3.forward);
                egg.transform.eulerAngles = new Vector3(0, 0, angle);
                egg.transform.position =new Vector2(egg.transform.position.x,egg.transform.position.y)+ normal.normalized * distance;

                Destroy(this.gameObject);
                Hero.GetComponent<HeroMainMove>().isPlacingEgg = false;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Ground"))
        {
            ballSprite.color = Color.green;
            istouching = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
            ballSprite.color = Color.green;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            ballSprite.color = Color.red;
            istouching = false;       
        }
    }

}
