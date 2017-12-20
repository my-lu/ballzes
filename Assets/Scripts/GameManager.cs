using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	public static GameManager instance = null;

	float nextLevelDelay = 0.1f;
	float nextBallDelay = 0.15f;
	float gameDelay = 0.5f;

	public Ball mainBall;
    public Ball ballSpawn;
    public bool ballSpawnSet;
	public bool ballsMoving;
	public bool squaresMoving;
	public int counter = 0;

	bool checkTimer;
	float moveStartedTimer;
	public GameObject speedUI;

	int level = 0;

	public DataControl dataController;

	int ballNumber;

	public List<Spawner> spawners = new List<Spawner>();
	public List<Square> squares = new List<Square>();
	public List<Ring> rings = new List<Ring>();
	public List<Ball> balls = new List<Ball>();

	public GameObject pointer;

	Vector3 startDrag = new Vector3();
	Vector2 pointerOffset;
	bool inDrag;
	float rotationSpeed = 25.0f;

    public GameObject levelUI;
    public GameObject bestLevelUI;
    public GameObject ballNumberUI;
    public GameObject header;
	public GameObject pauseMenu;
	public GameObject gameOverMenu;
	bool inPauseMenu;
	bool inGameOverMenu;

	public Canvas canvas;
	RectTransform canvasRect;

	Vector3 originalState = new Vector3(0.0f, -4.7f, 0.0f);

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if(instance != this)
			Destroy(gameObject);

		Physics.IgnoreLayerCollision(8, 8);
	}

	void Start()
	{
		dataController = FindObjectOfType<DataControl> ();
		ballSpawnSet = true;
		ballsMoving = false;
		squaresMoving = false;
		pointerOffset = pointer.GetComponent<RectTransform>().anchoredPosition - PointerPosition();
		pointer.SetActive(false);
		NextLevel();
	}

	void Update()
	{
		if (checkTimer) {
			if (moveStartedTimer >= 10.0f) {
				speedUI.SetActive (true);
				checkTimer = false;
			} else {
				moveStartedTimer += Time.deltaTime;
			}
		}

		bool inputDown = false;
		bool inputUp = false;

        Vector3 dragTemp;

#if UNITY_STANDALONE || UNITY_WEBPLAYER

        inputDown = Input.GetMouseButtonDown(0);
		inputUp = Input.GetMouseButtonUp(0);

        dragTemp = Input.mousePosition;

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		Touch myTouch = new Touch();
		if(Input.touchCount > 0)
		{
			myTouch = Input.touches[0];
			inputDown = myTouch.phase == TouchPhase.Began;
			inputUp = myTouch.phase == TouchPhase.Ended;
		}

		if (inDrag == false || Input.touchCount == 0)
		{
			pointer.SetActive (false);
			startDrag = Vector3.zero;
		}

		dragTemp = myTouch.position;

#endif

		if (inputDown && squaresMoving == false && ballsMoving == false && inPauseMenu == false && inGameOverMenu == false)
		{
			startDrag = dragTemp;
			inDrag = true;
		}

		if(inDrag && squaresMoving == false && ballsMoving == false && inPauseMenu == false && inGameOverMenu == false)
		{
			pointer.GetComponent<RectTransform>().anchoredPosition = PointerPosition() + pointerOffset;

			Vector3 direction = dragTemp - startDrag;
			if (direction.y > 0)
			{
				pointer.SetActive(false);
				return;
			}

			float ratio = direction.magnitude / 25;
			if(ratio < 1)
			{
				return;
			}
			else if(ratio > 4)
			{
				ratio = 4;
			}

			pointer.GetComponent<RectTransform> ().localScale = new Vector3 (1.0f, ratio, 1.0f);
			RotatePointer(direction);
			pointer.SetActive(true);
		}

		if (inputUp && squaresMoving == false && ballsMoving == false && inPauseMenu == false && inGameOverMenu == false)
		{
			inDrag = false;
			if(dragTemp.y < startDrag.y)
			{
				StartMove(new Vector3(startDrag.x - dragTemp.x, startDrag.y - dragTemp.y));
			}

			startDrag = Vector3.zero;
			pointer.SetActive(false);
		}

		if (ballsMoving == true && counter >= balls.Count)
		{
			ballsMoving = false;
			instance.NextLevel();
		}
	}

    Vector2 PointerPosition()
    {
        Vector2 vpPosition = Camera.main.WorldToViewportPoint(GameManager.instance.transform.position);
        return new Vector2(vpPosition.x * canvas.GetComponent<RectTransform>().sizeDelta.x - canvas.GetComponent<RectTransform>().sizeDelta.x * 0.5f, vpPosition.y * canvas.GetComponent<RectTransform>().sizeDelta.y - canvas.GetComponent<RectTransform>().sizeDelta.y * 0.5f);
    }

    void RotatePointer(Vector3 direction)
	{
		float angle = -Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		pointer.GetComponent<RectTransform>().rotation = Quaternion.Slerp(pointer.GetComponent<RectTransform>().rotation, Quaternion.Euler(0, 0, 90.0f - angle), rotationSpeed * Time.deltaTime);
    }

    public void NextLevel()
	{
		checkTimer = false;
		Time.timeScale = 1;
		speedUI.SetActive (false);

		squaresMoving = true;

		level++;
		dataController.SubmitPlayerScore (level);

		instance.transform.position = ballSpawn.transform.position;
		ballNumber = balls.Count;

        UpdateUI();
        UpdateNumberUI();

		SpawnNextLevel();
		StartCoroutine(MoveSquares());
	}

	void SpawnNextLevel()
	{
		RandomizeSpawners();
		int number = Random.Range(1, 6);

		if (level != 1)
			spawners[0].SpawnNewBall();

		for (int i = 1; i <= number; i++)
		{
			if (i >= spawners.Count)
			{
				return;
			}
			spawners[i].SpawnSquare(level);
		}
	}

	IEnumerator MoveSquares()
	{
		yield return new WaitForSeconds(nextLevelDelay);

		if (squares.Count == 0)
		{
			yield return new WaitForSeconds(nextLevelDelay);
		}

		for (int i = 0; i < squares.Count; i++)
		{
			squares[i].MoveSquare();
		}

		for (int i = 0; i < rings.Count; i++)
		{
			rings[i].MoveSquare();
		}

		yield return new WaitForSeconds(squares[0].moveTime);

		squaresMoving = false;
    }

    public void StartMove(Vector3 direction)
    {
        counter = 0;
		moveStartedTimer = 0;
		checkTimer = true;
        StartCoroutine(MoveBalls(direction.normalized));
    }

    IEnumerator MoveBalls(Vector3 direction)
	{
		ballsMoving = true;
		ballSpawn = null;
		ballSpawnSet = false;
		ballNumber = balls.Count;
		UpdateNumberUI();

		yield return new WaitForSeconds(nextBallDelay);

		for (int i = balls.Count - 1; i >= 0; i--)
		{
			if(balls[i].newBall == false)
			{
				balls[i].MoveBall(direction);
				ballNumber = i;
				UpdateNumberUI();
			}
			yield return new WaitForSeconds(nextBallDelay);
		}
	}

	void UpdateUI()
	{
		levelUI.GetComponent<Text>().text = level.ToString();
		bestLevelUI.GetComponent<Text>().text = dataController.getHighestPlayerScore().ToString();
	}

	void UpdateNumberUI()
	{
		if(ballNumber == 0)
		{
			ballNumberUI.GetComponent<TextMesh>().text = "";
		}
		else
		{
			ballNumberUI.GetComponent<TextMesh>().text = "x" + ballNumber.ToString();
		}
	}

	void RandomizeSpawners()
	{
		for (int i = 0; i < spawners.Count; i++)
		{
			Spawner temp = spawners[i];
			int randomIndex = Random.Range(i, spawners.Count);
			spawners[i] = spawners[randomIndex];
			spawners[randomIndex] = temp;
		}
	}

	public void LoseGame()
	{
		Invoke("GameState", gameDelay);
	}

	void GameState()
	{
		OpenGOMenu();
	}

	public void SetBallSpawn(Ball ball)
	{
		ballSpawnSet = true;
		ballSpawn = ball;
	}

	public void OpenPauseMenu()
	{
		header.SetActive(false);
		pauseMenu.SetActive(true);
		inPauseMenu = true;
		Time.timeScale = 0;
	}

	public void ClosePauseMenu()
	{
		header.SetActive(true);
		pauseMenu.SetActive(false);
		inPauseMenu = false;
		Time.timeScale = 1;
	}

	public void RestartGame()
	{
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
	}

	public void OpenGOMenu()
	{
		gameOverMenu.SetActive(true);
		inGameOverMenu = true;
		Time.timeScale = 0;
	}

	public void CloseGOMenu()
	{
		gameOverMenu.SetActive(false);
		inGameOverMenu = false;
		Time.timeScale = 1;
	}

	public void SpeedUpGame()
	{
		Time.timeScale = 2;
		speedUI.SetActive (false);
	}
}
