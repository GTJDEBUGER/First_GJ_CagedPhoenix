using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour
{
    public static bool ChangeMoveState;
    bool isEnter, isPressed;
    public LayerMask Birds;
    private void Update()
    {
        if (isEnter)
            if (Input.GetKeyDown(KeyCode.E))
                isPressed = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.CompareTag("Player"))
        {
            isEnter = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log(isEnter+"aaa"+isPressed);
        if (collision.CompareTag("Player"))
        {
            if (isPressed)
            {
                ChangeMoveState = true;
                BirdMove.ChangeLayerTo”ƒ¡È();
                isPressed = false;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        BirdMove.ChangeLayerToBirds();
        isEnter = false;
        isPressed = false;
        ChangeMoveState = false;
    }
}
