using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public void SetScoreText(string text)
    {
        if (scoreText)
        {
            scoreText.text = text;
        }
    }
    public void ShowGameOverPanel (bool isShow)
    {
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(isShow);
        }
    }
    public void ShowWinPanel(bool isShow)
    {
        if (winPanel)
        {
            winPanel.SetActive(isShow);
        }
    }

    #region Replay Button
    public void Replay()
    {
        SceneManager.LoadScene("aaa");
    }
    #endregion

    #region Next Level Button
    public void NextLevel()
    {
        int nextLevel = PlayerPrefs.GetInt("currentLevel",1) +1;
        PlayerPrefs.SetInt("currentLevel", nextLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion
}
