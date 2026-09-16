using System.Collections.Generic;
using UnityEngine;




//So luong cho tung ID chim (Config: Cau hinh)
[System.Serializable]
public class BirdSpeciesConfigJSON
{
    //Id
    public int birdID;
    //Tong so
    public int totalAmount;
}

//Cau truc cua 1 canh cay
[System.Serializable]
public class BranchSetUpJSON
{
    public int side;
    public List<int> slotID;
}

[System.Serializable]
public class LevelDataJSON
{
    public int numberLevel;
    public int totalBranches;
    public List<BirdSpeciesConfigJSON> birdConfigs;
    public List<BranchSetUpJSON> branchLists;
}
