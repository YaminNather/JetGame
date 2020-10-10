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
    private bool IsLoading;

    public void LoadScene_F(Scenes_EN scene)
    {
        if (IsLoading == true) return;
        
        StartCoroutine(LoadScene_IEF(scene));
    }

    private IEnumerator LoadScene_IEF(Scenes_EN scene)
    {
        IsLoading = true;
        yield return Addressables.LoadSceneAsync(SceneKeyGet_F(scene));
        IsLoading = false;
    }

    private string SceneKeyGet_F(Scenes_EN scene)
    {
        if (scene == Scenes_EN.MainMenu)
            return "MainMenu_Scene";
        else if (scene == Scenes_EN.MainGame)
            return "MainGame_Scene";
        else
            return null;
    }
}

public enum Scenes_EN { MainMenu, MainGame }
