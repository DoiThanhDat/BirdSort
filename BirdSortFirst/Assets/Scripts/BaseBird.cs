using UnityEngine;

public class BaseBird : MonoBehaviour
{
    public BaseBranch currentBranch;
    public int ID;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        Debug.Log("click: " + gameObject.name);
    }
    
}
