using UnityEngine;

[RequireComponent(typeof(Trajectory))]
public class MoveAlongTrajectory : MonoBehaviour
{
    [Tooltip("The trajectory to move along.")]
    public Trajectory trajectory;

    [Range(0.01f, 10f), Tooltip("Movement speed.")]
    public float speed = 1.0f;
    public float step_size = 0.1f;
    private float offset = 0f;

    [Tooltip("Start moving when the game starts, otherwise move on pressing space.")]
    public bool startAutomatically = false;

    public void Awake() {
        trajectory = this.GetComponent<Trajectory>();
    }

    public void Start() {
    }

    public void Update() {
        offset += Time.deltaTime * step_size;
        if (offset > 1) {
            return;
        }
        transform.position = trajectory.GetAt(offset);
    }
}
