using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSons : MonoBehaviour
{
    [SerializeField] LayerMask FireBirds;
    public bool hitDoor;
    void Start()
    {
        hitDoor = false;
    }
    private void Update()
    {
        this.transform.parent.GetComponent<Door>().hitDoor = hitDoor;
        if (hitDoor)
        {
            Invoke("StopWait", 0.5f);
        }

    }

    void StopWait()
    {
        hitDoor = false;
        StopAllCoroutines();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.IsTouchingLayers(FireBirds))
        { 
            hitDoor = true;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.IsTouchingLayers(FireBirds))
        {
            hitDoor = true;
        }
    }

}
