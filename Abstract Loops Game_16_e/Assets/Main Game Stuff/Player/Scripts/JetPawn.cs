using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JetPawn : Pawn
{
    #region Variables
    private JetMovementComponent jmc;
    private GameObject playerHitboxGObj;

    private Vector3 m_MovementInput;  

    private int m_PlayersLayerMask;
    private int m_BlocksLayerMask;
    private bool m_IsInvincible;
    public bool IsInvincible { get => m_IsInvincible; set => m_IsInvincible = value; }    
    #endregion

    private void Awake()
    {
        m_PlayersLayerMask = LayerMask.NameToLayer("Players");
        m_BlocksLayerMask = LayerMask.NameToLayer("Blocks");
        jmc = GetComponent<JetMovementComponent>();
        playerHitboxGObj = GetComponentInChildren<PlayerHitbox>().gameObject;
        playerHitboxGObj.SetActive(false);
        HealthMaxOut_F();
        HealthOnReachedZero_E += Kill_EF;
    }

    public void Update()
    {
        Move_F(m_MovementInput);
    }

    public override void OnPossess_F(PlayerController playerController)
    {
        base.OnPossess_F(playerController);
        m_MovementInput.z = 1f;
        playerHitboxGObj.SetActive(true);
    }

    public override void OnUnPossess_F()
    {
        m_MovementInput = Vector3.zero;
        jmc.VelocitySetToZero_F();
        playerHitboxGObj.SetActive(false);
    }

    public override void SetupInput_F(Player_InputAction player_InputAction)
    {
        player_InputAction.Player.Move.performed += MoveInput_IEF;
        player_InputAction.Player.Invincibility_Toggle.performed += InvincibilityToggle_IEF;
        //player_InputAction.Player.Move.canceled += MoveInput_EF;
    }

    private void MoveInput_IEF(InputAction.CallbackContext ctx)
    {
        Vector2 value = ctx.ReadValue<Vector2>();

        m_MovementInput.x = value.x;
        m_MovementInput.y = value.y;
    }

    private void InvincibilityToggle_IEF(InputAction.CallbackContext ctx)
    {
        Debug.Log("Invincibility Button Pressed");

        if (m_IsInvincible == false) InvincibilityStart_F();
        else InvincibilityStop_F();
    }
    
    public void Move_F(Vector3 input)
    {
        jmc.InputVectorAdd_F(input);
    }

    public void InvincibilityStart_F()
    {
        Physics.IgnoreLayerCollision(m_PlayersLayerMask, m_BlocksLayerMask, true);
        m_IsInvincible = true;
    }

    public void InvincibilityStop_F()
    {
        Physics.IgnoreLayerCollision(m_PlayersLayerMask, m_BlocksLayerMask, false);
        m_IsInvincible = false;
    }

    private void Kill_EF()
    {
        Debug.Log("Kill_EF() called");
        if(IsPossessed) m_PlayerController.UnPossess_F();
        OnDeath_E?.Invoke();

        gameObject.SetActive(false);
        MainGameReferences.s_Instance.playerExplosionPSMgr.DoBurst_F(transform.position);
    }

    public override void HealthReduce_F(int damage)
    {
        m_Health -= damage;
        m_Health = Mathf.Clamp(m_Health, 0, 100);

        if (m_Health == 0)
            HealthOnReachedZero_E?.Invoke();
    }

    public void Revive_F()
    {
        gameObject.SetActive(true);
        HealthMaxOut_F();
        StartCoroutine(Revive_IEF());
    }

    private IEnumerator Revive_IEF()
    {
        InvincibilityStart_F();
        yield return new WaitForSeconds(5f);
        
        InvincibilityStop_F();
    }
}

public abstract class Pawn : MonoBehaviour
{
    protected PlayerController m_PlayerController;

    public bool IsPossessed { get => m_PlayerController != null; }

    protected int m_Health;
    public int Health { get => m_Health; }
    public System.Action HealthOnReachedZero_E;
    public System.Action OnDeath_E;

    /// <summary>
    /// Executes when Pawn is being possessed by a PlayerController 
    /// </summary>
    /// <param name="playerController"></param>
    public virtual void OnPossess_F(PlayerController playerController) 
    {
        m_PlayerController = playerController;
    }

    /// <summary>
    /// Executes when Pawn stops being possessed by a PlayerController. 
    /// </summary>
    /// <param name="playerController"></param>
    public virtual void OnUnPossess_F() { }

    /// <summary>
    /// Use to add input bindings to Pawn functions when being possessed so that Pawn can be controlled by player input.
    /// </summary>
    /// <param name="player_InputAction"></param>
    public virtual void SetupInput_F(Player_InputAction player_InputAction) { }

    /// <summary>
    /// Use to lower Pawn's health. When Pawn health reaches zero OnHealthZero_E event is called to notify subscribers that Pawn health is outu.
    /// </summary>
    /// <param name="damage"></param>
    public virtual void HealthReduce_F(int damage) { }

    /// <summary>
    /// Makes players health back to full.
    /// </summary>
    public void HealthMaxOut_F() => m_Health = 100;
}