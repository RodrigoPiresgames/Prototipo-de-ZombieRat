using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControler : MonoBehaviour
{

    public void LoadScene() 
    {
        SceneManager.LoadScene("GameScene");
    }

    public void Exist()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

}
