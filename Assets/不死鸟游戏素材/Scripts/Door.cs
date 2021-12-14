using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool hitDoor;
    public bool isTrueDirection;
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        Invoke("StopWait", 1f);
    }
    
    private void Update()
    {
        isTrueDirection =BirdMove.isLookAtFireDoor;
        if (hitDoor && isTrueDirection)
        {
            OpenDoor();
        }
    }
    public void OpenDoor()
    {
        anim.SetBool("OpenDoor",true);
    }
    public void OpenedDoor()
    {
        anim.SetBool("OpenedDoor",true);
    }
    void StopWait()
    {
        hitDoor = false;
    }
}
