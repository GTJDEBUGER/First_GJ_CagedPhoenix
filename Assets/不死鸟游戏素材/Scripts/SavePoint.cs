using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public Vector2 savePosition;
    bool isEnter,isPressed;
    private void Awake()
    {
        savePosition = this.transform.position;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isEnter = true;
        }
    }
    private void Update()
    {
        if(isEnter)
            if (Input.GetKeyDown(KeyCode.E))
                isPressed=true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isPressed)
        {
            if (collision.gameObject.GetComponent<Charactors_Data>() != null)
            {
                var info = collision.gameObject.GetComponent<Charactors_Data>();
                info.position = savePosition;
                Debug.Log("Œª÷√¥¢¥Ê≥…π¶");
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        isEnter = false;
        isPressed = false;
    }

}
