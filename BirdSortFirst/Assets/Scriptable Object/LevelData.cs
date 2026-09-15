using UnityEngine;
using System.Collections.Generic;


//So luong cho tung ID chim (Config: Cau hinh)
[System.Serializable]
public class BirdSpeciesConfig
{
    //Id
    public int birdID;
    //Tong so
    public int totalAmount;
}

//Cau truc cua 1 canh cay
[System.Serializable]
public class BranchSetUp
{
    public int side;
    public List<int> slotID;
}

//Cau truc tong the cua 1 lv(khai bao)
[CreateAssetMenu(fileName = "Level_", menuName = "Bird Sort/Level Data Final")]
public class LevelData : ScriptableObject
{
    public int numberLevel;
    public int totalBranches;
    public List<BirdSpeciesConfig> birdConfigs;
    public List<BranchSetUp> branchLists;

}
