using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public GameObject squarePrefab;
    public GameObject newBallPrefab;

    GameObject square;

    void Start()
    {
        GameManager.instance.spawners.Add(this);
    }

    public void SpawnSquare(int level)
    {
        square = Instantiate(squarePrefab, transform) as GameObject;
        int health = Random.Range(1, 3) * level;
        square.GetComponent<Square>().SetHealth(health);
    }

    public void SpawnNewBall()
    {
        Instantiate(newBallPrefab, transform);
    }
}
