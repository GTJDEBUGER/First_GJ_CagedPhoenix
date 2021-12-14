using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindMove : MonoBehaviour
{
    [SerializeField] float keepTime;
    [SerializeField] LayerMask Ground;
    void Update()
    {
        Invoke("DestroyThis",keepTime);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.name=="left"|| collision.collider.name=="right"&&collision.transform.parent.CompareTag("WindDoor"))
            collision.transform.parent.GetComponent<Door>().hitDoor = true;
        if (collision.collider.IsTouchingLayers(Ground))
        {
            Debug.Log(collision.collider.name);
            Destroy(this.gameObject);
        }
    }
    void DestroyThis()
    {
        Debug.Log("×Óµ¯´Ý»ÙÁË");
        Destroy(this.gameObject);
    }
}
