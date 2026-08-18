using UnityEngine;
using System.Collections.Generic;

public class BaseBranch : MonoBehaviour
{
    
    public float birdRange;
    public int capacity;
    List<BaseBird> birds = new List<BaseBird> ();
    
    
  
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
       
        for (int i = 0; i < birds.Count; i++)
        {
            float yPos = transform.position.y + 1;
            float xPos = (float)i * birdRange;
            Vector3 birdPosition = transform.position + new Vector3(xPos,yPos,0);
            birds[i].transform.position = birdPosition;
        }
    }
    public bool CanAddBird(BaseBird bird) // chưa cần dùng đến phần trong ngoặc, nhưng về sau sẽ dùng... 
    {
        if (birds.Count < capacity)
            return true;
        else
            return false;
    }
    public void MoveBirdTo(BaseBird bird, BaseBranch targetBranch)
    { 
        if (targetBranch.CanAddBird(bird))
        {
            RemoveBird(bird);
            targetBranch.AddBird(bird);
            UpdateBirdPosition();
            targetBranch.UpdateBirdPosition();
        }
    }
    
}
