using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this is the state class. you can inherit from it to create your own states
/// </summary>
public class State
{
    public FSM parentFSM;

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void CalledInUpdate() { }
    public virtual void CalledInFixedUpdate() { }

}

/// <summary>
/// this is the state machine component
/// </summary>
public class FSM 
{
    protected Dictionary<int, State> m_states = new Dictionary<int, State>();
    protected State m_currentState;

    public FSM(State firstState)
    {
        SetCurrentState(firstState);
    }

    public void Add(int key, State state)
    {
        m_states.Add(key, state);
        state.parentFSM = this;
    }

    public State GetState(int key)
    {
        return m_states[key];
    }

    public void SetCurrentState(State state)
    {
        state.parentFSM = this;

        if (m_currentState != null)
        {
            m_currentState.Exit();
        }

        m_currentState = state;

        if (m_currentState != null)
        {
            m_currentState.Enter();
        }
    }

    public void CalledInUpdate()
    {
        if (m_currentState != null)
        {
            m_currentState.CalledInUpdate();
        }
    }

    public void CalledInFixedUpdate()
    {
        if (m_currentState != null)
        {
            m_currentState.CalledInFixedUpdate();
        }
    }
}
