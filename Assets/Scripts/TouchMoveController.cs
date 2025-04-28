using UnityEngine;
using UnityEngine.EventSystems;

public class TouchMoveController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public PlayerActions playerActions;

    public RectTransform rectTransform;
    public RectTransform circleBig;
    public RectTransform circleSmall;

    private Vector2 initPos = Vector2.zero;

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, pointerEventData.position, pointerEventData.pressEventCamera, out initPos);

        circleBig.anchoredPosition = initPos;
        circleSmall.anchoredPosition = initPos;

        circleBig.gameObject.SetActive(true);
        circleSmall.gameObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        circleBig.gameObject.SetActive(false);
        circleSmall.gameObject.SetActive(false);

        playerActions.OnStopMove();
        playerActions.OnStopRotate();
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, pointerEventData.position, pointerEventData.pressEventCamera, out position);

        Vector2 diff = position - initPos;
        float radius = circleBig.sizeDelta.x / 2;

        if (diff.magnitude > 0.01f)
        {
            float newPosX = initPos.x + diff.x * radius / diff.magnitude;
            float newPosY = initPos.y + diff.y * radius / diff.magnitude;

            circleSmall.anchoredPosition = new Vector2(newPosX, newPosY);

            if (Mathf.Abs(diff.x) < 5f)
            {
                playerActions.OnStopRotate();
            }
            else if (diff.x > 0)
            {
                playerActions.OnRotateRight();
            }
            else
            {
                playerActions.OnRotateLeft();
            }

            if (diff.y > 0)
            {
                playerActions.OnRun();
            }
            else
            {
                playerActions.OnWalkBack();
            }
        }        
    }
}
