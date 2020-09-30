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
    public void LoadScene_F(Scenes_EN scene)
    {
        StartCoroutine(LoadScene_IEF(scene));
    }

    private IEnumerator LoadScene_IEF(Scenes_EN scene)
    {        
        yield return Addressables.LoadSceneAsync(SceneKeyGet_F(scene));
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
