using UnityEngine;

public class ExitApplication : MonoBehaviour
{
    // Call this method to exit the game
    public void ExitGame()
    {
        // This will close the application when running as a standalone build
        Application.Quit();


    }
}
