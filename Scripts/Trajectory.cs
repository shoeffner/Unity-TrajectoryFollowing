using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Trajectory : MonoBehaviour
{
    [Tooltip("The trajectory points. Always includes this GameObject's transform and optionally a goal transform.")]
    [SerializeField]
    public List<Vector3> trajectory = new List<Vector3>(new Vector3[] {
        new Vector3(1f, 0f, 0f),
        new Vector3(2f, 0f, 0f),
        new Vector3(3f, 0f, 0f),
        new Vector3(4f, 0f, 0f),
    });

    [Tooltip("The trajectory goal - if empty, the last point of the trajectory is the goal.")]
    public Transform goal;

    public Vector3 GetAt(float offset) {
        return new Vector3(offset * 10, 0, 0);
    }


    void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        foreach(Vector3 p in trajectory) {
            Gizmos.DrawSphere(p, 0.2f);
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        foreach(Vector3 p in trajectory) {
            Gizmos.DrawSphere(p, 0.2f);
        }
    }
}
