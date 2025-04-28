using UnityEngine;

public class FistController : MonoBehaviour
{
    public PlayerController playerController;
    
    public bool IsPunching()
    {
        return playerController.IsPunching();
    }

    public bool IsCurrentPlayer(int id)
    {
        return playerController.IsSamePlayer(id);
    }
}
