using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class BranchTest : MonoBehaviour
{
    public static BaseBranch selectedBranch = null;     
    int m_score;
    bool m_isGameOver;
    public int totalBranchSets;
    int completedBranch;
    bool isGameFinished;
    UIManager m_ui;

    public float totalTime;
    float currentTime;


    void Start()
    {
        m_ui = FindAnyObjectByType<UIManager>();
        m_ui.SetScoreText("Score: " + m_score);
        currentTime = totalTime;
    }

    // Update is called once per frame
    void Update()
    {
       CheckTime();
       if (m_isGameOver)
        {
            m_ui.ShowGameOverPanel(true);
            return;
        }
       
       if (isGameFinished)
        {
            m_ui.ShowWinPanel(true);
            return;
        }
    }

    public void CheckTime()
    {
        currentTime -= Time.deltaTime;
        if (currentTime < 0 && isGameFinished == false)
        {
            SetGameOverState(true);
        }
    }
    public void Replay()
    {
        SceneManager.LoadScene("aaa");
    }

    #region Check Dieu Kien Thang 
    public void AddCompletedBranch ()
    {
        completedBranch++;
    }
    public void CheckIsGameFinished(bool dk)
    {
        if (completedBranch >=  totalBranchSets) 
            isGameFinished = dk;
    }
    public bool SetGameFinishedState()
    {
        return isGameFinished;
    }
    #endregion

    #region Set diem, Set game over
    public void SetScore(int value)
        { m_score = value; }
    public int GetScore()
        { return m_score; }
    public void ScoreIncrement()
    {
        m_score++;
        m_ui.SetScoreText("Score: " + m_score);
    }
    public void SetGameOverState(bool state)
    {
        m_isGameOver = state;
    }
    public bool IsGameOver()
    {
        return m_isGameOver;
    }
    #endregion








}