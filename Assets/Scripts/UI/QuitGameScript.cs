using UnityEngine;

public class QuitGameScript : MonoBehaviour
{
   public void QuitGame()
    {
        Debug.Log("Quiting Game");
        Application.Quit();
    }
}
