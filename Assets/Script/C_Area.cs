using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Area : MonoBehaviour
{
    public GameObject wall, floor, bulpa, star;
    public GameObject[] portals = new GameObject[4];
    public Transform[] area = new Transform[3];
    public Vector3Int size = new Vector3Int(8,8,8);
    public int doors = 1;
    Vector3Int doorCoord;
    Vector3[] starCoord = new Vector3[3];
    // Start is called before the first frame update
    void Start()
    {
        doors = C_GameValues.World;
        size = Vector3Int.one * (4 + C_GameValues.level);
        doorCoord = new Vector3Int(Random.Range(0, size.x), Random.Range(2, size.y), 0);
        int[] limits = { 1, size.x - 1, 0, doorCoord.y - 1, 1, size.z - 1 };
        
        for (int d = 0; d < starCoord.Length; d++)
        {
            bool onFloor = Random.Range(0f, 1f) >= 0.5f;
            bool isNew = false;
            while (!isNew)
            {
                isNew = true;
                starCoord[d] = new Vector3(Random.Range(limits[0], limits[1] + 1), !onFloor ? Random.Range(limits[2], limits[3] + 1) : 0, onFloor ? -Random.Range(limits[2], limits[3] + 1) : 0) + new Vector3(0.5f,0.5f,-0.5f);
                for (int i = 0; i < d; i++)
                {
                    if (starCoord[i] == starCoord[d]) isNew = false;
                }
            }
            Instantiate(star, starCoord[d], Quaternion.identity);
        }
        
        
        genWall(floor, area[0], 0, 2);
        genWall(wall, area[1], 0, 1);

        genInvisWall(new Vector3(-0.5f, size.y / 2f, -size.z / 2f), 0);
        genInvisWall(new Vector3(size.x + 0.5f, size.y / 2f, -size.z / 2f), 0);        
        genInvisWall(new Vector3(size.x / 2f, size.y + 0.5f, -size.z / 2f), 1);
        genInvisWall(new Vector3(size.x / 2f, size.y / 2f, -size.z - 0.5f), 2);


        Transform[] fB= GetComponentsInChildren<Transform>()[1].gameObject.GetComponentsInChildren<Transform>();
        Transform bulpaSpawner = fB[fB.Length - 5];
        GameObject jumper = Instantiate(bulpa, bulpaSpawner.transform.position + Vector3.up/2, Quaternion.identity);
        jumper.name = "Bulpa";
        jumper.GetComponent<C_BulpAI>().start = bulpaSpawner.gameObject;
    }

    void genInvisWall (Vector3 pos, int siz)
    {
        GameObject board = Instantiate(wall, pos, Quaternion.identity);
        Vector3 s = new Vector3(size.x + 2, size.y + 2, size.z + 2);
        s[siz] = 1;
        board.transform.localScale = s;
        board.GetComponent<BoxCollider>().isTrigger = true;
        Transform[] mr = board.GetComponentsInChildren<Transform>();
        foreach (Transform t in mr)
        {
            Destroy(t.GetComponent<MeshRenderer>());
            Destroy(t.GetComponent<MeshFilter>());
        }
        board.layer = 2;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    
    void genWall (GameObject gO, Transform par, int c1, int c2)
    {
        Vector3 offset = Vector3.one / 2f;
        GameObject portal = portals[2];
        Vector3 angle = new Vector3 (((c1 == 0) || (c2 == 0)) ? 0f : 90f, ((c1 == 1) || (c2 == 1)) ? 0f : 90f, ((c1 == 2) || (c2 == 2)) ? 0f : -90f);
        if (((c1 == 0) && (c2 == 2)) || ((c1 == 2) && (c2 == 0))) //Пол
        {
            offset += Vector3.down + Vector3.back;
            portal = portals[1];
        }
        if (((c1 == 0) && (c2 == 1)) || ((c1 == 1) && (c2 == 0))) //Правая стена
        {
            offset += Vector3.zero;
            portal = portals[2];
        }
        Vector3Int c0 = Vector3Int.zero;
        GameObject generate;
        for (c0[c1] = 0; c0[c1] < size[c1]; c0[c1]++)
        {
            for (c0[c2] = 0; c0[c2] < size[c2]; c0[c2]++)
            {
                generate = gO;
                if (doorCoord == c0)
                {
                    portal.GetComponent<C_Finish>().exit = true;
                    generate = portal;
                }
                GameObject block = Instantiate(generate, C_MF.mulVec3(c0, (Vector3Int.one + 2 * Vector3Int.back)) + offset, Quaternion.Euler(angle), par);
                block.name = string.Format("{0}_{1}_{2}", generate.name, c0[c1], c0[c2]); 
            }
        }
    }
}
