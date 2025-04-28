using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("Elements")]
    public GameObject main;
    public GameObject loading;
    public GameObject popupError;
    public GameObject popupTimeout;
    public GameObject popupDie;

    public void ShowLoading()
    {
        loading.SetActive(true);
    }

    public void HideLoading()
    {
        loading.SetActive(false);
    }

    public void ShowPopupError()
    {
        popupError.SetActive(true);
    }

    public void ShowPopupTimeout()
    {
        popupTimeout.SetActive(true);
    }

    public void ShowPopupDie()
    {
        popupDie.SetActive(true);
    }

    public void HidePopups()
    {
        popupError.SetActive(false);
        popupTimeout.SetActive(false);
        popupDie.SetActive(false);
    }

    public void HideMain()
    {
        main.SetActive(false);
    }

    public void ShowMain()
    {
        main.SetActive(true);
    }
}
