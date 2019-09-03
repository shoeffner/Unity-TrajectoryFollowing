using UnityEngine;
using UnityEditor;

namespace TrajectoryFollowing {

[System.Serializable, ExecuteInEditMode]
public class TrajectoryPoint : MonoBehaviour
{

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(gameObject.transform.position, 0.04f);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(gameObject.transform.position, 0.04f);
    }
}

}