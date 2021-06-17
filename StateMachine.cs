using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DexCode
{
    public class StateMachine : MonoBehaviour
    {
        public State CurrentState { get; private set; }
        public void Initilize(State state)
        {
            CurrentState = state;
            state.Enter();
        }

        public void ChangeState(State state)
        {
            CurrentState.Exit();
            CurrentState = state;
            state.Enter();
        }

    }
}
