using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

/// <summary>
/// This class is PlayerController made specifically for jets.
/// </summary>
public class JetPlayerController : PlayerController
{    
    [SerializeField] private PlayerCamera_Mgr PlayerCamera;                  

    /// <summary>
    /// Possess player. Possessed Player is the player that is being controlled by the controller, and also sets up the camera.
    /// </summary>
    /// <param name="pawn"></param>
    public override void Possess_F(Pawn pawn)
    {
        base.Possess_F(pawn);
        PlayerCamera.OnPossess_F(pawn);
    }

    /// <summary>
    /// Unpossesses Player. Player stops being controlled by PlayerController, and also desetups the camera.
    /// </summary>
    public override void UnPossess_F()
    {
        base.UnPossess_F();
        PlayerCamera.OnUnPossess_F();
    }
}

/// <summary>
/// This object controls the player by possessing it.
/// </summary>
public abstract class PlayerController : MonoBehaviour
{
    #region Variables
    protected Pawn m_PossessedPawn; // Field storing currently possessed Player.
    public Pawn PossessedPawn { get => m_PossessedPawn; } // Property for m_Possessed Player.
    protected bool IsPossessed { get => m_PossessedPawn != null; } // Property to check if playercontroller is possessing a player.
    private Player_InputAction player_InputAction;
    #endregion

    protected virtual void Awake()
    {       
        player_InputAction = new Player_InputAction();
    }

    public virtual void Possess_F(Pawn pawn)
    {
        if (pawn == null)
            return;

        if (IsPossessed)
            UnPossess_F();

        m_PossessedPawn = pawn;
        pawn.OnPossess_F(this);

        player_InputAction.Enable();
        pawn.SetupInput_F(player_InputAction);
    }

    public virtual void UnPossess_F()
    {
        if (!IsPossessed) return;

        player_InputAction.Disable();
        PossessedPawn.OnUnPossess_F();
        m_PossessedPawn = null;
    }
}
