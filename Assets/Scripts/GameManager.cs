using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool wrapScenes = true;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Reload();
        if (Input.GetKeyDown(KeyCode.N)) Next();
        if (Input.GetKeyDown(KeyCode.P)) Prev();
    }
    void Reload()
    {
        var i = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(i);
    }
    void Next()
    {
        int i = SceneManager.GetActiveScene().buildIndex;
        int count = SceneManager.sceneCountInBuildSettings;
        int next = i + 1;
        if (next >= count && !wrapScenes) return;
        SceneManager.LoadScene(next % count);
    }
    void Prev()
    {
        int i = SceneManager.GetActiveScene().buildIndex;
        int count = SceneManager.sceneCountInBuildSettings;
        int prev = i - 1;
        if (prev < 0 && !wrapScenes) return;
        if (prev < 0) prev = count - 1;
        SceneManager.LoadScene(prev);
    }
}
