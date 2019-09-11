using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCrush : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void crush ()
    {
        var r = new System.Random();
        for (int i = 0; i < transform.childCount; i++)
        {
            Rigidbody rigid = transform.GetChild(i).GetComponent<Rigidbody>();
            Vector3 velocity = new Vector3();
            velocity.x = getRandom(-2, 2, 3);
            velocity.z = getRandom(0, 2, 4);
            rigid.velocity = velocity;

            velocity.x = getRandom(-10, 10, 3);
            velocity.y = getRandom(-10, 10, 3);
            velocity.z = getRandom(-10, 10, 3);
            rigid.angularVelocity = velocity;
        }

        Destroy(gameObject, 1);
    }

    private float getRandom(int min, int max, float fixedValue)
    {
        var random = new System.Random();
        float value = random.Next(min * 100, max * 100) / 100f;
        value = (value > 0) ? (value + fixedValue) : (value - fixedValue);
        return value;
    }
}
