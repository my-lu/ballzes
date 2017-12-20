using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    Vector3 location;
    float moveSpeed = 1.0f;
    public float moveTime = 1.0f;

    public GameObject ball;

    void Start()
    {
        location = transform.position;
        GameManager.instance.rings.Add(this);
    }

    void Update()
    {
        if (transform.position != location)
        {
            transform.position = Vector3.MoveTowards(transform.position, location, moveSpeed * Time.deltaTime);
        }

        CheckDestroy();
    }

    void OnTriggerEnter(Collider other)
    {
        ball.GetComponent<Ball>().enabled = true;

        ball.transform.SetParent(null);
        GameManager.instance.rings.Remove(this);
        Destroy(gameObject);
    }

    public void MoveSquare()
    {
        location.y -= 1;
    }

    void CheckDestroy()
    {
        if (transform.position.y <= -4)
        {
            Destroy(gameObject, moveTime);
        }
    }
}
