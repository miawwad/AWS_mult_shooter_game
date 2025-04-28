using UnityEngine;

public class TargetMoverVertical : MonoBehaviour
{
    public float moveDistance = 2f;  // How far up/down it moves
    public float moveSpeed = 2f;     // How fast it moves

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = startPos + new Vector3(0, y, 0);
    }
}
