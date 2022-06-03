using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject gameMenu, pauseMenu;
    
    void Awake()
    {
        gameMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            pauseMenu.SetActive(!pauseMenu.activeSelf);
    }
}
