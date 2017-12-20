using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    float ballVelocity = 550.0f;
    float jumpVelocity = 100.0f;
    float moveVelocity = 10.0f;
	float bumpVelocity = 25.0f;

    bool moveToSpawnPoint = false;
    public bool newBall = true;
    
    Rigidbody rb;
    SphereCollider sc;
    public Material whiteMaterial;
    public Material greenMaterial;

	public float yValue = -10;
	public int bumpcounter = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
        GameManager.instance.balls.Add(this);
        if (newBall)
        {
            GetComponent<MeshRenderer>().material = greenMaterial;
            MoveNewBall();
        }
    }

    public void MoveBall(Vector3 direction)
    {
        rb.isKinematic = false;
        rb.AddForce(new Vector3(ballVelocity * direction.x, ballVelocity * direction.y, 0.0f));
    }

    void MoveNewBall()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(new Vector3(0.0f, jumpVelocity, 0.0f));
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "Floor")
        {
			StopBall ();
            CheckForSpawnPoint();
            GameManager.instance.counter++;
        }

		if (other.gameObject.tag == "Wall") {
			if (bumpcounter == 5) {
				rb.AddForce (0.0f, -bumpVelocity, 0.0f);
				yValue = -10;
			} else if (Mathf.Abs (transform.position.y - yValue) < Mathf.Epsilon) {
				bumpcounter++;
			} else {
				yValue = transform.position.y;
				bumpcounter = 0;
			}
		}
    }

	void StopBall(){
		rb.velocity = Vector3.zero;
		rb.isKinematic = true;
		rb.useGravity = false;
		transform.position = new Vector3 (transform.position.x, -4.7f, transform.position.z);
		transform.rotation = Quaternion.identity;
	}

    void CheckForSpawnPoint()
    {
        if(GameManager.instance.ballSpawnSet)
        {
            moveToSpawnPoint = true;
        }
        else
        {
            GameManager.instance.SetBallSpawn(this);
        }
    }

    void Update()
    {
		if (GameManager.instance.ballSpawnSet) {
			if (transform.position == GameManager.instance.ballSpawn.transform.position) {
				moveToSpawnPoint = false;
			}
		}

        if (moveToSpawnPoint)
        {
            transform.position = Vector3.MoveTowards(transform.position, GameManager.instance.ballSpawn.transform.position, moveVelocity * Time.deltaTime);
        }

		if(transform.position.y <= -4 && newBall)
        {
			newBall = false;
			sc.enabled = true;
			GetComponent<MeshRenderer>().material = whiteMaterial;
        }

		if (GameManager.instance.ballsMoving == false) {
			StopBall ();
		}
    }
}
