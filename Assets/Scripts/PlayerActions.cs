using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public GameController gameController;

    private void Update()
    {
        if (gameController.IsPlaying())
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                OnRotateLeft();
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                OnStopRotate();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                OnRotateRight();
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                OnStopRotate();
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                OnRun();
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                OnStopMove();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                OnWalkBack();
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                OnStopMove();
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                OnPunch();
            }
        }
    }

    public void OnWalkBack()
    {
        gameController.WalkBack();
    }

    public void OnRun()
    {
        gameController.RunPlayer();
    }

    public void OnStopMove()
    {
        gameController.StopMoving();
    }

    public void OnPunch()
    {
        gameController.PunchPlayer();
    }

    public void OnRotateLeft()
    {
        gameController.RotateLeftPlayer();
    }

    public void OnRotateRight()
    {
        gameController.RotateRightPlayer();
    }

    public void OnStopRotate()
    {
        gameController.StopRotate();
    }
}
