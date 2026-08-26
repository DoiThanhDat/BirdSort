using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class BaseBranch : MonoBehaviour
{
    public static BaseBranch selectedBranch = null;
    public float birdRange;
    public int capacity;
    public List<BaseBird> birds = new List<BaseBird> ();
    public bool isRightBranch;
    BranchTest m_gc;
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_gc = FindAnyObjectByType<BranchTest>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    #region Add, Remove & Update Bird Position
    public void AddBird (BaseBird bird )
    {
        birds.Add ( bird );
        bird.currentBranch = this;
    }

    public void RemoveBird (BaseBird bird)
    {
        birds.Remove ( bird );
        bird.currentBranch = null;
    }
    
    public void UpdateBirdPosition()
    {
        if (isRightBranch == false)
        {
            for (int i = 0; i < birds.Count; i++)
            {
                float yPos = (float)(transform.position.y + 0.5f);
                float xPos = (float)(transform.position.x - 2 + i * birdRange);
                Vector3 birdPosition = new Vector3(xPos, yPos, 0);
                birds[i].transform.position = birdPosition;
            }
        }
        if (isRightBranch)
            for (int i = 0;i < birds.Count;i++)
            {
                float yPos = (float)(transform.position.y + 0.5f);
                float xPos = (float)((transform.position.x + 2 + (-i) * birdRange));
                Vector3 birdPosition = new Vector3(xPos, yPos, 0);
                birds[i].transform.position = birdPosition;
            }
    }
    #endregion

    #region Move Bird To 
    public void MoveBirdTo(BaseBranch sourceBranch, BaseBranch targetBranch)
    {
            List<BaseBird> MovinBird = sourceBranch.CheckColor(); //gán hàm
            int emptySlots = targetBranch.capacity - targetBranch.birds.Count;
            int birdsToEmptySlot = Mathf.Min(MovinBird.Count, emptySlots);
            bool canMove = emptySlots > 0 && (targetBranch.birds.Count == 0 || (targetBranch.birds[targetBranch.birds.Count - 1].ID == MovinBird[0].ID));
            if (canMove)
            {
                for (int i = 0; i < birdsToEmptySlot; i++)
                {
                    BaseBird birdToMove = sourceBranch.birds[sourceBranch.birds.Count - 1];
                    sourceBranch.birds.RemoveAt(sourceBranch.birds.Count - 1);
                    targetBranch.birds.Add(birdToMove);
                }
            sourceBranch.UpdateBirdPosition();
            targetBranch.UpdateBirdPosition();
            }
            FindFirstObjectByType<GameDesignerDemo>().CheckGameOver();
        CheckPoint();
    }
    #endregion

    #region On Mouse Down
    private void OnMouseDown()
    {
        if (m_gc.IsGameOver() || m_gc.SetGameFinishedState())
            return;
        if (selectedBranch == null)
        {
            if (birds.Count > 0)
            {
                selectedBranch = this;
            }
        }
        else
        {
            if (selectedBranch == this)
            {
                selectedBranch = null;
            }
            else
            {
                MoveBirdTo(selectedBranch, this);
                selectedBranch = null;
            }
        }
    }
    #endregion

    #region Check Point
    public void CheckPoint()
    {
        int demCungMau = 0;
        for (int i = birds.Count - 1; i > 0; i--)
        {
            if (birds[birds.Count - 1].ID == birds[i - 1].ID)
            {
                demCungMau++;
                if (demCungMau == capacity - 1)
                {
                    foreach (BaseBird bird in birds)
                    {
                        Destroy(bird.gameObject);
                    }
                    Destroy(gameObject);
                    m_gc.AddCompletedBranch();
                    m_gc.ScoreIncrement();
                }
            }
            m_gc.CheckIsGameFinished(true);
        }

    }
    #endregion

    #region Check Color
public List<BaseBird> CheckColor()
    {
        List<BaseBird> BirdsToMove = new List<BaseBird>();
        if (birds.Count == 0)
            return BirdsToMove;
        if (birds.Count == 1)
        {
            BirdsToMove.Add(birds[0]);
        }
        if (birds.Count > 1 && birds.Count <= capacity)
        {
            int targetID = birds[birds.Count-1].ID;
            for (int i = birds.Count -1; i >= 0; i--)
            {
                if (birds[i].ID == targetID)
                {
                    BirdsToMove.Add(birds[i]);

                }
                else
                    break;
            }
        }
        return BirdsToMove;
    }
    #endregion

    #region Check Top Bird
    public BaseBird CheckTopBird()
    {
        if (birds.Count == 0)
            return null;
        else if (birds.Count > 0 && birds.Count <= capacity)
            return birds[birds.Count-1];
        else return null;
    }
    #endregion

}
