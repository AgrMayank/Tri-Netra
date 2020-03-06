using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button3D : MonoBehaviour
{
    public GameObject UIPanel;

    void OnMouseDown()
    {        
        if (UIPanel.activeInHierarchy)
        {
            UIPanel.SetActive(false);
        }
        else
        {
            UIPanel.SetActive(true);
        }
    }
}
