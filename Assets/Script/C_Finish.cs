using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Finish : MonoBehaviour
{
    public bool exit = false;
    public GameObject[] blocks;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void boom(bool wall = false)
    {
        GameObject block = blocks[wall ? 1 : 0];
        Instantiate(block, transform.position, transform.rotation, transform.parent);
        Destroy(gameObject);
    }
}
