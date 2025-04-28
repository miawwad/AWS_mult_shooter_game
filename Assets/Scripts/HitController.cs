using UnityEngine;

public class HitController : MonoBehaviour
{
    public PlayerController playerController;

    private bool canReceiveHit = true;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Fist" && canReceiveHit)
        {
            FistController fistController = col.GetComponent<FistController>();

            if (!fistController.IsCurrentPlayer(playerController.GetPlayerId()) && fistController.IsPunching())
            {
                canReceiveHit = false;
                playerController.ReceiveHitToHead();
            }
        }
    }

    public void ResetReceiveHit()
    {
        canReceiveHit = true;
    }
}
