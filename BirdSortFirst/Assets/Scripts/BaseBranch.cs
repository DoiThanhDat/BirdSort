using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class BaseBranch : MonoBehaviour
{
    public static BaseBranch selectedBranch = null;
    public float birdRange;
    public int capacity;
    List<BaseBird> birds = new List<BaseBird> ();
    public bool isRightBranch;

    
    
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
                float xPos = (float)(transform.position.x + i * birdRange);
                Vector3 birdPosition = new Vector3(xPos, yPos, 0);
                birds[i].transform.position = birdPosition;
            }
        }
        if (isRightBranch)
            for (int i = 0;i < birds.Count;i++)
            {
                float yPos = (float)(transform.position.y + 0.5f);
                float xPos = (float)((transform.position.x + (-i) * birdRange));
                Vector3 birdPosition = new Vector3(xPos, yPos, 0);
                birds[i].transform.position = birdPosition;
            }
    }
   
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
    }
   
    private void OnMouseDown()
    {
        CheckBirdOnBranch();
        if (selectedBranch == null)
        {
            if (birds.Count > 0)
            {
                selectedBranch = this; 
                Debug.Log("Đã chọn cành nguồn: " + gameObject.name);
            }
        }
        else
        {
            if (selectedBranch == this)
            {
                selectedBranch = null;
                Debug.Log("Hủy chọn cành");
            }
            else
            {
                MoveBirdTo(selectedBranch, this);
                selectedBranch = null;
            }
        }
    }
    
    public void CheckBirdOnBranch()
    {
        int demCungMau = 0;
        if (birds.Count == 0)
            Debug.Log("Canh khong co chim");
        else if (birds.Count > 0 && birds.Count <= capacity)
        {
            for(int i = birds.Count - 1; i > 0 ; i--)
            {
                if (birds[birds.Count - 1].ID == birds[i - 1].ID)
                {
                   demCungMau++;
                    if (demCungMau == capacity-1)
                    {
                        foreach (BaseBird bird in birds)
                        {
                            Destroy(bird.gameObject);
                        }
                        Destroy(gameObject);
                    }
                }
                if (birds[birds.Count - 1].ID != birds[i - 1].ID)
                {
                    break;
                }

            }
        }
        
    }
    
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
    
}
