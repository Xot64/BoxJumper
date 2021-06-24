using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class C_Player : MonoBehaviour
{
    //public GameObject[] Figures;
    Vector3 direct = new Vector3(1f, 1f, -1f);
    public GameObject cubes;
    public GameObject box;
    public int maxBoxes = 8;
    public Material[] materials = new Material[3];
    GameObject takingBlock;
    GameObject newBox;
    int col = 0;
    public Text[] text = new Text[4];

    // Start is called before the first frame update
    void Start()
    {

    }
    Vector3[] boxSizes;
    Transform[] boxes;
    // Update is called once per frame
    void Update()
    {
        Ray ray = gameObject.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool stat = false;
        text[0].text = string.Format("{0}-{1}", C_GameValues.World, C_GameValues.level);
        if (Input.GetButtonDown("Fire1"))
        {
            if (Physics.Raycast(ray, out hit, 1000f, 3 << 6))
            {
                takingBlock = hit.collider.gameObject;
                Vector3 dir = Vector3.zero;

                switch (takingBlock.tag)
                {
                    case "Wall":
                        dir = Vector3.back;
                        stat = true;
                        break;
                    case "Floor":
                        dir = Vector3.up;
                        stat = true;
                        break;
                    case "Box":
                        dir = hit.point - takingBlock.GetComponent<Transform>().position;
                        dir.x = (Mathf.Abs(dir.x) > Mathf.Abs(dir.y) && Mathf.Abs(dir.x) > Mathf.Abs(dir.z)) ? Mathf.Sign(dir.x) : 0;
                        dir.y = (Mathf.Abs(dir.y) > Mathf.Abs(dir.x) && Mathf.Abs(dir.y) > Mathf.Abs(dir.z)) ? Mathf.Sign(dir.y) : 0;
                        dir.z = (Mathf.Abs(dir.z) > Mathf.Abs(dir.x) && Mathf.Abs(dir.z) > Mathf.Abs(dir.y)) ? Mathf.Sign(dir.z) : 0;
                        stat = false;
                        break;
                }
                newBox = Instantiate(box, takingBlock.GetComponent<Transform>().position + dir, Quaternion.identity);
                Rigidbody r = newBox.GetComponent<Rigidbody>();
                r.isKinematic = true;
                r.useGravity = false;
                Transform[] mr = newBox.GetComponentsInChildren<Transform>();
                foreach (Transform t in mr)
                {
                    t.GetComponent<Renderer>().material = materials[1];
                }
                newBox.layer = 2;
                newBox.GetComponent<BoxCollider>().enabled = false;
            }
        }
        if (Input.GetButton("Fire1"))
        {
            if (Physics.Raycast(ray, out hit, 1000f, 3 << 6))
            {
                newBox.SetActive(true);
                takingBlock = hit.collider.gameObject;
                Vector3 dir = Vector3.zero;

                switch (takingBlock.tag)
                {
                    case "Wall":
                        dir = Vector3.back;
                        stat = true;
                        break;
                    case "Floor":
                        dir = Vector3.up;
                        stat = true;
                        break;
                    case "Box":
                        dir = hit.point - takingBlock.GetComponent<Transform>().position;
                        dir.x = (Mathf.Abs(dir.x) > Mathf.Abs(dir.y) && Mathf.Abs(dir.x) > Mathf.Abs(dir.z)) ? Mathf.Sign(dir.x) : 0;
                        dir.y = (Mathf.Abs(dir.y) > Mathf.Abs(dir.x) && Mathf.Abs(dir.y) > Mathf.Abs(dir.z)) ? Mathf.Sign(dir.y) : 0;
                        dir.z = (Mathf.Abs(dir.z) > Mathf.Abs(dir.x) && Mathf.Abs(dir.z) > Mathf.Abs(dir.y)) ? Mathf.Sign(dir.z) : 0;
                        stat = false;
                        break;
                }
                newBox.GetComponent<Transform>().position = hit.collider.transform.position + dir;
                newBox.GetComponent<Rigidbody>().useGravity = false;
            }
            else
            {
                newBox.SetActive(false);
            }
        }
        if ((Input.GetButtonUp("Fire1")) && (newBox != null))
        {
            if (newBox.activeSelf)
            {
                newBox.GetComponent<Rigidbody>().isKinematic = stat;
                newBox.GetComponent<Rigidbody>().useGravity = true;
                newBox.GetComponent<BoxCollider>().enabled = true;
                Transform[] mr = newBox.GetComponentsInChildren<Transform>();
                Color nC = Color.black;
                while (nC == Color.black)
                {
                    nC = new Color(Random.Range(0, 2) * 0.98f, Random.Range(0, 2) * 0.98f, Random.Range(0, 2) * 0.98f);
                }
                foreach (Transform t in mr)
                {

                    t.GetComponent<Renderer>().material = materials[0];
                    t.GetComponent<Renderer>().material.color = nC;
                }
                newBox.layer = 6;
                newBox = null;
            }
            else
            {
                Destroy(newBox);
            }

        }        
    }

    void reColor(int c)
    {
        col = c;
        for (int i = 0; i < takingBlock.GetComponentsInChildren<Renderer>().Length; i++)
        {
            takingBlock.GetComponentsInChildren<Renderer>()[i].material = materials[c];
            if (col == 0)
                takingBlock.GetComponentsInChildren<BoxCollider>()[i].tag = "board";
            else
                takingBlock.GetComponentsInChildren<BoxCollider>()[i].tag = "Unvis";
        }
    }

    bool onTrig()
    {
        C_BoxCollider[] triggers = takingBlock.GetComponentsInChildren<C_BoxCollider>();
        foreach (C_BoxCollider BC in triggers)
        {
            if (BC.trig != null) if (BC.tag != "Unvis") return true;
        }
        return false;
    }

    void boxSize(float s)
    {

        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i].localScale = s * boxSizes[i];
        }
    }
    void setKinematic(bool b)
    {
        for (int i = 0; i < takingBlock.GetComponentsInChildren<Rigidbody>().Length; i++)
        {
            takingBlock.GetComponentsInChildren<Rigidbody>()[i].isKinematic = b;
            
        }
    }
    
}
