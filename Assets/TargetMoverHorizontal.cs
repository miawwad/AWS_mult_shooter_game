using UnityEngine;

public class TargetMoverHorizontal : MonoBehaviour
{
    public float moveDistance = 2f;  // How far left/right it moves
    public float moveSpeed = 2f;     // How fast it moves

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = startPos + new Vector3(x, 0, 0);
    }
}
