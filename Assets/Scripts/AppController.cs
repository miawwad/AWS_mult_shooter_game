using UnityEngine;

public class AppController : MonoBehaviour
{
    public ServerConnect server;
    public UIController uIController;

    private void Start()
    {
        Application.runInBackground = true;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Invoke("DisconnectFromServer", Utils.timeOutAfter);
        }
        else
        {
            CancelInvoke("DisconnectFromServer");
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            Invoke("DisconnectFromServer", Utils.timeOutAfter);
        }
        else
        {
            CancelInvoke("DisconnectFromServer");
        }
    }

    void OnApplicationQuit()
    {
        server.DisconnectFromServer();
    }

    private void DisconnectFromServer()
    {
        server.DisconnectFromServer();
        uIController.ShowPopupTimeout();
    }
}
