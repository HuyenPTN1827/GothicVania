using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine {
    public EnemyState CurrentState { get; set; }

    public void Initialize(EnemyState state) {
        //Debug.Log("Init: " + state.name);
        CurrentState = state;
        CurrentState.EnterState();
    }

    public void ChangeState(EnemyState state) {
        //Debug.Log("Enter: " + state.name);
        CurrentState.ExitState();
        CurrentState = state;
        CurrentState.EnterState();
    }


}
