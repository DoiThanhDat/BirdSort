using UnityEngine;

public class BranchTest : MonoBehaviour
{
    public BaseBranch branchA;
    public BaseBranch branchB;
    public BaseBird birdA1;
    public BaseBird birdA2;
    public BaseBird birdA3;
    public BaseBird birdA4;
    public BaseBird birdB1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        branchA.AddBird(birdA1);
        branchA.AddBird(birdA2);
        branchA.AddBird(birdA3);
        branchB.AddBird(birdB1);
        branchA.UpdateBirdPosition();
        branchA.MoveBirdTo(birdA4, branchB);
        branchA.UpdateBirdPosition();
        branchB.UpdateBirdPosition();


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
