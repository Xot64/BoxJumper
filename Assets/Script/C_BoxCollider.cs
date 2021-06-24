using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_BoxCollider : MonoBehaviour
{
    public Collider trig = null;
    public string tag = "";

    void Start()
    {
        
    }
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        trig = other;
        tag = other.tag;
    }

    private void OnTriggerStay(Collider other)
    {
        trig = other;
        tag = other.tag;
    }

    private void OnTriggerExit(Collider other)
    {
        trig = null;
        tag = "";
    }
}
