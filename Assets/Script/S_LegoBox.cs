using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_LegoBox : MonoBehaviour
{
    // Start is called before the first frame update
    Ray toWall;
    bool vacuum = false;
    private void Start()
    {
        
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        if (vacuum)
        {
            transform.Translate(Vector3.forward * 0.01f);
            transform.Rotate(Vector3.forward,15f);
            transform.localScale *= 0.9f;
            if (transform.position.z >= 0.5f) Destroy(gameObject);
        }
        else
        {
            toWall = new Ray(gameObject.transform.position + Vector3.up * 0.4f, Vector3.forward);
            RaycastHit hit;
            if (Physics.Raycast(toWall, out hit, 1f, 1 << 7))
            {
                if (hit.collider.tag == "Finish")
                {
                    vacuum = true;
                    Destroy(GetComponent<BoxCollider>());
                    Destroy(GetComponent<Rigidbody>());

                }
            }
        }
        
    }
}
