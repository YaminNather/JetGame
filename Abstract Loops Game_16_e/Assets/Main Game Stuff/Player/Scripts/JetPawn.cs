using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class JetPawn : Pawn
{
    #region Variables
    private JetMovementComponent m_jmc;
    private JetAudioMgr m_JetAudioMgr;
    private GameObject playerHitboxGObj;

    private Vector3 m_MovementInput;  

    private int m_PlayersLayerMask;
    private int m_BlocksLayerMask;
    private bool m_IsInvincible;
    public bool IsInvincible { get => m_IsInvincible; set => m_IsInvincible = value; }

    private Material m_DefMat;
    [SerializeField] private Material m_InvincibilityMat;
    #endregion

    private void Awake()
    {
        m_PlayersLayerMask = LayerMask.NameToLayer("Players");
        m_BlocksLayerMask = LayerMask.NameToLayer("Blocks");
        m_jmc = GetComponent<JetMovementComponent>();
        m_JetAudioMgr = GetComponentInChildren<JetAudioMgr>();
        playerHitboxGObj = GetComponentInChildren<PlayerHitbox>().gameObject;
        playerHitboxGObj.SetActive(false);
        HealthMaxOut_F();
        HealthOnReachedZero_E += Kill_EF;
        SetupMaterialFields_F();
    }

    private void SetupMaterialFields_F()
    {
        m_DefMat = GetComponentInChildren<MeshRenderer>().sharedMaterial;
        if (m_InvincibilityMat != null)
        {
            m_InvincibilityMat = Instantiate(m_InvincibilityMat);
            m_InvincibilityMat.SetTexture("_WireframeTexture", m_DefMat.GetTexture("_WireframeTexture"));
            m_InvincibilityMat.SetFloat("_EmissionStrength", m_DefMat.GetFloat("_EmissionStrength"));
        }
    }

    public void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (m_IsInvincible == false)
            {
                //Debug.Log("Called InvincibilityStart_F()");
                InvincibilityStart_F();
            }
            else 
            {
                //Debug.Log("Called InvincibilityStop_F()");
                InvincibilityStop_F();
            }
        }

        Move_F(m_MovementInput);
    }

#if UNITY_EDITOR
    public void LateUpdate()
    {
        if (Application.isPlaying == false || IsPossessed == false) return;

        UnityEditor.SceneView view = UnityEditor.SceneView.lastActiveSceneView;
        if (view != null)
        {
            view.pivot = transform.position.With(x: 0f, y: 0f);
            //view.LookAtDirect(transform.position.With(x:0f, y:0f), Quaternion.Euler(45f, 45f, 0f));
        }
    }
#endif

    public override void OnPossess_F(PlayerController playerController)
    {
        base.OnPossess_F(playerController);
        m_MovementInput.z = 1f;
        playerHitboxGObj.SetActive(true);
    }

    public override void OnUnPossess_F()
    {
        m_MovementInput = Vector3.zero;
        m_jmc.VelocitySetToZero_F();
        playerHitboxGObj.SetActive(false);
    }

    public override void InputSetup_F(Player_InputAction player_InputAction)
    {
#if UNITY_EDITOR
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
#endif
        player_InputAction.Player.Move.performed += MoveInput_IEF;
        player_InputAction.Player.MoveWithMouse.performed += MoveWithMouse_IEF;
        player_InputAction.Player.Invincibility_Toggle.performed += InvincibilityToggle_IEF;
        //player_InputAction.Player.Move.canceled += MoveInput_EF;
    }

    public override void InputDesetup_F(Player_InputAction player_InputAction)
    {
#if UNITY_EDITOR
        //Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
#endif
        player_InputAction.Player.Move.performed -= MoveInput_IEF;
        player_InputAction.Player.MoveWithMouse.performed -= MoveWithMouse_IEF;
        player_InputAction.Player.Invincibility_Toggle.performed -= InvincibilityToggle_IEF;
    }

    private void MoveInput_IEF(InputAction.CallbackContext ctx)
    {
        Vector2 value = ctx.ReadValue<Vector2>();

        m_MovementInput.x = value.x;
        m_MovementInput.y = value.y;
    }

    private void MoveWithMouse_IEF(InputAction.CallbackContext ctx)
    {        
        //if (Mouse.current.leftButton.IsPressed() == false)
        //    return;

        Vector2 value = ctx.ReadValue<Vector2>();
        float maxPixelDist =(45.0f / 1080.0f) * Screen.width; // This is the max distance(in px's) for drag to get full speed. Changing this changes sensitivity.
        value.x = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-maxPixelDist, maxPixelDist, value.x));
        value.y = Mathf.Lerp(-1f, 1f, Mathf.InverseLerp(-maxPixelDist, maxPixelDist, value.y));

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
        m_jmc.InputVectorAdd_F(input);
    }

    public void InvincibilityStart_F()
    {
        Physics.IgnoreLayerCollision(m_PlayersLayerMask, m_BlocksLayerMask, true);
        m_IsInvincible = true;

        MaterialSwitchToInvincibilityMat_F();
    }
    
    private void MaterialSwitchToInvincibilityMat_F()
    {
        if (m_InvincibilityMat == null)
            return;

        MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
        mr.sharedMaterial = m_InvincibilityMat;
    }

    public void InvincibilityStop_F()
    {
        Physics.IgnoreLayerCollision(m_PlayersLayerMask, m_BlocksLayerMask, false);
        m_IsInvincible = false;
        MaterialSwitchToDefaultMat_F();
    }
    
    private void MaterialSwitchToDefaultMat_F()
    {
        if (m_InvincibilityMat == null)
            return;

        MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
        mr.material = m_DefMat;        
    }

    private void Kill_EF()
    {
        //Debug.Log("Kill_EF() called");
        m_JetAudioMgr.PlayAudioWithGlobalAudioSource_F(JetAudioMgr.AudioClipsEN.Explode);
        GlobalMgr.INSTANCE.m_BackgroundMusicMgr.FadeOut_F();
        if(IsPossessed) m_PlayerController.UnPossess_F();
        OnDeath_E?.Invoke();

        gameObject.SetActive(false);
        MainGameReferences.INSTANCE.playerExplosionPSMgr.DoBurst_F(transform.position);
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

    public void AudioPlay_F(JetAudioMgr.AudioClipsEN audioClip)
    {
        m_JetAudioMgr.PlayAudio_F(JetAudioMgr.AudioClipsEN.ZoomingPastBlock);
    }
}

#if UNITY_EDITOR
public partial class JetPawn : Pawn
{

    private const string JETBASEPREFABPATH = "Assets/Main Game Stuff/Player/Players/Base Prefab/PlayerBase_Prefab.prefab";

    [MenuItem("Assets/Create/Player Stuff/Create Base Player_Prefab Variant", true)]
    private static bool NewJetCreateValidator_F()
    {
        //Check if a folder is selected.
        foreach(UnityEngine.Object obj in Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Assets))
        {
            if (Path.HasExtension(AssetDatabase.GetAssetPath(obj)) == true) return false;            
        }

        //Check if the PlayerBase_Prefab exists.
        GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(JETBASEPREFABPATH);
        if (asset == null && asset.TryGetComponent(out JetPawn _) == false) return false;
        
        return true;
    }

    [MenuItem("Assets/Create/Player Stuff/Create Base Player_Prefab Variant")]
    private static void NewJetCreate_F()
    {
        //Loading and instantiating a temporary object from the base prefab.
        GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(JETBASEPREFABPATH);
        GameObject instantiatedObj = PrefabUtility.InstantiatePrefab(asset) as GameObject;        
        instantiatedObj.transform.name = "Player Base_PrefabVariant";
        
        //Finding the path where the variant should be created.
        string path = "Assets";
        foreach(UnityEngine.Object obj in Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if(!string.IsNullOrEmpty(path)) break;
        }
        path += $"/{instantiatedObj.transform.name}.prefab";
        Debug.Log($"<color=yellow>path = {path}</color>");

        //Saving the instantiated temporary object in the path and removing the temporary object.
        PrefabUtility.SaveAsPrefabAsset(instantiatedObj, path);        
        //ProjectWindowUtil.CreateAsset(instantiatedObj, path);
        DestroyImmediate(instantiatedObj);        
    }
}
#endif

public abstract class Pawn : MonoBehaviour
{
    #region Variables
    protected PlayerController m_PlayerController;

    public bool IsPossessed => m_PlayerController != null;

    protected int m_Health;
    public int Health => m_Health;
    public System.Action HealthOnReachedZero_E;
    public System.Action OnDeath_E;
    #endregion

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
    public virtual void InputSetup_F(Player_InputAction player_InputAction) { }

    /// <summary>
    /// Use to remove input bindings to Pawn functions when being possessed so that Pawn is no longer controlled by player input.
    /// </summary>
    /// <param name="player_InputAction"></param>
    public virtual void InputDesetup_F(Player_InputAction player_InputAction) { }

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