using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    Vector3 location;
    float moveSpeed = 1.0f;
    public float moveTime = 1.0f;

    int health;

    public GameObject particles;
    public TextMesh healthText;

    void Start()
    {
        location = transform.position;
        GameManager.instance.squares.Add(this);
    }

    void Update()
    {
        if(transform.position != location)
        {
            transform.position = Vector3.MoveTowards(transform.position, location, moveSpeed * Time.deltaTime);
        }

        if (health <= 0)
        {
            GameManager.instance.squares.Remove(this);
            Instantiate(particles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        CheckLoseGame();
    }

    void OnCollisionEnter(Collision other)
    {
        health--;
        UpdateUI();
    }

    public void MoveSquare()
    {
        location.y -= 1;
    }

    public void SetHealth(int hp)
    {
        health = hp;
        UpdateUI();
    }

    void UpdateUI()
    {
        healthText.text = health.ToString();
    }
    
    void CheckLoseGame()
    {
        if(transform.position.y <= -4)
        {
            GameManager.instance.LoseGame();
        }
    }
}
