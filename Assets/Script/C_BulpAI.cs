using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static C_MF;


public class C_BulpAI : MonoBehaviour
{
    public Animator moves;
    float g;
    float t;
    bool finish = false;
    float walkSpeed = 1f;
    //[HideInInspector]
    public int stars = 0;

    // Start is called before the first frame update
    void Start()
    {
        g = Mathf.Abs(Physics.gravity.y);
    }
    float finishTime;
    public float finishAngle = 3f;
    public float finishDelay = 2f;
    Vector3 finishDir;
    public GameObject start;
    C_Finish portal;
    public state AS = state.Walk;
    public state S = state.Walk;

    public enum state
    {
        Walk,
        TurnLeft,
        Flip,
        DownJump,
        Falling,
        Repeat
    }
    float dxP, dzP;

    // Update is called once per frame

    private void FixedUpdate()
    {
        int cb = checkBoxes();
        if (!onAir && !onSpin && !finish)
        {
            switch (cb)
            {
                case 0:
                    AS = S = state.Walk;
                    GetComponent<Rigidbody>().velocity = gameObject.transform.forward * walkSpeed;
                    break;

                case 2:
                    direct -= 90;
                    //spin();
                    AS = S = state.TurnLeft;
                    
                    break;

                case 1:
                   
                    AS = state.Flip;
                    moves.SetBool(1, true);
                    step(jumpTime, correctJump, 1);

                    break;

                case 3:
                    AS = state.DownJump;
                    moves.SetBool(2, true);
                    step(jumpTime, correctDown, -1);
                    break;
                case 4:
                    finish = true;
                    finishTime = Time.time;
                    transform.position += Vector3.up / 4;
                    GetComponentsInChildren<Transform>()[1].localPosition = Vector3.down * 0.3f;
                    GetComponent<Rigidbody>().useGravity = false;
                    Vector3 vacuum = portal.transform.position - transform.position + Vector3.back/2;
                    GetComponent<Rigidbody>().velocity = (vacuum).normalized * (vacuum.magnitude / finishDelay);
                    break;
            }
            moves.SetInteger("State", (int)AS);
        }
        spin();
        if (finish)
        {
            C_GameValues.status = 1;
            if (Time.time - finishTime <= finishDelay)
            {
                transform.Rotate(Vector3.forward * finishAngle);
                
                transform.localScale = Vector3.one * ((finishDelay + 0.1f) - (Time.time - finishTime)) / finishDelay;
            }
            else
            {
                finish = false;
                onSpin = false;
                onAir = false;
                direct = 0;
            }
        }
    }


    bool onAir = false;
    public float correctStep = 1.15f;
    public float correctJump = 1.15f;
    public float correctDown = 1.15f;
    public float stepTime = 0.4f;
    public float stepDelay = 0.5f;
    public float jumpTime = 1f;
    void step(float time, float k, int level = 0)
    {
        onAir = true;
        t = time;
        GetComponent<Rigidbody>().velocity = level != 0 ? gameObject.transform.forward / t * k + Vector3.up * (g * t / 2 + level / t) : gameObject.transform.forward;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.collider.tag != "Unvis") && (collision.collider.tag != "") && (collision.collider.tag != "Star"))
        {
            if (onAir) transform.position = new Vector3(Mathf.Floor(transform.position.x) + 0.5f, transform.position.y, Mathf.Floor(transform.position.z) + 0.5f);
            onAir = false; 
        }

    }

  
    private void OnCollisionStay(Collision collision)
    {
        if ((collision.collider.tag != "Unvis") && (collision.collider.tag != "") && (collision.collider.tag != "Star"))
        {
            onAir = false;
           
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if ((collision.collider.tag != "Unvis") && (collision.collider.tag != "") && (collision.collider.tag != "Star"))
        {
            onAir = true;
        }
    }
    

    bool onSpin = false;
    float direct = 0f;
    public float rotTime = 0.3f;
    void spin()
    {
        float angle = (transform.eulerAngles.y);
        float cD = C_MF.toRotation(angle, direct);
        if (Mathf.Abs(cD) > 0.01)
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            onSpin = true;

            float dTA = Time.deltaTime * 90 / rotTime;
            float dA = Mathf.Min(dTA, Mathf.Abs(cD)) * Mathf.Sign(cD);
            transform.eulerAngles = C_MF.NormalAngles(transform.eulerAngles) + Vector3.up * dA;  
        }
        else
        {
            if (onSpin)
            {
                transform.position = new Vector3(Mathf.Floor(transform.position.x) + 0.5f, transform.position.y, Mathf.Floor(transform.position.z) + 0.5f);
                transform.eulerAngles = Vector3.up * direct;
                onSpin = false;
                checkBoxes();
            }
        }
    }

    public Transform eye;
    int checkBoxes()
    {
        
        // 0 - ��� ������
        // 1 - �������� �����
        // 2 - �������� ���������� - �������
        // 3 - �������� ����
        // 4 - �����
        C_BoxCollider[] eyes = eye.GetComponentsInChildren<C_BoxCollider>();
        if (eyes[7].tag == "Star" && eyes[7].trig != null)
        {

            Destroy(eyes[7].trig.gameObject);
            eyes[7].tag = "";
            eyes[7].trig = null;
            stars++;

        }

        if (eyes[0].tag == "Finish")
        {
            //   Debug.Log(string.Format("���� �����"));
            //finish = true;
            portal = eyes[0].GetComponent<C_BoxCollider>().trig.GetComponent<C_Finish>();
            finishDir = Vector3.up;
            //S = state.Falling;
            return 4;
        }
        if (eyes[1].tag == "Finish")
        {
            //   Debug.Log(string.Format("���� �����"));
            //finish = true;
            portal = eyes[1].GetComponent<C_BoxCollider>().trig.GetComponent<C_Finish>();
            finishDir = Vector3.right;
           // S = state.Falling;
            return 4;
        }
        if (eyes[2].tag == "Finish")
        {
            //   Debug.Log(string.Format("���� �����"));
            //finish = true;
            portal = eyes[2].GetComponent<C_BoxCollider>().trig.GetComponent<C_Finish>();
            finishDir = Vector3.right;
           // S = state.Falling;
            return 4;
        }
        if (eyes[4].tag == "Finish")
        {
            //   Debug.Log(string.Format("���� �����"));
            //finish = true;
            portal = eyes[4].GetComponent<C_BoxCollider>().trig.GetComponent<C_Finish>();
            finishDir = Vector3.forward;
           // S = state.Falling;
            return 4;
        }
        if ((eyes[4].tag != "Unvis") && (eyes[4].tag != "") && (eyes[4].tag != "Star"))
        {
           // Debug.Log(string.Format("������� ����"));
            if ((eyes[3].tag != "Unvis") && (eyes[3].tag != "") && (eyes[3].tag != "Star"))
            {
                //       Debug.Log(string.Format("�� ���� ����������"));
            //    S = state.TurnLeft;
                return 2;
            }
            else
            {
                //     Debug.Log(string.Format("���� ����������"));
            //    S = state.Flip;
                return 1;
            }
        }
        else
        {
          //  Debug.Log(string.Format("������� ����� ���"));
            if ((eyes[5].tag != "Unvis") && (eyes[5].tag != "") && (eyes[5].tag != "Star"))
            {
                //     Debug.Log(string.Format("���� ���� ������"));
             //   S = state.Walk;
                return 0;
            }
            else
            {
           //     Debug.Log(string.Format("������� �����"));
                if ((eyes[6].tag != "Unvis") && (eyes[6].tag != "") && (eyes[6].tag != "Star"))
                {
                    //        Debug.Log(string.Format("���� ���������"));
            //        S = state.DownJump;
                    return 3;
                }
                else
                {
                    //        Debug.Log(string.Format("�� ���� ���������"));
            //        S = state.TurnLeft;
                    return 2;
                }
            }
        }    
    }
}
