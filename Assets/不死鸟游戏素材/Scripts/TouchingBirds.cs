using UnityEngine;

public class TouchingBirds : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(LayerMask.LayerToName(collision.gameObject.layer));
        if (LayerMask.LayerToName(collision.gameObject.layer) == "сдаИ")
        {
            Cage.ChangeMoveState = false;
            BirdMove.ChangeLayerToBirds();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(LayerMask.LayerToName(collision.gameObject.layer));
        if (LayerMask.LayerToName(collision.gameObject.layer) == "сдаИ")
        {
            Cage.ChangeMoveState = false;
            BirdMove.ChangeLayerToBirds();
        }

    }
}
