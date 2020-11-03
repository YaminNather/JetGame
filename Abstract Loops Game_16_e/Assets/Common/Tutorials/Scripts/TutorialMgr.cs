using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using TMPro;
using UnityEditor;
#endif

public partial class TutorialMgr : MonoBehaviour
{
    [SerializeField] private GameObject[] m_SlideGObjs;
    private int m_Index;

    private void Start()
    {
        if (m_SlideGObjs.Length == 0)
        {
            Destroy(gameObject);
            return;
        }

        GetComponentInChildren<Button>(true).onClick.AddListener(SlideToNext_F);

    }

    private void OnEnable()
    {
        m_Index = 0;
        m_SlideGObjs[0].SetActive(true);
        for(int i = 1; i < m_SlideGObjs.Length; i++) m_SlideGObjs[i].SetActive(false);
    }

    private void SlideToNext_F()
    {
        m_SlideGObjs[m_Index].SetActive(false);
        if (m_Index != m_SlideGObjs.Length - 1)
        {
            m_Index++;
            m_SlideGObjs[m_Index].SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}

#if UNITY_EDITOR
public partial class TutorialMgr : MonoBehaviour
{
    [MenuItem("CONTEXT/TutorialMgr/Setup")]
    private static void Setup_F(MenuCommand menuCommand)
    {
        TutorialMgr context = menuCommand.context as TutorialMgr;
        Undo.RegisterCompleteObjectUndo(context, "Setup");

        RaycastPaddingDisable_F(context);
        AddSlides_F(context);

    }

    private static void AddSlides_F(TutorialMgr context)
    {
        context.m_SlideGObjs = new GameObject[context.transform.childCount];
        for (int i = 0; i < context.transform.childCount; i++)
            context.m_SlideGObjs[i] = context.transform.GetChild(i).gameObject;
    }

    private static void RaycastPaddingDisable_F(TutorialMgr tutorialMgr)
    {
        
        foreach (Graphic g in tutorialMgr.GetComponentsInChildren<Graphic>())
        {
            if (g.gameObject == tutorialMgr.gameObject) continue;

            Undo.RegisterCompleteObjectUndo(g, "Set Raycast Target to false for slides");
            g.raycastTarget = g.GetComponent<Button>() != null;
        }
    }

    //private static void AddSlidesToNextBtn_F()
    //{
    //    GameObject button = new GameObject("Slides To Next Btn");
    //    RectTransform rectTransform = button.AddComponent<RectTransform>();
    //    button.AddComponent<Image>();
    //    button.AddComponent<Button>();

    //    rectTransform.
    //}
}
#endif
