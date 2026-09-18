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

    public void NextLevel()
    {
        int nextLevel = PlayerPrefs.GetInt("currenLevel",1) +1;
        PlayerPrefs.SetInt("currenLevel", nextLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
