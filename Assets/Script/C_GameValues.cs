using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class C_GameValues : MonoBehaviour
{
    public static int level = 1;
    public static int status = 0;
    public GameObject[] b = new GameObject[2];
    public GameObject Confetti;

    private void Update()
    {
        switch (status)
        {
            case 1:
                end(0);
                break;
            case 2:
                end(1);
                break;
        }
    }
    public void end(int butt)
    {
        b[butt].SetActive(true);
        if (butt == 0) Confetti.SetActive(true); 
    }
}