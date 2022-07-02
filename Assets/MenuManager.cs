using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour
{

    public Button startButton;
    public Button exitButton;
    
    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(delegate ()
        {
            LevelManager.loadScene(LevelManager.scene1);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
