using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingBirds : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.layer.ToString());
        if (collision.gameObject.layer.ToString() == "сдаИ")
        {
            Cage.ChangeMoveState = false;
            BirdMove.ChangeLayerToBirds();
        }
    }
        void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer.ToString() == "сдаИ")
        {
            Cage.ChangeMoveState = false;
            BirdMove.ChangeLayerToBirds();
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer.ToString() == "сдаИ")
        {
            Cage.ChangeMoveState = false;
            BirdMove.ChangeLayerToBirds();
        }
    }
}
