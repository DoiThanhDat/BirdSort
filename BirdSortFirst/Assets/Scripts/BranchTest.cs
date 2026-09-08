using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.Port;

public class BranchTest : MonoBehaviour
{
    [SerializeField] Canvas gamePlayCanvas;
    public static BaseBranch selectedBranch = null;
    public bool isMoving = false;
    int m_score;
    bool m_isGameOver;
    public int totalBranchSets;
    int completedBranch;
    bool isGameFinished;
    UIManager m_ui;
    GameDesignerDemo m_gd;

    void Start()
    {
        m_gd = FindAnyObjectByType<GameDesignerDemo>();
        m_ui = FindAnyObjectByType<UIManager>();
        m_ui.SetScoreText("Score: " + m_score);
    }

    // Update is called once per frame
    void Update()
    {
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

    #region On Mouse Down
    public void OnClickedBranch(BaseBranch clickedBranch)
    {
        if (IsGameOver() || SetGameFinishedState() || isMoving)
            return;
        if (selectedBranch == null)
        {
            if (clickedBranch.birds.Count > 0)
            {
                selectedBranch = clickedBranch;
            }
        }
        else if (selectedBranch == clickedBranch)
        {
            selectedBranch = null;
        }
       else
        {
            MoveBirdTo(selectedBranch, clickedBranch);
            selectedBranch = null;
        }
    }
    #endregion

    #region Move Bird To 
    public void MoveBirdTo(BaseBranch sourceBranch, BaseBranch targetBranch)
    {
        List<BaseBird> MovinBird = sourceBranch.CheckColor(); //gán List BirdsToMove vừa return ở CheckColor(); Done 
        int emptySlots = targetBranch.capacity - targetBranch.birds.Count;
        int birdsToEmptySlot = Mathf.Min(MovinBird.Count, emptySlots);
        bool canMove = emptySlots > 0 && (targetBranch.birds.Count == 0 || (targetBranch.birds[targetBranch.birds.Count - 1].ID == MovinBird[0].ID));
        if (canMove)
        {
            isMoving = true;
            int completedCount = 0;
            for (int i = 0; i < birdsToEmptySlot; i++)
            {
                BaseBird birdToMove = sourceBranch.birds[sourceBranch.birds.Count - 1];
                sourceBranch.RemoveBird(birdToMove);
                int targetSlotIndex = targetBranch.birds.Count ;
                Vector3 targetPos = targetBranch.GetSlotPosition(targetSlotIndex);
                Vector3 targetWolrdPos = targetBranch.transform.TransformPoint(targetPos);

                birdToMove.transform.SetParent(targetBranch.transform, true);
                birdToMove.transform.localScale = Vector3.one;
                targetBranch.AddBird(birdToMove);

                if (birdToMove.transform.position.x < targetWolrdPos.x)
                {
                    birdToMove.SetFacing(targetBranch.isRightBranch ? -1f : 1f);
                }
                if (birdToMove.transform.position.x > targetWolrdPos.x)
                {
                    birdToMove.SetFacing(targetBranch.isRightBranch ? 1f : -1f);
                }
                birdToMove.MoveTo(targetPos, () =>
                {
                    birdToMove.SetFacing(1f);
                    completedCount++;
                    if (completedCount == birdsToEmptySlot)
                    {
                        isMoving = false;
                        targetBranch.CheckPoint();
                        m_gd.CheckGameOver();
                    }
                   
                });
            }   
        }
    }
    #endregion

    #region Replay Button
    public void Replay()
    {
        SceneManager.LoadScene("aaa");
    }
    #endregion

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