using UnityEngine;
using UnityEngine.SceneManagement;

public class Playsceneloader : MonoBehaviour
{
    public void LoadMainPlayScene()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void LoadMultiplayer()
    {
        SceneManager.LoadScene("multiplayer", LoadSceneMode.Single);
    }
}