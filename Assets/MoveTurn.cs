using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MoveState
{
    Stop,
    Straight,
    ReadyTurn,
    TurnLeft,
    TurnRigth
}

public class MoveTurn : MonoBehaviour
{
    public GameObject crush;

    private Rigidbody rigid;
    private MoveState currentState;
    private GameObject turnCube;
    private float radius;
    private float speed;
    private float angularSpeed;
    private float turnAngle;
    private Vector3 eulerAngles;

    private bool startScale;
    private GameObject follow;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        radius = 1;
        speed = 4;
        angularSpeed = (360 / (Mathf.PI * 2)) * (speed / radius);
        currentState = MoveState.Straight;

        follow = GameObject.Find("Follow");
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        scale();
        move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Turn"))
        {
            turnAngle = 0;
            turnCube = collision.gameObject;
            currentState = MoveState.ReadyTurn;
            eulerAngles = transform.eulerAngles;
        }

        if (collision.gameObject.tag.Equals("Block"))
        {
            GameObject gbBlock = Instantiate(crush, collision.transform.position, collision.transform.rotation);
            gbBlock.transform.localScale = collision.transform.localScale;
            BlockCrush bc = gbBlock.GetComponent<BlockCrush>();
            bc.crush();

            rigid.velocity = new Vector3(0, 3, -6);
            Destroy(collision.gameObject);
        }
    }








    private void scale()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startScale = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            startScale = false;
        }
        if (startScale)
        {
            float increment = Input.GetAxis("Mouse Y") * 0.5f;
            Vector3 tScale = transform.localScale;
            tScale.y = tScale.y + increment;
            tScale.y = Mathf.Min(1.4f, tScale.y);
            tScale.y = Mathf.Max(0.2f, tScale.y);
            tScale.x = 1.6f - tScale.y;
            transform.localScale = tScale;
        }
    }

    private void move ()
    {
        if (currentState == MoveState.Stop)
        {
        }
        else if (currentState == MoveState.Straight)
        {
            Vector3 position = rigid.position + (transform.forward * speed * Time.deltaTime);
            position.y = transform.localScale.y / 2;
            rigid.MovePosition(position);
        }
        else if (currentState == MoveState.ReadyTurn)
        {
            Vector3 position = rigid.position + (transform.forward * speed * Time.deltaTime);
            Vector3 tPosition = turnCube.transform.InverseTransformPoint(position);
            if (Mathf.Abs(tPosition.x) < 0.5)
            {
                if (tPosition.x > 0)
                {
                    currentState = MoveState.TurnRigth;
                    tPosition.x = 0.5f;
                }
                else
                {
                    currentState = MoveState.TurnLeft;
                    tPosition.x = -0.5f;
                }
                position = turnCube.transform.TransformPoint(tPosition);
            }
            position.y = transform.localScale.y / 2;
            rigid.MovePosition(position);
        }
        else
        {
            turnAngle = turnAngle + (Time.deltaTime * angularSpeed);
            turnAngle = Mathf.Min(90, turnAngle);
            float angle = (turnAngle / 180) * Mathf.PI;
            Vector3 tPosition = new Vector3();
            tPosition.z = -0.5f * Mathf.Cos(angle);
            tPosition.x = 0.5f * Mathf.Sin(angle);
            Vector3 tEulerAngles = eulerAngles;
            if (currentState == MoveState.TurnLeft)
            {
                tEulerAngles.y = tEulerAngles.y - turnAngle;
                tPosition.x = tPosition.x - 0.5f;
                tPosition.z = tPosition.z + 0.5f;
            }
            else
            {
                tEulerAngles.y = tEulerAngles.y + turnAngle;
                tPosition.x = -1f * tPosition.x;
                tPosition.x = tPosition.x + 0.5f;
                tPosition.z = tPosition.z + 0.5f;
            }
            tPosition = turnCube.transform.TransformPoint(tPosition);
            Vector3 position = transform.position;
            position.x = tPosition.x;
            position.z = tPosition.z;
            position.y = transform.localScale.y / 2;
            transform.position = position;
            transform.eulerAngles = tEulerAngles;

            if (turnAngle >= 90)
            {
                turnAngle = 0;
                turnCube = null;
                currentState = MoveState.Straight;
                eulerAngles = transform.eulerAngles;
            }
        }

        follow.transform.position = transform.position;
        follow.transform.eulerAngles = transform.eulerAngles;
    }
}
