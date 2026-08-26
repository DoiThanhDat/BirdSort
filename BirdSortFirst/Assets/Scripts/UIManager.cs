using System.Security.Cryptography;
using UnityEngine;
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
}
