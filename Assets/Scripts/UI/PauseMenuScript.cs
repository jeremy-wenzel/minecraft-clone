using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class PauseMenuScript : MonoBehaviour
{
    public static bool GamePaused = false;

    public GameObject PauseMenuObject;

    private static PauseMenuScript Menu;

    private void Start()
    {
        Menu = this;
        SetGameState(false);
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SetGameState(!GamePaused);
        }
    }

    public static void Unpause()
    {
        Menu.SetGameState(false);
    }

    private void SetGameState(bool isPaused)
    {
        GamePaused = isPaused;
        PauseMenuObject.SetActive(isPaused);
        Cursor.visible = isPaused;
    }
}
