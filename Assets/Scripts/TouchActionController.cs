using UnityEngine;
using UnityEngine.EventSystems;

public class TouchActionController : MonoBehaviour, IPointerDownHandler
{
    public PlayerActions playerActions;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        playerActions.OnPunch();
    }
}
