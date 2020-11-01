using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class ScenesLoader : MonoBehaviour
{
    #region Variables
    private bool IsLoading;
    #endregion

    public void LoadScene_F(ScenesEN scene)
    {
        if (IsLoading == true) return;
        
        StartCoroutine(LoadScene_IEF(scene));
    }

    private IEnumerator LoadScene_IEF(ScenesEN scene)
    {
        IsLoading = true;
        yield return Addressables.LoadSceneAsync(scene.ToSceneString_F());
        IsLoading = false;
    }

    public AsyncOperationHandle<SceneInstance> LoadScene_F(ScenesEN scene, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
    {
        return Addressables.LoadSceneAsync(scene.ToSceneString_F(), loadSceneMode, activateOnLoad);
    }

    public enum ScenesEN { MainMenu, MainGame }
}


//ScenesEN
public partial class ExtensionMethods
{
    public static string ToSceneString_F(this ScenesLoader.ScenesEN scene)
    {
        return scene switch
        {
            ScenesLoader.ScenesEN.MainMenu => "MainMenu_Scene",
            ScenesLoader.ScenesEN.MainGame => "MainGame_Scene",
            _ => throw new System.Exception($"Scene {scene} not available.")
        };
    }
}
