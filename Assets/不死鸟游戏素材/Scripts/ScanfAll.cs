using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanfAll : MonoBehaviour
{
    Collider2D[] colliders=new Collider2D[50];
    private void Awake()
    {
        InvokeRepeating("ScanfAndOpen",3,5);
    }
    void ScanfAndOpen()
    {

        var a=Physics2D.OverlapCircleNonAlloc(this.transform.position,20,colliders);
        if (colliders != null)
        {
            foreach (var item in colliders)
            {
                Debug.Log(item);
                if (item != null)
                {
                    if (!item.gameObject.activeInHierarchy)
                    {
                        item.gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}
