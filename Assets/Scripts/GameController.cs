using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Controllers")]
    public CharacterCustomizer characterController;
    public Transform spawnPoint;
    public UIController uIController;
    public ServerConnect client;

    [Header("Objects")]
    public GameObject[] playerModels;
    public GameObject mainCamera;

    private Dictionary<int, PlayerController> players;
    private PlayerController currentPlayer;
    private bool isPlaying;


    void Start()
    {
        players = new Dictionary<int, PlayerController>();        
    }

    public void Play()
    {
        Reset();

        uIController.HidePopups();
        uIController.HideMain();
        uIController.ShowLoading();

        Invoke("InitServer", 0.1f);
    }

    public void Respawn()
    {
        currentPlayer.RespawnCurrentPlayer();
    }

    public void GiveUp()
    {
        client.DisconnectFromServer();
        uIController.ShowMain();
    }

    public void Reset()
    {
        isPlaying = false;
        currentPlayer = null;

        foreach (PlayerController playerController in players.Values)
        {
            Destroy(playerController.gameObject);
        }

        mainCamera.SetActive(true);
        players.Clear();        
    }

    public void InitServer()
    {
        client.ConnectToGameLiftServer();
    }

    public void OnCurrentPlayerAccepted(int playerId, List<PlayerData> playersData)
    {
        AddCurrentPlayer(playerId);
       
        for (int i = 0; i < playersData.Count; i++)
        {
            AddNewPlayer(playersData[i]);
        }    
    }

    public void OnPlayerAccepted(int playerId)
    {
        players.Add(playerId, null);
    }

    private void AddCurrentPlayer(int playerId)
    {
        currentPlayer = Instantiate(
            playerModels[characterController.GetSelectedIndex()],
            spawnPoint.position,
            Quaternion.identity
        ).GetComponent<PlayerController>();


        players.Add(playerId, currentPlayer);
   
        currentPlayer.InitCurrentPlayer(playerId, characterController.GetSelectedIndex(), characterController.GetNickName());
    }

    private void AddNewPlayer(PlayerData playerData)
    {
        PlayerController playerController = Instantiate(playerModels[playerData.charId], playerModels[playerData.charId].transform.parent).GetComponent<PlayerController>();
        playerController.SetLooAtConstraint(currentPlayer.constraint);

        if (!players.TryAdd(playerData.id, playerController))
        {
            players[playerData.id] = playerController;
        }
        
        UpdatePlayer(playerController, playerData);
    }

    public void OnPlayerChanged(PlayerData playerData)
    {
        PlayerController playerController = players[playerData.id];

        if (playerController == null)
        {
            AddNewPlayer(playerData);
        }
        else
        {
            UpdatePlayer(playerController, playerData);
        }
    }

    private void UpdatePlayer(PlayerController playerController, PlayerData playerData)
    {
        playerController.SetPlayerData(playerData);

        if (playerData.con == 1)
        {
            playerController.SetPlayerName();
            playerController.SetPlayerLife();
            playerController.SetPlayerPosAndRot();
            playerController.ShowPlayer();
            playerController.SetPlayerAnimation();
        }
    }

    public void OnCurrentPlayerChanged()
    {
        if (!isPlaying)
        {
            currentPlayer.SetPlayerName();
            currentPlayer.SetPlayerLife();
            currentPlayer.SetPlayerPosAndRot();
            currentPlayer.ShowCurrentPlayerCam();
            currentPlayer.ShowPlayer();

            uIController.HideLoading();
            gameObject.SetActive(true);
            isPlaying = true;
        }
    }

    public void OnPlayerDisconnected(int playerId)
    {
        if (players[playerId] != null)
        {
            Destroy(players[playerId].gameObject);
        }

        players.Remove(playerId);
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public void RunPlayer()
    {
        if (currentPlayer.CanRun())
        {
            currentPlayer.MoveRun();
            currentPlayer.Run();
        }
    }

    public void PunchPlayer()
    {
        if (currentPlayer.IsRunning())
        {
            currentPlayer.FlyingPunch();
        }
        else
        {
            currentPlayer.Punch();
        }
    }

    public void RotateLeftPlayer()
    {
        currentPlayer.RotateLeft();
    }

    public void RotateRightPlayer()
    {
        currentPlayer.RotateRight();
    }

    public void StopRotate()
    {
        currentPlayer.StopRotate();
    }

    public void WalkBack()
    {
        if (currentPlayer.CanWalkBack())
        {
            currentPlayer.MoveBack();
            currentPlayer.WalkBack();
        }
    }

    public void StopMoving()
    {
        currentPlayer.StopMoving();
        currentPlayer.StopAction();
    }
}

