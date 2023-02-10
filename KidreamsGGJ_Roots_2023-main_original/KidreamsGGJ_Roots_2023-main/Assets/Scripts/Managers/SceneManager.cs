using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public void ChangeToGameScene(int num)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(num);
    }
}
