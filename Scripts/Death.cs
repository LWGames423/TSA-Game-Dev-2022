using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Scene = UnityEditor.SearchService.Scene;

public class Death : MonoBehaviour
{
    private Collider2D playerCollider;
    public GameObject player;


    private void FixedUpdate()
    {
        if(player.transform.position.y < -10)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

        
        
}
