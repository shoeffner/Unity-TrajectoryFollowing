using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Trajectory))]
public class MoveAlongTrajectory : MonoBehaviour
{
    [Tooltip("The trajectory to move along.")]
    public Trajectory trajectory;

    [Range(0.01f, 10.0f), Tooltip("Movement speed in meters (i.e. units) per second.")]
    public float speed = 1.0f;

    [Range(0.0f, 10.0f), Tooltip("Start after this time (in seconds).")]
    public float startAfter = 0.0f;

    [Tooltip("Use transform updates to avoid physics calculations.")]
    public bool ignorePhysics = false;

    private float currentOffset;
    private float lookAhead = 0.1f;
    private Vector3 currentTarget;
    private bool moving = false;

    public void Awake() {
        // Get the trajectory if not explicitly set.
        if (trajectory == null) {
            trajectory = this.GetComponent<Trajectory>();
            if (trajectory == null) {
                Debug.LogError("Trajectory required to move along.");
            }
        }

        // Initialize look-ahead and first target point.
        currentOffset = lookAhead;
        currentTarget = trajectory.GetAt(currentOffset);

        // Initialize Rigidbody if present and physics shall be used.
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (ignorePhysics == false) {
            if (rigidbody == null) {
                ignorePhysics = true;
                Debug.LogWarning("No RigidBody present! Ignoring physics.");
            } else if (!rigidbody.isKinematic) {
                Debug.LogWarning("Rigidbody must be kinematic! Making it kinematik for now.");
                rigidbody.isKinematic = true;
            }
        }
    }

    public void Start() {
        StartCoroutine(DelayedStart(startAfter));
    }

    /// <summary>
    /// If the GameObject is "moving", that is moving == true, this updates the position.
    /// First, the travel time and speed determine the distance traveled along the trajectory.
    /// If the next point (that is the look ahead offset on the trajectory) is further away than
    /// the travel distance, the object is moved towards that position. Otherwise, a point which is
    /// away far enough is retrieved and the object is moved towards it.
    /// If the goal is reached, i.e. within travel distance, movement is stopped and the goal set
    /// as the new position.
    /// </summary>
    public void FixedUpdate() {
        if (moving) {
            float travelDistance = Time.fixedDeltaTime * speed;
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

            // position + direction * distance
            Vector3 newPosition = transform.position + (currentTarget - transform.position).normalized * travelDistance;
            if (ignorePhysics) {
                transform.position = newPosition;
            } else {
                Rigidbody r = GetComponent<Rigidbody>();
                r.MovePosition(newPosition);
            }
        }
    }

    IEnumerator DelayedStart(float timeToWait) {
        yield return new WaitForSeconds(timeToWait);
        StartMoving();
    }

    public void StartMoving() {
        Debug.Log("Starting trajectory following.");
        moving = true;
    }

    public void StopMoving() {
        Debug.Log("Stopping trajectory following.");
        moving = false;
    }
}