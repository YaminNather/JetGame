using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;


public class JetDisplayMgr : MonoBehaviour
{
    #region Variables
    private GlobalData gd;
    private Animator m_Animator;

    private Dictionary<string, JetData> m_Jets;
    public Dictionary<string, JetData> Jets => m_Jets;
    private JetData m_JetCur;
    public JetData JetCur => m_JetCur;

    [SerializeField] private Transform VolumetricLight_Trans;
    [SerializeField]private Transform JetHolder_Trans;    
    #endregion

    //private void Awake()
    //{
    //    gd = GlobalMgr.s_Instance.globalData;
    //    m_Animator = GetComponent<Animator>();
    //    Destroy(JetHolder_Trans.GetChild(0).transform.gameObject);
    //    //SetupInitialTransforms_F();
    //}

    //private void SetupInitialTransforms_F()
    //{
    //    VolumetricLight_Trans.localScale = new Vector3(1f, 0f, 1f);
    //    JetHolder_Trans.transform.localScale = Vector3.zero;
    //    JetHolder_Trans.transform.position = Vector3.zero;
    //}
    
    //public void Init_F()
    //{
    //    StartCoroutine(Init_IEF());
    //}

    //private IEnumerator Init_IEF()
    //{
    //    yield return StartCoroutine(LoadAllModels_IEF());
    //    //Debug.Log("<color=yellow>All Display Jets Loaded</color>");
    //    JetObjCurMake_F(GlobalMgr.s_Instance.globalData.JetCur);
    //    Entry_F(true);
    //}
    
    //private IEnumerator LoadAllModels_IEF()
    //{
    //    m_Jets = new Dictionary<string, JetData>();

    //    IList<IResourceLocation> rls = null;
    //    AsyncOperationHandle<IList<IResourceLocation>> aoh_0 = Addressables.LoadResourceLocationsAsync("Player Display");
    //    yield return aoh_0;
    //    rls = aoh_0.Result;

    //    foreach (IResourceLocation rl in rls)
    //    {
    //        AsyncOperationHandle<GameObject> aoh_1 = Addressables.InstantiateAsync(rl);
    //        yield return aoh_1;
    //        GameObject gObj = aoh_1.Result;
    //        gObj.SetActive(false);

    //        JetData jetData = gObj.GetComponent<JetData>();
    //        gObj.transform.name = jetData.ID;
    //        m_Jets.Add(jetData.name, jetData);
    //    }
    //}

    //private void JetObjCurMake_F(string jetName)
    //{
    //    JetObjCurUnMake_F();

    //    JetData jet = m_Jets[jetName];
    //    jet.transform.parent = JetHolder_Trans;
    //    jet.transform.localPosition = Vector3.zero;
    //    jet.transform.localRotation = Quaternion.identity;
    //    jet.transform.localScale = Vector3.one;
    //    jet.gameObject.SetActive(true);
    //    m_JetCur = jet;
    //}

    //private void JetObjCurUnMake_F()
    //{
    //    if (m_JetCur == null) return;

    //    //Debug.Log($"<color=yellow>Unmaking {m_JetCur.transform.name} from being JetCur</color>");
    //    m_JetCur.transform.parent = null;
    //    m_JetCur.gameObject.SetActive(false);
    //    m_JetCur = null;
    //}

    //private void Update()
    //{
    //    //Keyboard kb = InputSystem.GetDevice<Keyboard>();
    //    //if(kb.digit7Key.wasPressedThisFrame)
    //    //{
    //    //    JetChange_F();
    //    //}
    //}

    //private void Entry_F(bool isBought)
    //{
    //    if (!isBought)
    //        m_Animator.SetTrigger("Entry_Trigger");
    //    else
    //        m_Animator.SetTrigger("BoughtJetEntry_Trigger");
    //}              
    
    //public void JetChange_F(string jetName)
    //{        
    //    JetObjCurMake_F(jetName);

    //    Entry_F(gd.JetCheckIfOwned_F(jetName));
    //}

    //public void JetPurchased_F()
    //{
    //    Entry_F(true);
    //}
}
