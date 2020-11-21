using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void Quit()
    {
        SceneManager.LoadScene(0);
    }
}
