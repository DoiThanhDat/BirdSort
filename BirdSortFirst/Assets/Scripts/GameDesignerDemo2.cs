using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class GameDesignerDemo2 : MonoBehaviour
{
    public LevelData currentLevel;
    public BirdCatalog catalog;
    public BaseBranch branches;
    public List<Transform> branchPoint;

    void Start()
    {
        
    }

  public void SpawnLevel()
    {
        if (currentLevel == null || catalog == null || branches == null)
        {
            return;
        }
        // Quet cau hinh tu Level Data
        for (int i = 0;i < currentLevel.branchLists.Count;i++)
        {
            if (i >= branchPoint.Count) break;
            Transform targerPos = branchPoint[i];
            if (targerPos == null) continue;
            BranchSetUp setUp = currentLevel.branchLists[i];

            //Spawn canh cay:
            BaseBranch newBranch = Instantiate(branches, targerPos);
            newBranch.transform.localPosition = Vector3.zero;

            //thiet lap quay
            if (setUp.side == 1)
            {
                newBranch.isRightBranch = true;
                newBranch.transform.localScale = new Vector3 (-1,1,1);
            }
            else
            {
                newBranch.isRightBranch= false;
                newBranch.transform.localScale = Vector3.one;
            }

            //Spawn chim
            for (int j = 0;j < setUp.slotID.Count;j++)
            {
                int birdID = setUp.slotID[j];
                BaseBird birdToSPawn = catalog.GetBirdsByID(birdID);
                if (birdToSPawn != null) continue;
                BaseBird newBird = Instantiate(birdToSPawn, newBranch.transform);  

                //quay nguoc chim lan nua:
                newBird.transform.localScale = newBranch.isRightBranch? new Vector3(-1,1,1): Vector3.one;
                if (j < newBranch.birdPositionInBranch.Count)
                {
                    newBird.transform.position = newBranch.birdPositionInBranch[j].position;
                }
                newBranch.AddBird(newBird);
                newBird.SetFacing(1f);
            }
        }
    }
}
