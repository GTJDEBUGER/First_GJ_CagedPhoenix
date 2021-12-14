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
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("FireBird")&&this.transform.CompareTag("FireDoorRight")||this.transform.CompareTag("FireDoorLeft"))
        {
            this.transform.parent.GetComponent<Door>().hitDoor = hitDoor;
            hitDoor = true;
        }
        if (collision.CompareTag("Wind") && this.transform.CompareTag("WindDoorRight") || this.transform.CompareTag("WindDoorLeft"))
        {
            Debug.Log(collision);
            this.transform.parent.GetComponent<Door>().hitDoor = hitDoor;
            hitDoor = true;
        }
    }
}
