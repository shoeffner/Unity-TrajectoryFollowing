using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Trajectory))]
public class MoveAlongTrajectory : MonoBehaviour
{
    [Tooltip("The trajectory to move along.")]
    public Trajectory trajectory;

    [Range(-10.0f, 10.0f), Tooltip("Movement speed in meters (i.e. units) per second.")]
    public float speed = 0.0f;

    [Tooltip("Start automatically.")]
    public bool autostart = false;

    [Range(0.0f, 10.0f), Tooltip("Start after this time (in seconds).")]
    public float startAfter = 0.0f;
    private float delayedStartSpeed = 0.0f;


    [Tooltip("Use transform updates to avoid physics calculations.")]
    public bool ignorePhysics = false;

    [Tooltip("Input to start and stop movement.")]
    public List<string> enableMovementWith = new List<string>(){"Jump"};

    private float currentOffset;
    private float currentDirection = 1.0f;
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
    }

    public void Start() {
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

        if (autostart) {
            delayedStartSpeed = speed;
            StartCoroutine(DelayedStart(startAfter));
        }
    }

    public void Update() {
        foreach(string button in enableMovementWith) {
            if (Input.GetButtonDown(button)) {
                moving = !moving;
            }
        }
    }

    /// <summary>
    /// First, the travel time and speed determine the distance traveled along the trajectory.
    /// If the next point (that is the look ahead offset on the trajectory) is further away than
    /// the travel distance, the object is moved towards that position. Otherwise, a point which is
    /// away far enough is retrieved and the object is moved towards it.
    /// If the goal is reached, i.e. within travel distance, movement is stopped and the goal set
    /// as the new position.
    /// </summary>
    public void FixedUpdate() {
        if (moving) {
            if (Mathf.Sign(speed) != Mathf.Sign(currentDirection)) {
                currentDirection = Mathf.Sign(speed);
                currentOffset += lookAhead * Mathf.Sign(speed);
                currentTarget = trajectory.GetAt(currentOffset);
            }

            float travelDistance = Time.fixedDeltaTime * Mathf.Abs(speed);
            float distance = Vector3.Distance(transform.position, currentTarget);
            while (travelDistance >= distance) {
                currentOffset += lookAhead * Mathf.Sign(speed);
                currentTarget = trajectory.GetAt(currentOffset);
                distance = Vector3.Distance(transform.position, currentTarget);
                if (trajectory.IsBehind(currentOffset)) {
                    currentOffset = Mathf.Round(currentOffset) + lookAhead;
                    currentTarget = trajectory.GetEnd();
                    moving = false;
                    break;
                } else if (trajectory.IsBefore(currentOffset)) {
                    currentOffset = -lookAhead;
                    currentTarget = trajectory.GetStart();
                    moving = false;
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
        speed = delayedStartSpeed;
        moving = true;
    }
}