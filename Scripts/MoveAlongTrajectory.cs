using UnityEngine;

[RequireComponent(typeof(Trajectory))]
public class MoveAlongTrajectory : MonoBehaviour
{
    [Tooltip("The trajectory to move along.")]
    public Trajectory trajectory;

    [Range(0.01f, 10f), Tooltip("Movement speed.")]
    public float speed = 1.0f;

    [Tooltip("Start moving when the game starts, otherwise move on pressing space.")]
    public bool startAutomatically = true;

    private float offset;
    private bool moving;

    public void Awake() {
        if (trajectory == null) {
            trajectory = this.GetComponent<Trajectory>();
        }
        offset = 0.0f;
    }

    public void Start() {
        if (startAutomatically) {
            StartMoving();
        }
    }

    public void StartMoving() {
        moving = true;
    }

    public void StopMoving() {
        moving = false;
    }

    public void Update() {
        if (moving) {
            offset += Time.deltaTime * speed;
            transform.position = trajectory.GetAt(offset);
        }
    }
}
