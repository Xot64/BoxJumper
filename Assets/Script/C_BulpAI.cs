using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static C_MF;


public class C_BulpAI : MonoBehaviour
{
    public Animator moves;
    float g;
    float t;
    bool finish = false;
    float walkSpeed = 1f;
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
        /*dxP = Mathf.Abs(transform.position.x - 0.5f - Mathf.Floor(transform.position.x -0.5f));
        dzP = Mathf.Abs(transform.position.z - 0.5f - Mathf.Floor(transform.position.z -0.5f));
        if ((dxP < 0.1f) && ((dzP < 0.1f)) && (Mathf.Abs(C_MF.toRotation(transform.eulerAngles.y, direct)) < 0.05) && !onAir && !onSpin)
        {
            switch (checkBoxes())
            {
                case 0:
                    AS = S = state.Walk;
                    
                    break;
                case 1:
                    AS = S = state.Flip;               
                    break;
                case 2:
                    S = state.TurnLeft;
                    AS = state.Walk;
                    direct -= 90;
                    break;
                case 3:
                    AS = S = state.DownJump;
                    break;
                case 4:
                    finish = true;
                    finishTime = Time.time;
                    break;
            }
        }*/
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
                    GetComponentsInChildren<Transform>()[1].localPosition = Vector3.down / 4;
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
            if (Time.time - finishTime <= finishDelay)
            {
                transform.Rotate(Vector3.forward * finishAngle);
                
                transform.localScale = Vector3.one * ((finishDelay + 0.1f) - (Time.time - finishTime)) / finishDelay;
            }
            else if (Time.time - finishTime <= 2* finishDelay)
            {
                if  (portal != null)
                {
                    bool exit = portal.GetComponent<C_Finish>().exit;
                    if (!exit)
                    {
                        portal.GetComponent<C_Finish>().boom(finishDir != Vector3.up);
                    }
                    else
                    {
                        C_GameValues.level++;
                        if (C_GameValues.level > 4)
                        {
                            C_GameValues.level = 1;
                            C_GameValues.World++;
                        }
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                }
                transform.position = start.transform.position + Vector3.up / 2;
                transform.Rotate(Vector3.up * finishAngle);
                transform.localScale = Vector3.one * ((Time.time - finishTime) - finishDelay) / finishDelay;
            }
            else if (Time.time - finishTime <= 2 * finishDelay + 0.5f)
            {
                transform.eulerAngles = Vector3.zero;
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
        if ((collision.collider.tag != "Unvis") && (collision.collider.tag != ""))
        {
            if (onAir) transform.position = new Vector3(Mathf.Floor(transform.position.x) + 0.5f, transform.position.y, Mathf.Floor(transform.position.z) + 0.5f);
            onAir = false;
            
        }

    }

    private void OnCollisionStay(Collision collision)
    {
        if ((collision.collider.tag != "Unvis") && (collision.collider.tag != ""))
        {
            onAir = false;
           
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if ((collision.collider.tag != "Unvis") && (collision.collider.tag != ""))
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
        // 0 - Шаг вперед
        // 1 - Прыгнуть вверх
        // 2 - Прыгнуть невозможно - поворот
        // 3 - Прыгнуть вниз
        // 4 - Выход
        C_BoxCollider[] eyes = eye.GetComponentsInChildren<C_BoxCollider>();
        if (eyes[0].tag == "Finish")
        {
            //   Debug.Log(string.Format("Вижу ВЫХОД"));
            //finish = true;
            portal = eyes[0].GetComponent<C_BoxCollider>().trig.GetComponent<C_Finish>();
            finishDir = Vector3.up;
            //S = state.Falling;
            return 4;
        }
        if (eyes[1].tag == "Finish")
        {
            //   Debug.Log(string.Format("Вижу ВЫХОД"));
            //finish = true;
            portal = eyes[1].GetComponent<C_BoxCollider>().trig.GetComponent<C_Finish>();
            finishDir = Vector3.right;
           // S = state.Falling;
            return 4;
        }
        if (eyes[2].tag == "Finish")
        {
            //   Debug.Log(string.Format("Вижу ВЫХОД"));
            //finish = true;
            portal = eyes[2].GetComponent<C_BoxCollider>().trig.GetComponent<C_Finish>();
            finishDir = Vector3.right;
           // S = state.Falling;
            return 4;
        }
        if (eyes[4].tag == "Finish")
        {
            //   Debug.Log(string.Format("Вижу ВЫХОД"));
            //finish = true;
            portal = eyes[4].GetComponent<C_BoxCollider>().trig.GetComponent<C_Finish>();
            finishDir = Vector3.forward;
           // S = state.Falling;
            return 4;
        }
        if ((eyes[4].tag != "Unvis") && (eyes[4].tag != ""))
        {
           // Debug.Log(string.Format("Впереди ящик"));
            if ((eyes[3].tag != "Unvis") && (eyes[3].tag != ""))
            {
                //       Debug.Log(string.Format("Не могу запрыгнуть"));
            //    S = state.TurnLeft;
                return 2;
            }
            else
            {
                //     Debug.Log(string.Format("Могу запрыгнуть"));
            //    S = state.Flip;
                return 1;
            }
        }
        else
        {
          //  Debug.Log(string.Format("Впереди ящика нет"));
            if ((eyes[5].tag != "Unvis") && (eyes[5].tag != ""))
            {
                //     Debug.Log(string.Format("Могу идти вперед"));
             //   S = state.Walk;
                return 0;
            }
            else
            {
           //     Debug.Log(string.Format("Впереди спуск"));
                if ((eyes[6].tag != "Unvis") && (eyes[6].tag != ""))
                {
                    //        Debug.Log(string.Format("Могу спрыгнуть"));
            //        S = state.DownJump;
                    return 3;
                }
                else
                {
                    //        Debug.Log(string.Format("Не могу спрыгнуть"));
            //        S = state.TurnLeft;
                    return 2;
                }
            }
        }    
    }
}
