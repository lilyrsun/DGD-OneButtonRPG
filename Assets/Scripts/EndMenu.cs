using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndMenu : MonoBehaviour
{
    public TMP_Text resultText;

    void Start()
    {
        resultText.text = EndStateHolder.PlayerWon ? "You Win!" : "You Lose...";
    }

    public void Restart() => SceneManager.LoadScene("Game");
    public void MainMenu() => SceneManager.LoadScene("Start");
}
