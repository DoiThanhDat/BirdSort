using System.Collections.Generic;
using UnityEngine;





[CreateAssetMenu(fileName = "BirdCatalogJSON", menuName = "Bird Sort/ Bird Catalog JSON New")]
public class BirdCatalog : ScriptableObject
{
    public List<BaseBird> allBirds = new List<BaseBird>();
    public List<BranchSetUp> branchLists;
    public BaseBird GetBirdsByID(int id)
    {
        for (int i = 0; i < allBirds.Count; i++)
        {
            if (allBirds[i].ID == id) return allBirds[i];
        }
        return null;

    }
}
