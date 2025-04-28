using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Aws.GameLift.Realtime.Event;
using Aws.GameLift.Realtime;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.CognitoIdentity;
using Amazon;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class ServerConnect : MonoBehaviour
{
    [Header("Controllers")]
    public UIController uIController;
    public GameController gameController;
    public bool IsConnectedToServer { get; set; }
    public TMPro.TMP_Text log;

    private Client _client;
    private Queue<Action> _mainThreadQueue = new Queue<Action>();

    private const int CURRENT_PLAYER_ACCEPTED = 100;
    private const int PLAYER_ACCEPTED = 101;
    private const int PLAYER_DISCONNECTED = 102;
    private const int CURRENT_PLAYER_UPDATE = 200;
    private const int PLAYER_UPDATE = 201;


    // Start is called before the first frame update
    void Start()
    {
        IsConnectedToServer = false;
    }

    // Update is called once per frame
    void Update()
    {
        RunMainThreadQueueActions();
    }

    public async void ConnectToGameLiftServer()
    {
        log.text += "Reaching out to client service Lambda function";
        Debug.Log("Reaching out to client service Lambda function");

        try
        {
            string identityPoolId = "us-east-2:602a8446-6ff3-4035-a407-b643051e82bc";
            // paste this in from the Amazon Cognito Identity Pool console
            CognitoAWSCredentials credentials = new CognitoAWSCredentials(
                identityPoolId, // identity pool ID here
                RegionEndpoint.USEast2 // region us-east-2
            );

            log.text += "\nCognito AWS credential OK.";

            AmazonLambdaClient client = new AmazonLambdaClient(credentials, RegionEndpoint.USEast2);

            log.text += "\nAmazon Lambda Client OK.";

            InvokeRequest request = new InvokeRequest
            { 
                FunctionName = Utils.lambdaFunction,
                InvocationType = InvocationType.RequestResponse
            };

            log.text += "\nInvoking Lambda function";

            InvokeResponse response = await client.InvokeAsync(request);

            log.text += "\nGot response from Lambda function";

            if (response.FunctionError == null)
            {
                log.text += "\nNo exception invoking Lambda";

                if (response.StatusCode == 200)
                {
                    log.text += "\nStatus code 200";

                    var payload = Encoding.ASCII.GetString(response.Payload.ToArray()) + "\n";
                    var playerSessionObj = JsonUtility.FromJson<PlayerSessionObject>(payload);

                    if (playerSessionObj.FleetId == null)
                    {
                        log.text += "\nFleetId == null";
                        uIController.ShowPopupError();
                        Debug.Log($"Error in Lambda: {payload}");
                    }
                    else
                    {
                        log.text += "\nConnecting to server";
                        QForMainThread(ActionConnectToServer, playerSessionObj.IpAddress, Int32.Parse(playerSessionObj.Port), playerSessionObj.PlayerSessionId);
                    }
                }
                else
                {
                    log.text += "\nWrong status code: " + response.StatusCode;
                }
            }
            else
            {
                log.text += "\nException invoking Lambda!";
                uIController.ShowPopupError();
                Debug.LogError(response.FunctionError);
            }
        }
        catch (Exception e)
        {
            log.text += "\nException: " + e.StackTrace;
        }
    }

    public void ActionConnectToServer(string ipAddr, int port, string tokenUID)
    {
        StartCoroutine(ConnectToServer(ipAddr, port, tokenUID));
    }

    // common code whether we are connecting to a GameLift hosted server or
    // a local server
    private IEnumerator ConnectToServer(string ipAddr, int port, string tokenUID)
    {
        ConnectionToken token = new ConnectionToken(tokenUID, null);

        ClientConfiguration clientConfiguration = ClientConfiguration.Default();

        _client = new Client(clientConfiguration);
        _client.ConnectionOpen += new EventHandler(OnOpenEvent);
        _client.ConnectionClose += new EventHandler(OnCloseEvent);
        _client.DataReceived += new EventHandler<DataReceivedEventArgs>(OnDataReceived);
        _client.ConnectionError += new EventHandler<ErrorEventArgs>(OnConnectionErrorEvent);

        int UDPListenPort = FindAvailableUDPPort();
        if (UDPListenPort == -1)
        {            
            uIController.ShowPopupError();
            log.text += "\nNo UDP port available";
            Debug.Log("Unable to find an open UDP listen port");
            yield break;
        }
        else
        {
            log.text += "\nUDP OK";
            Debug.Log($"UDP listening on port: {UDPListenPort}");
        }

        log.text += "\nAttempting to connect to server";
        Debug.Log($"[client] Attempting to connect to server ip: {ipAddr} TCP port: {port} Player Session ID: {tokenUID}");
        _client.Connect(ipAddr, port, UDPListenPort, token);        

        while (true)
        {
            if (_client.ConnectedAndReady)
            {                
                IsConnectedToServer = true;
                log.text += "\nConnected!";
                Debug.Log("[client] Connected to server");
                break;
            }
            yield return null;
        }
    }

    public void DisconnectFromServer()
    {
        if (_client != null && _client.Connected)
        {
            _client.Disconnect();
        }
    }

    private void OnOpenEvent(object sender, EventArgs e)
    {
        Debug.Log("[server-sent] OnOpenEvent");
    }

    private void OnCloseEvent(object sender, EventArgs e)
    {
        Debug.Log("[server-sent] OnCloseEvent");
    }

    private void OnConnectionErrorEvent(object sender, ErrorEventArgs e)
    {
        uIController.ShowPopupError();
        Debug.Log($"[client] Connection Error! : ");
    }

    private void OnDataReceived(object sender, DataReceivedEventArgs e)
    {
        string data = Encoding.Default.GetString(e.Data);
        Debug.Log($"[server-sent] OnDataReceived - Sender: {e.Sender} OpCode: {e.OpCode} data: {data}");

        switch (e.OpCode)
        {
            case CURRENT_PLAYER_ACCEPTED:
                Debug.Log($"Current Player Accepted: {e.Sender}");
                QForMainThread(OnCurrentPlayerAccepted, e.Sender, JsonUtility.FromJson<PlayersData>(data));
                break;

            case PLAYER_ACCEPTED:
                Debug.Log($"Player Accepted: {e.Sender}");
                QForMainThread(OnPlayerAccepted, e.Sender);
                break;

            case CURRENT_PLAYER_UPDATE:
                Debug.Log($"Current player changed: {e.Sender}");
                QForMainThread(OnCurrentPlayerChanged);
                break;

            case PLAYER_UPDATE:
                Debug.Log($"Player changed: {e.Sender}");
                QForMainThread(OnPlayerChanged, JsonUtility.FromJson<PlayerData>(data));
                break;

            case PLAYER_DISCONNECTED:
                Debug.Log($"Player disconnected: {e.Sender}");
                QForMainThread(OnPlayerDisconnected, e.Sender);
                break;
        }
    }

    // given a starting and ending range, finds an open UDP port to use as the listening port
    private int FindAvailableUDPPort()
    {
        TcpListener l = new TcpListener(IPAddress.Loopback, 0);
        l.Start();
        int port = ((IPEndPoint)l.LocalEndpoint).Port;
        l.Stop();
        return port;
    }

    private void QForMainThread(Action fn)
    {
        lock (_mainThreadQueue)
        {
            _mainThreadQueue.Enqueue(() => { fn(); });
        }
    }

    private void QForMainThread<T1>(Action<T1> fn, T1 p1)
    {
        lock (_mainThreadQueue)
        {
            _mainThreadQueue.Enqueue(() => { fn(p1); });
        }
    }

    private void QForMainThread<T1, T2>(Action<T1, T2> fn, T1 p1, T2 p2)
    {
        lock (_mainThreadQueue)
        {
            _mainThreadQueue.Enqueue(() => { fn(p1, p2); });
        }
    }

    private void QForMainThread<T1, T2, T3>(Action<T1, T2, T3> fn, T1 p1, T2 p2, T3 p3)
    {
        lock (_mainThreadQueue)
        {
            _mainThreadQueue.Enqueue(() => { fn(p1, p2, p3); });
        }
    }

    private void RunMainThreadQueueActions()
    {
        // as our server messages come in on their own thread
        // we need to queue them up and run them on the main thread
        // when the methods need to interact with Unity
        lock (_mainThreadQueue)
        {
            while (_mainThreadQueue.Count > 0)
            {
                _mainThreadQueue.Dequeue().Invoke();
            }
        }
    }

    public void OnPlayerAccepted(int playerId)
    {
        gameController.OnPlayerAccepted(playerId);
    }

    public void OnCurrentPlayerAccepted(int playerId, PlayersData players)
    {
        gameController.OnCurrentPlayerAccepted(playerId, players.players);        
    }

    public void OnCurrentPlayerChanged()
    {
        gameController.OnCurrentPlayerChanged();
    }

    public void OnPlayerChanged(PlayerData playerData)
    {
        gameController.OnPlayerChanged(playerData);
    }

    public void OnPlayerDisconnected(int playerId)
    {
        gameController.OnPlayerDisconnected(playerId);
    }

    public void ChangePlayer(PlayerData playerData)
    {
        // inform server the hop button was pressed by local player
        _client.SendEvent(PLAYER_UPDATE, Encoding.UTF8.GetBytes(JsonUtility.ToJson(playerData)));
    }
}
