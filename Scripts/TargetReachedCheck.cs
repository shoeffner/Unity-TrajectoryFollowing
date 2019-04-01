using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetReachedCheck : MonoBehaviour
{
    private enum State {
        DEFAULT, GOALSTATE, FINISHED
    }
    public GameObject goalFor;
    [Range(0.0f, 1.0f)] public float ratioOfCollidersInTargetForSuccess = 0.5f;
    [Range(0.0f, 10.0f)] public float validAfterSecondsAtGoal = 1;
    private HashSet<Collider> goalColliders;
    private HashSet<Collider> enteredGoalColliders = new HashSet<Collider>();
    private float timeInGoalState = 0;
    private State state = State.DEFAULT;

    void Start()
    {
        goalColliders = new HashSet<Collider>(goalFor.GetComponentsInChildren<Collider>());
    }

    void Update()
    {
        switch (this.state) {
            case State.DEFAULT:
                timeInGoalState = 0;
                if (this.isInGoalState()) {
                    this.state = State.GOALSTATE;
                }
                return;
            case State.GOALSTATE:
                timeInGoalState += Time.deltaTime;
                if (!this.isInGoalState()) {
                    this.state = State.DEFAULT;
                } else if (timeInGoalState > validAfterSecondsAtGoal) {
                    this.state = State.FINISHED;
                    Debug.Log("Goal reached.");
                }
                return;
            case State.FINISHED:
                return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (goalColliders.Contains(other) && !enteredGoalColliders.Contains(other)) {
            enteredGoalColliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        enteredGoalColliders.Remove(other);
    }

    private bool isInGoalState() {
        return enteredGoalColliders.Count > goalColliders.Count * ratioOfCollidersInTargetForSuccess;
    }
}
