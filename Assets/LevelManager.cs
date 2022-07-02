using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class LevelManager 
{
    public static string  scene1 = "level1";
    public static string  scene2 = "level2";
    // Start is called before the first frame update


    public static void loadScene(string name)
    {
        SceneManager.LoadSceneAsync(name);
    }

}
