using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MainMenuJetMgr : MonoBehaviour
{
    #region Variables
    private JetMovementComponent m_jmc;

    private Dictionary<int, GameObject> m_JetMeshes;
    public Dictionary<int, GameObject> JetMeshes => m_JetMeshes;
    private int m_JetCurID;
    public int JetCurID => m_JetCurID;

    private JetsDatabase JetDatas => GlobalMgr.INSTANCE.m_JetsDatabase;
    private GlobalData GlobalData => GlobalMgr.INSTANCE.m_GlobalData;

    [SerializeField] private Material MainMenuJetMaterial;

    [SerializeField] private CinemachineVirtualCamera m_VCamera;

    private Tweener JetChangeT;
    #endregion

    private void Awake()
    {
        m_jmc = GetComponent<JetMovementComponent>();
        m_JetCurID = -1;
    }

    private void Start()
    {        
        StartCoroutine(RandomJetSway_IEF());
    }

    public void JetsInstantiate_F()
    {
        m_JetMeshes = new Dictionary<int, GameObject>();
        JetsDatabase jdb = GlobalMgr.INSTANCE.m_JetsDatabase;

        foreach(KeyValuePair<int, JetData> kvp in jdb.m_JetDatas)
        {
            GameObject gObj = jdb.MainMenuJetInstantiate_F(kvp.Key);
            MainMenuMaterialAssign_F(gObj);
            m_JetMeshes.Add(kvp.Key, gObj);

            if (kvp.Key == GlobalMgr.INSTANCE.m_GlobalData.JetCur)
            {
                JetCurSet_F(kvp.Key);
            }
            else
                gObj.SetActive(false);            
        }
    }

    private void Update()
    {
        m_jmc.InputVectorAdd_F(new Vector3(0f, 0f, 1f));
    }

    private IEnumerator RandomJetSway_IEF()
    {        
        while(true)
        {
            float time = Random.Range(0f, 0.5f);            
            Vector3 input = new Vector3(Random.Range(-1, 2), Random.Range(-1, 2), 0f);
            //Debug.Log($"<color=yellow>Time = {time}, dir = {input}</color>");
            while (time > 0f)
            {
                m_jmc.InputVectorAdd_F(input);
                time -= Time.deltaTime;
                yield return null;
            }
        }
    }
    
    /// <summary>
    /// Removes old materials from instantiated mesh and assigns the main menu material for the jet.
    /// </summary>
    /// <param name="gObj">The Jet change material for.</param>
    private void MainMenuMaterialAssign_F(GameObject gObj)
    {
        //Getting reference to old material to copy its values and creating main menu material.
        Material materialCur = gObj.GetComponentInChildren<MeshRenderer>().sharedMaterial;
        Material materialToAssign = Instantiate(MainMenuJetMaterial);
        
        //Assigning old material values to the new one.
        materialToAssign.SetTexture("_WireframeTexture", materialCur.GetTexture("_WireframeTexture"));
        materialToAssign.SetFloat("_EmissionStrength", materialCur.GetFloat("_EmissionStrength"));

        //Setting some default values for the new material.
        materialToAssign.SetFloat("_AlphaStep", 0f);
        materialToAssign.SetFloat("_MaterialType", 0f);

        //Assigning new material to all meshrenderers.
        foreach (MeshRenderer mr in gObj.GetComponentsInChildren<MeshRenderer>()) mr.sharedMaterial = materialToAssign;            
    }

    /// <summary>
    /// Changes the Jet currently displayed in menu(doesnt have to be the equipped one)
    /// by setting the old one to inactive and then making a tween for the new one to fade in.
    /// </summary>
    /// <param name="id">Jet ID for the jet to be set as Current Jet.</param>
    public void JetCurSet_F(int id)
    {
        if (id == m_JetCurID) return;

        //Deactivates the current one if present.
        if (m_JetCurID != -1)
        {
            m_JetMeshes[m_JetCurID].SetActive(false);
            m_JetMeshes[m_JetCurID].transform.parent = null;
        }
        
        //Enables the jet with id.
        m_JetMeshes[id].SetActive(true);
        m_JetMeshes[id].transform.parent = transform;
        m_JetMeshes[id].transform.localPosition = Vector3.zero;
        m_JetCurID = id;

        //Sets material type to the wireframe style type if owned or some plain color.
        m_JetMeshes[id].GetComponentInChildren<MeshRenderer>().sharedMaterial.SetFloat("_MaterialType", GlobalData.JetCheckIfOwned_F(id) ? 1 : 0);

        //Ensures old tween for fading is not active and then creates a new one to fade the jet in.
        if (JetChangeT.IsActive() == true) JetChangeT.Kill();
        JetChangeT = DOTween.To(() => 0f, val =>
        {            
            m_JetMeshes[id].GetComponentInChildren<MeshRenderer>().sharedMaterial.SetFloat("_AlphaStep", val);
        }, 1f, 1f);
    }

    /// <summary>
    /// Set current jet in menu to the actual equipped jet.
    /// </summary>
    public void JetCurSetToEquippedJet_F() => JetCurSet_F(GlobalData.JetCur);

    /// <summary>
    /// Call to Blast Off the jet to transition of the the MainGameScene.
    /// </summary>
    public void BlastOff_F()
    {
        m_VCamera.Follow = null;
    }
}
