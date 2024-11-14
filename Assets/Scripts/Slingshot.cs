using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class Slingshot : MonoBehaviour
{
    public LineRenderer[] lineRenderers;
    public Transform[] stripPositions;
    public Transform center;
    public Transform idlePosition;

    public float maxLength;
    public float bottomBoundary;
    public Vector3 currentPosition;

    bool isMouseDown;
    bool birdReleased = false;
    public GameObject birdPrefab;
    public float birdPositionOffset;
    Rigidbody2D bird;
    Collider2D birdCollider;
    public float force;

    public TextMeshProUGUI scoreText; // 점수를 표시하기 위한 UI 텍스트
    public GameObject gameClearPopup; // 게임 클리어 팝업
    public TextMeshProUGUI gameClearText; // 게임 클리어 팝업의 텍스트
    private int score = 100; // 플레이어의 초기 점수는 100점
    private int birdsShot = 0; // 던진 새의 수
    private float startTime; // 게임 시작 시간

    // Start는 첫 프레임 업데이트 전에 호출됩니다
    void Start()
    {
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;
        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);

        CreateBird();
        UpdateScoreText();
        startTime = Time.time;

        StartCoroutine(CheckTargetsRoutine());
    }

    void CreateBird()
    {
        bird = Instantiate(birdPrefab).GetComponent<Rigidbody2D>();
        birdCollider = bird.GetComponent<Collider2D>();
        birdCollider.enabled = false;

        bird.isKinematic = true;
        birdReleased = false;

        ResetStrips();
    }

    // 매 프레임마다 호출됩니다
    void Update()
    {
        if (isMouseDown)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;

            currentPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            currentPosition = center.position + Vector3.ClampMagnitude(currentPosition - center.position, maxLength);

            currentPosition = ClampBoundary(currentPosition);
            SetStrips(currentPosition);

            if (birdCollider)
            {
                birdCollider.enabled = true;
            }
        }
        else if (!birdReleased)
        {
            ResetStrips();
        }
    }

    private void OnMouseDown()
    {
        isMouseDown = true;
    }

    private void OnMouseUp()
    {
        isMouseDown = false;
        Shoot();
    }

    void Shoot()
    {
        bird.isKinematic = false;
        Vector3 direction = (center.position - currentPosition).normalized;
        Vector3 birdForce = direction * force;
        bird.velocity = birdForce;

        birdReleased = true;

        Invoke("DestroyBird", 2); // 새를 날린 후 2초 뒤에 제거

        birdsShot++;
        score -= 10; // 새를 날릴 때마다 점수 10점 감소
        UpdateScoreText();
        Invoke("CreateBird", 2.5f);
    }

    void DestroyBird()
    {
        if (bird != null)
        {
            Destroy(bird.gameObject);
            bird = null; // Prevent accessing the destroyed bird
        }
    }

    void ResetStrips()
    {
        currentPosition = idlePosition.position;
        SetStrips(currentPosition);
    }

    void SetStrips(Vector3 position)
    {
        lineRenderers[0].SetPosition(1, position);
        lineRenderers[1].SetPosition(1, position);

        if (bird)
        {
            Vector3 dir = position - center.position;
            bird.transform.position = position + dir.normalized * birdPositionOffset;
            bird.transform.right = -dir.normalized;
        }
    }

    Vector3 ClampBoundary(Vector3 vector)
    {
        vector.y = Mathf.Clamp(vector.y, bottomBoundary, 1000);
        return vector;
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
        CheckGameClear();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    public void CheckGameClear()
    {
        // "Target" 태그를 가진 오브젝트가 있는지 확인하여 게임 클리어 여부를 판단합니다
        if (GameObject.FindGameObjectsWithTag("Target").Length == 0)
        {
            ShowGameClearPopup();
        }
    }

    void ShowGameClearPopup()
    {
        Debug.Log("Game Clear! Showing popup...");
        float elapsedTime = Time.time - startTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);

        gameClearPopup.SetActive(true);
        gameClearText.text = $"Game Clear!\n\nBirds Shot: {birdsShot}\nScore: {score}\nTime: {minutes}m {seconds}s";
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬 다시 로드
    }

    // 타겟 오브젝트 수를 2초마다 체크하는 코루틴
    IEnumerator CheckTargetsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            int targetCount = GameObject.FindGameObjectsWithTag("Target").Length;
            Debug.Log($"Number of targets remaining: {targetCount}");
            if (targetCount == 0)
            {
                ShowGameClearPopup();
                yield break; // 모든 타겟이 없어졌다면 코루틴 종료
            }
        }
    }
}