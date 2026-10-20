using DG.Tweening;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI iqText;
    public GameObject gameOverPanel;
    public GameObject winPanel;
    public GameObject winBox;
    public GameObject pausePanel;
    public GameObject pauseBox;
    public GameObject settingPanel;

    #region Show Win & Lose Panel
    public void ShowGameOverPanel (bool isShow)
    {
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(isShow);
        }
    }
    public void EscGameOverPanel()
    {
        gameOverPanel.SetActive(false);
    }
    public void ShowWinPanel(bool isShow)
    {
        CanvasGroup canvasGroup = winPanel.GetComponent<CanvasGroup>();
        winBox.transform.DOKill();
        canvasGroup.alpha = 0f;
        winBox.transform.localScale = Vector3.zero;
        winBox.SetActive(isShow);
        winPanel.SetActive(isShow);
        winBox.transform.DOScale(Vector3.one, 0.5f)
                            .SetEase(Ease.OutBack);
        canvasGroup.DOFade(1f, 0.25f);
    }
    #endregion

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

    #region Pause Panel
    public void PauseButton()
    {
        CanvasGroup canvasGroup = pausePanel.GetComponent<CanvasGroup>();

        pauseBox.transform.DOKill();

        canvasGroup.alpha = 0f;
        pauseBox.transform.localScale = Vector3.zero;
        pausePanel.SetActive(true);
        pauseBox.SetActive(true);

        pauseBox.transform.DOScale(Vector3.one, 0.5f)
                            .SetEase(Ease.OutBack);
        canvasGroup.DOFade(1f, 0.25f);
    }
    public void EscPausePannel()
    {
        CanvasGroup canvasGroup = pausePanel.GetComponent<CanvasGroup>();
        pauseBox.transform.DOKill();
        pauseBox.transform.DOScale(Vector3.zero, 0.3f)
                            .SetEase(Ease.InBack)
                            .OnComplete(() => 
                            {
                                pauseBox.SetActive(false);
                                canvasGroup.DOFade(0f, 0.1f).OnComplete(() =>
                                {
                                    pausePanel  .SetActive(false);
                                });
                            });
    }
    #endregion

    #region Setting Panel
    public void SettingPanelButton()
    {
        CanvasGroup canvasGroup = settingPanel.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        settingPanel.SetActive(true);
        canvasGroup.DOFade(1f, 0.25f);
    }
    public void EscSettingPanel()
    {
        CanvasGroup canvasGroup = settingPanel.GetComponent<CanvasGroup>();
        canvasGroup.DOFade(0f, 0.25f).OnComplete(() =>
        {
            settingPanel.SetActive(false);
        });
    }
    #endregion

    #region IQ Text
    public void SetScoreText(string text)
    {
        iqText.text = text;
    }
    #endregion

}
