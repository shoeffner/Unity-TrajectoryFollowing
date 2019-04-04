using UnityEngine;

[RequireComponent(typeof(Trajectory))]
public class MoveAlongTrajectory : MonoBehaviour
{
    [Tooltip("The trajectory to move along.")]
    public Trajectory trajectory;

    [Range(0.01f, 10f), Tooltip("Movement speed in meters (i.e. units) per second.")]
    public float speed = 1.0f;

    [Tooltip("Start moving when the game starts, otherwise move on pressing space.")]
    public bool startAutomatically = true;

    private float currentOffset;
    private float lookAhead = 0.1f;
    private Vector3 currentTarget;
    private bool moving = false;
    private bool gravityCache = true;

    public void Awake() {
        if (trajectory == null) {
            trajectory = this.GetComponent<Trajectory>();
        }
        currentOffset = lookAhead;
    }

    public void Start() {
        currentTarget = trajectory.GetAt(currentOffset);
        if (startAutomatically) {
            StartMoving();
        }
    }

    public void StartMoving() {
        Debug.Log("Following trajectory, disabling gravity.");
        Rigidbody r = GetComponent<Rigidbody>();
        if (r != null) {
            gravityCache = r.useGravity;
            r.useGravity = false;
        }
        moving = true;
    }

    public void StopMoving() {
        Debug.Log("Stopping following trajectory, resetting gravity.");
        moving = false;
        Rigidbody r = GetComponent<Rigidbody>();
        if (r != null) {
            r.useGravity = gravityCache;
        }
    }

    public void Update() {
        if (moving) {
            float travelDistance = Time.deltaTime * speed;

            float distance = Vector3.Distance(transform.position, currentTarget);
            while (travelDistance >= distance) {
                currentOffset += lookAhead;
                currentTarget = trajectory.GetAt(currentOffset);
                distance = Vector3.Distance(transform.position, currentTarget);
                if (trajectory.IsBehind(currentOffset) || trajectory.IsBefore(currentOffset)) {
                    StopMoving();
                    break;
                }
            }

            transform.position += (currentTarget - transform.position).normalized * travelDistance;
        }
    }
}