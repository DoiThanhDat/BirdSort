using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BranchTest : MonoBehaviour
{
    public static BaseBranch selectedBranch = null;

    int m_score;
    bool m_isGameOver;
    public int totalBranchSets;
    int completedBranch;
    bool isGameFinished;

    public void AddCompletedBranch ()
    {
        completedBranch++;
        if (completedBranch >= totalBranchSets)
        {
            isGameFinished = true;
        }
    }
   
    #region Set diem, Set game over
    public void SetScore(int value)
        { m_score = value; }
    public int GetScore()
        { return m_score; }
    public void ScoreIncrement()
    {
        m_score++;
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