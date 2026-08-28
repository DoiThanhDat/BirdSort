using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;
using DG.Tweening;

public class BaseBranch : MonoBehaviour
{
    public static BaseBranch selectedBranch = null;
    public float birdRange;
    public int capacity;
    public List<BaseBird> birds = new List<BaseBird> ();
    public bool isRightBranch;
    BranchTest m_gc;
    public float moveSpeed;
    public static bool isMoving = false;
  
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
        for (int i = 0; i < birds.Count; i++)
        {
            if (isRightBranch == false)
            {
                float yPos = (float)(transform.position.y + 0.5f);
                float xPos = (float)(transform.position.x - 2 + i * birdRange);
                Vector3 birdPosition = new Vector3(xPos, yPos, 0);
                birds[i].transform.position = birdPosition;
            }
            else if (isRightBranch == true)
            {
                 float yPos = (float)(transform.position.y + 0.5f);
                float xPos = (float)((transform.position.x + 2 + (-i) * birdRange));
                Vector3 birdPosition = new Vector3(xPos, yPos, 0);
                birds[i].transform.position = birdPosition;
            }
        }
    }
    /*
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
       */
    #endregion

    //Mới: Lambda Expression
    #region Update Bird Posion With Tween
    public void UpdateBirdPositionWithTween(System.Action onAllMovesComplated = null)
    {
        if (birds.Count  == 0)
        {
            onAllMovesComplated?.Invoke();
            return;
        }
        else if (birds.Count > 0 && birds.Count <= capacity)
        {
            if (isRightBranch == false)
            {
                int completedMoveBird = 0;
                for (int i = 0; i < birds.Count; i++)
                {
                    float yPos = (float)(transform.position.y + 0.5f);
                    float xPos = (float)(transform.position.x - 2 + i * birdRange);
                    Vector3 targetPosition = new Vector3(xPos, yPos, 0);
                    float distanceMove = Vector3.Distance(birds[i].transform.position, targetPosition);
                    float moveDuration = (float)distanceMove / moveSpeed;
                    birds[i].transform.DOMove(targetPosition, moveDuration).OnComplete(() =>
                    {
                        completedMoveBird++;
                        if (completedMoveBird == birds.Count)
                        {
                            onAllMovesComplated?.Invoke();
                        }
                    });
                }
            }
            if (isRightBranch)
            {
                int completedMoveBird = 0;
                for (int i = 0; i < birds.Count; i++)
                {
                    float yPos = (float)(transform.position.y + 0.5f);
                    float xPos = (float)((transform.position.x + 2 + (-i) * birdRange));
                    Vector3 targetPosition = new Vector3(xPos, yPos, 0);
                    float distanceMove = Vector3.Distance(birds[i].transform.position, targetPosition);
                    float moveDuration = (float)distanceMove / moveSpeed;
                    birds[i].transform.DOMove(targetPosition, moveDuration).OnComplete(() =>
                    {
                        completedMoveBird++;
                        if (completedMoveBird == birds.Count)
                        {
                            onAllMovesComplated?.Invoke();
                        }
                    });
                }
            }
        }
    }
    #endregion

    //Hàm mới: Mathf.Min
    #region Move Bird To 
    public void MoveBirdTo(BaseBranch sourceBranch, BaseBranch targetBranch)
    {
            List<BaseBird> MovinBird = sourceBranch.CheckColor(); //gán hàm BirdsToMove vừa return ở CheckColor(); 
            int emptySlots = targetBranch.capacity - targetBranch.birds.Count;
            int birdsToEmptySlot = Mathf.Min(MovinBird.Count, emptySlots);
            bool canMove = emptySlots > 0 && (targetBranch.birds.Count == 0 || (targetBranch.birds[targetBranch.birds.Count - 1].ID == MovinBird[0].ID));
            if (canMove)
            {
            isMoving = true;
                for (int i = 0; i < birdsToEmptySlot; i++)
                {
                    BaseBird birdToMove = sourceBranch.birds[sourceBranch.birds.Count - 1];
                    sourceBranch.birds.RemoveAt(sourceBranch.birds.Count - 1);
                    targetBranch.birds.Add(birdToMove);
                }
            sourceBranch.UpdateBirdPosition();
            targetBranch.UpdateBirdPositionWithTween(() => {
                isMoving = false;
                targetBranch.CheckPoint();
                FindFirstObjectByType<GameDesignerDemo>().CheckGameOver();
            });
            }
    }
    #endregion

    #region On Mouse Down
    private void OnMouseDown()
    {
        if (m_gc.IsGameOver() || m_gc.SetGameFinishedState() || isMoving)
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
