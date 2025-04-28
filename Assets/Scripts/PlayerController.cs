using UnityEngine.Animations;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Controllers")]
    public UIController uIController;
    public ServerConnect serverConnect;
    public HitController hitController;
    public GameController gameController;

    [Header("Objects")]
    public TMP_Text playerName;
    public GameObject playerLife;
    public LookAtConstraint lookAtConstraintName;
    public LookAtConstraint lookAtConstraintLife;
    public Transform constraint;
    public Camera playerCam;
    public Animator animator;

    [Header("Data")]
    public PlayerData playerData;
    public bool isCurrentPlayer;

    private bool running;
    private bool walkingBack;
    private bool rotateLeft;
    private bool rotateRight;
    private bool punching;
    private bool flyPunching;
    private bool hitToHead;
    private bool isDied;

    private PlayerStatus oldStatus;

    public bool hasGun = false;  // for gun functionality. if equip no punch.


    // Shared functions
    private void Start()
    {        
        animator.ResetTrigger("Punch");
        animator.ResetTrigger("Run");
    }

    void OnAnimatorMove()
    {
        if (animator)
        {
            if (running)
            {
                transform.position += transform.forward * Utils.runSpeed * Time.deltaTime;
            }
            else if (walkingBack)
            {
                transform.position += transform.forward * Utils.walkBack * Time.deltaTime;
            }

            if (rotateLeft)
            {
                transform.Rotate(Vector3.up * -Utils.rotateSpeed * Time.deltaTime);
            }
            else if (rotateRight)
            {
                transform.Rotate(Vector3.up * Utils.rotateSpeed * Time.deltaTime);
            }

            if (isCurrentPlayer)
            {
                playerData.pos.x = transform.position.x;
                playerData.pos.y = transform.position.y;
                playerData.pos.z = transform.position.z;

                playerData.rot.x = transform.rotation.x;
                playerData.rot.y = transform.rotation.y;
                playerData.rot.z = transform.rotation.z;
                playerData.rot.w = transform.rotation.w;
            }
        }
    }

    public void SetPlayerData(PlayerData playerData)
    {
        if (oldStatus == null)
        {
            oldStatus = new PlayerStatus();
        }

        oldStatus.move = this.playerData.status.move;
        oldStatus.rot = this.playerData.status.rot;
        oldStatus.action = this.playerData.status.action;

        this.playerData.id = playerData.id;
        this.playerData.charId = playerData.charId;
        this.playerData.life = playerData.life;
        this.playerData.name = playerData.name;
        this.playerData.pos.x = playerData.pos.x;
        this.playerData.pos.y = playerData.pos.y;
        this.playerData.pos.z = playerData.pos.z;
        this.playerData.rot.x = playerData.rot.x;
        this.playerData.rot.y = playerData.rot.y;
        this.playerData.rot.z = playerData.rot.z;
        this.playerData.rot.w = playerData.rot.w;
        this.playerData.con = playerData.con;
        this.playerData.status.move = playerData.status.move;
        this.playerData.status.rot = playerData.status.rot;
        this.playerData.status.action = playerData.status.action;
    }

    public void SetPlayerName()
    {
        playerName.text = playerData.name;
    }

    public void SetPlayerLife()
    {
        playerLife.transform.localScale = new Vector3(playerData.life / 100.0f, playerLife.transform.localScale.y, playerLife.transform.localScale.z);
        playerLife.transform.localPosition = new Vector3((playerData.life / 100.0f - 1) / 2, playerLife.transform.localPosition.y, playerLife.transform.localPosition.z);
    }

    public void SetPlayerPosAndRot()
    {
        transform.position = new Vector3(playerData.pos.x, transform.position.y, playerData.pos.z);
        transform.rotation = new Quaternion(playerData.rot.x, playerData.rot.y, playerData.rot.z, playerData.rot.w);
    }

    public void SetLooAtConstraint(Transform constraint)
    {
        ConstraintSource source = lookAtConstraintName.GetSource(0);
        source.sourceTransform = constraint;
        lookAtConstraintName.SetSource(0, source);

        source = lookAtConstraintLife.GetSource(0);
        source.sourceTransform = constraint;
        lookAtConstraintLife.SetSource(0, source);
    }

    public void ShowPlayer()
    {
        gameObject.SetActive(true);
    }

    public bool CanRun()
    {
        return !(running || punching || flyPunching || hitToHead || isDied);
    }

    public void Run()
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("WalkBack");
        animator.SetTrigger("Run");

        if (isCurrentPlayer)
        {
            playerData.status.action = Utils.statusActionRun;
        }
    }

    public void MoveRun()
    {
        walkingBack = false;
        running = true;

        if (isCurrentPlayer)
        {
            playerData.status.move = Utils.statusMoveRun;
            InvokeRepeating("SendCurrentDataToServer", 0, Utils.sendPlayerDataInterval);
        }
    }

    public bool IsRunning()
    {
        return running;
    }

    public bool CanWalkBack()
    {
        return !(punching || flyPunching || hitToHead || walkingBack || isDied);
    }

    public void WalkBack()
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Run");
        animator.SetTrigger("WalkBack");

        if (isCurrentPlayer)
        {
            playerData.status.action = Utils.statusActionWalkBack;
        }
    }

    public void MoveBack()
    {
        running = false;
        walkingBack = true;

        if (isCurrentPlayer)
        {
            playerData.status.move = Utils.statusMoveBack;
            InvokeRepeating("SendCurrentDataToServer", 0, Utils.sendPlayerDataInterval);
        }
    }

    public void StopMoving()
    {
        running = false;
        walkingBack = false;

        if (isCurrentPlayer)
        {
            playerData.status.move = Utils.statusNoMove;
            CancelInvoke("SendCurrentDataToServer");
        }
    }

    private bool CanPunch()
    {
        return !(punching || flyPunching || hitToHead || walkingBack || isDied);
    }

    public void Punch()
    {
        if (hasGun) return; // 🔫 Don't punch if holding a gun

        if (CanPunch())
        {
            punching = true;
            animator.ResetTrigger("Idle");
            animator.SetTrigger("Punch");

            if (isCurrentPlayer)
            {
            playerData.status.action = Utils.statusActionPunch;
            SendCurrentDataToServer();
            }
        }
    }


    public void FlyingPunch()
    {
        if (CanPunch())
        {
            flyPunching = true;
            animator.ResetTrigger("Run");
            animator.SetTrigger("FlyPunch");

            if (isCurrentPlayer)
            {
                playerData.status.action = Utils.statusActionFlyingPunch;
                SendCurrentDataToServer();
            }
        }
    }

    public void StopAction()
    {
        running = false;
        walkingBack = false;
        punching = false;
        flyPunching = false;
        hitToHead = false;

        animator.ResetTrigger("Run");
        animator.ResetTrigger("WalkBack");        
        animator.ResetTrigger("Punch");
        animator.ResetTrigger("FlyPunch");
        animator.ResetTrigger("HitToHead");

        hitController.ResetReceiveHit();
        Idle();
    }

    public bool IsPunching()
    {
        return punching || flyPunching;
    }

    public int GetPlayerId()
    {
        return playerData.id;
    }

    public bool IsSamePlayer(int id)
    {
        return playerData.id == id;
    }

    public bool CanRotateLeft()
    {
        return !rotateLeft;
    }

    public void RotateLeft()
    {
        rotateRight = false;
        rotateLeft = true;
            
        if (isCurrentPlayer)
        {
            playerData.status.rot = Utils.statusRotateLeft;
            SendCurrentDataToServer();
        }
    }

    public bool CanRotateRight()
    {
        return !rotateRight;
    }

    public void RotateRight()
    {
        rotateLeft = false;
        rotateRight = true;

        if (isCurrentPlayer)
        {
            playerData.status.rot = Utils.statusRotateRight;
            SendCurrentDataToServer();
        }
    }

    public bool CanStopRotate()
    {
        return rotateLeft || rotateRight;
    }

    public void StopRotate()
    {
        if (CanStopRotate())
        {
            rotateLeft = false;
            rotateRight = false;

            if (isCurrentPlayer)
            {
                playerData.status.rot = Utils.statusNoRotate;
                SendCurrentDataToServer();
            }
        }
    }

    public void Idle()
    {
        isDied = false;

        animator.ResetTrigger("Die");
        animator.SetTrigger("Idle");

        if (isCurrentPlayer)
        {
            playerData.status.action = Utils.statusNoAction;
            SendCurrentDataToServer();
        }
    }

    public void HitToHead()
    {
        hitToHead = true;
        punching = false;
        flyPunching = false;
        running = false;
        walkingBack = false;

        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Punch");
        animator.ResetTrigger("FlyPunch");
        animator.ResetTrigger("Run");
        animator.ResetTrigger("WalkBack");
        animator.SetTrigger("HitToHead");

        if (isCurrentPlayer)
        {
            playerData.status.action = Utils.statusActionHitToHead;
            SendCurrentDataToServer();
        }
    }

    public void Die()
    {
        isDied = true;
        punching = false;
        flyPunching = false;
        running = false;
        walkingBack = false;

        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Punch");
        animator.ResetTrigger("FlyPunch");
        animator.ResetTrigger("Run");
        animator.ResetTrigger("WalkBack");
        animator.SetTrigger("Die");

        if (isCurrentPlayer)
        {
            uIController.ShowPopupDie();

            playerData.status.action = Utils.statusActionDie;
            SendCurrentDataToServer();
        }
    }

    public void ReceiveHitToHead()
    {
        if (isCurrentPlayer)
        {
            playerData.life = playerData.life - Utils.hitLifeValue;
            SetPlayerLife();

            if (playerData.life <= 0)
            {
                Die();
            }
            else
            {
                HitToHead();
            }
        }
    }

    public void RespawnCurrentPlayer()
    {
        playerData.life = 100;         
        playerData.status.action = Utils.statusNoAction;
        playerData.pos = new PlayerCoord
        {
            x = Random.Range(10, 20),
            y = gameController.playerModels[playerData.charId].transform.position.y,
            z = Random.Range(10, 20),
        };
        playerData.rot = new PlayerCoord();

        hitController.ResetReceiveHit();
        SetPlayerPosAndRot();
        SetPlayerLife();
        Idle();

        uIController.HidePopups();
    }

    // Functions for current player
    public void InitCurrentPlayer(int playerId, int charId, string playerName)
    {
        isCurrentPlayer = true;

        playerData = new PlayerData
        {
            id = playerId,
            charId = charId,
            life = 100,
            pos = new PlayerCoord
            {
                x = Random.Range(10, 20),
                y = gameController.playerModels[charId].transform.position.y,
                z = Random.Range(10, 20),
            },
            rot = new PlayerCoord(),
            name = playerName,
            con = 1,
            status = new PlayerStatus()
        };

        serverConnect.ChangePlayer(playerData);
    }

    private void SendCurrentDataToServer()
    {
        serverConnect.ChangePlayer(playerData);     
    }

    public void ShowCurrentPlayerCam()
    {
        gameController.mainCamera.SetActive(false);
        playerCam.gameObject.SetActive(true);
    }

    // Functions for other players
    public void SetPlayerAnimation()
    {
        if (oldStatus.move != playerData.status.move)
        {
            switch(playerData.status.move)
            {
                case Utils.statusMoveRun:
                    MoveRun();
                    break;
                case Utils.statusMoveBack:
                    MoveBack();
                    break;
                case Utils.statusNoMove:
                    StopMoving();
                    break;
            }
        }

        if (oldStatus.rot != playerData.status.rot)
        {
            switch (playerData.status.rot)
            {
                case Utils.statusRotateLeft:
                    RotateLeft();
                    break;
                case Utils.statusRotateRight:
                    RotateRight();
                    break;
                case Utils.statusNoRotate:
                    StopRotate();
                    break;
            }
        }

        if (oldStatus.action != playerData.status.action)
        {
            switch (playerData.status.action)
            {
                case Utils.statusActionRun:
                    Run();
                    break;
                case Utils.statusActionWalkBack:
                    WalkBack();
                    break;
                case Utils.statusActionPunch:
                    Punch();
                    break;
                case Utils.statusActionFlyingPunch:
                    FlyingPunch();
                    break;
                case Utils.statusActionHitToHead:
                    HitToHead();
                    break;
                case Utils.statusActionDie:
                    Die();
                    break;
                case Utils.statusNoAction:
                    StopAction();
                    break;
            }
        }
    }
}
