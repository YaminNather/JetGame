using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
#endif

public partial class LevelEndHitbox : Hitbox
{
    private LevelMgr level;

    protected override void Awake()
    {
        base.Awake();

        level = GetComponentInParent<LevelMgr>();
        if (level != null)
            ListenerAdd_F(HitboxOnEnterLevelDespawn_EF);
        else
            Destroy(gameObject);
    }

    private void HitboxOnEnterLevelDespawn_EF(Collider other)
    {
        if(other.TryGetComponent(out PlayerHitbox ph))
        {
            if (MainGameReferences.INSTANCE != null) MainGameReferences.INSTANCE.levelsMgr.LevelDespawn_F(level);
            else FindObjectOfType<LevelsMgr>()?.LevelDespawn_F(level);
        }
    }
}

#if UNITY_EDITOR
public partial class LevelEndHitbox
{
    [MenuItem("CONTEXT/LevelEndHitbox/Set Position")]
    private static bool SetPosValidator_F(MenuCommand menuCommand)
    {
        //\nContext = {(menuCommand.context as GameObject).transform.name}
        Debug.Log($"<color=yellow>Selection object = {Selection.activeGameObject.transform.name}</color>");
        return PrefabStageUtility.GetCurrentPrefabStage() != null && Selection.objects.Length == 1 && (menuCommand.context as LevelEndHitbox);
    }

    [MenuItem("CONTEXT/LevelEndHitbox/Set Position")]
    private static void SetPos_F(MenuCommand menuCommand)
    {
        GameObject context = (menuCommand.context as LevelEndHitbox).gameObject;
        Debug.Log($"<color=yellow>Context = {context.transform.name}</color>");
        float MaxPos = 0.0f;
        foreach (Transform t in context.transform.parent.GetComponentsInChildren<BlockEditorMgr>().Select(x => x.transform))
        {
            if (t.position.z > MaxPos) MaxPos = t.position.z;
        }

        Undo.RegisterCompleteObjectUndo(context.transform, "Level End Hitbox Set Position");
        context.transform.position = new Vector3(0.0f, 0.0f, MaxPos + 10.0f);
    }
}
#endif
