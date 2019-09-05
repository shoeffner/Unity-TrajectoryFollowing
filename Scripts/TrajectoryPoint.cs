using UnityEngine;
using UnityEditor;

namespace TrajectoryFollowing {

[ExecuteAlways]
public class TrajectoryPoint : MonoBehaviour
{
    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(gameObject.transform.position, 0.01f);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(gameObject.transform.position, 0.015f);
    }
}

}