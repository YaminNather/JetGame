using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerControllerState : MonoBehaviour
{
    /// <summary>
    /// Reference to PlayerController.
    /// </summary>
    protected JetPlayerController m_PlayerController;

    [SerializeField] protected PlayerControllerStates m_StateName; //State Name
    public PlayerControllerStates Name => m_StateName;

    /// <summary>
    /// Function executed on Entering State.
    /// </summary>
    /// <param name="playerController"></param>
    /// <param name="stateFrom"></param>
    public virtual void OnEnter_F(JetPlayerController playerController, PlayerControllerState stateFrom)
    {
        m_PlayerController = playerController;
    }

    /// <summary>
    /// Function executed every frame while in State.
    /// </summary>
    public virtual void OnUpdate_F()
    {

    }

    /// <summary>
    /// Function executed while leaving State.
    /// </summary>
    /// <param name="stateTo"></param>
    public virtual void OnExit_F(PlayerControllerState stateTo)
    {

    }
}

/// <summary>
/// Enum of states for PlayerController
/// </summary>
public enum PlayerControllerStates
{
    Idle, Possessed
}
