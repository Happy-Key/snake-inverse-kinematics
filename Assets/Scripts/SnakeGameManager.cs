using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeGameManager : MonoBehaviour
{
    [System.NonSerialized]
    public List<Vector3> apples = new List<Vector3>();

    [Header("Game Settings")]
    public int maxApples;
    public float appleRadius;
    public float leeway;
    public float defaultSegmentSize;
    public float growthSpeed;

    [Header("Board Settings")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    [Header("Other")]
    public int score;
    public SnakeController snake;
    [System.NonSerialized]
    public IKLimb snakeLimb;
    private bool playing = true;

    private void Update()
    {
        if (playing) GameLoop();
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void GameLoop()
    {
        snake.Movement();
        Vector3 head = snakeLimb.points[0].position;
        if (head.x > maxX || head.x < minX || head.y > maxY || head.y < minY)
        { 
            Debug.Log("Death: Out of Bounds, Score: " + score);
            Lose();
        }
        for (int i = 1; i < snakeLimb.points.Count; i++)
        {
            if ((head - snakeLimb.points[i].position).magnitude < (snakeLimb.points[0].jointLength + snakeLimb.points[i].jointLength) / 2f - leeway)
            {
                Debug.Log("Death: Self-collision, Score: " + score);
                Lose();
            }
        }
        for (int i = 0; i < apples.Count; i++)
        {
            Vector3 a = apples[i];
            if ((head - a).sqrMagnitude < Mathf.Pow(snakeLimb.points[0].jointLength / 2f + appleRadius, 2))
            {
                EatApple(i);
            }
        }
        while (apples.Count < maxApples)
        {
            apples.Add(new Vector3(Random.Range(minX + 1, maxX - 1), Random.Range(minY + 1, maxY - 1), 0f));
        }
    }

    void EatApple(int index)
    {
        apples.RemoveAt(index);
        foreach (IKPoint p in snakeLimb.points)
        {
            p.jointLength += growthSpeed;
        }
        IKPoint point = new IKPoint(snakeLimb.points[snakeLimb.points.Count - 1]);
        point.jointLength = defaultSegmentSize;
        point.position.x += 0.01f;
        snakeLimb.AddPoint(point);

        score++;
    }

    public void Lose()
    {
        playing = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Vector3 a in apples)
        {
            Gizmos.DrawSphere(a, 0.3f);
        }
    }

    private void Start()
    {
        snakeLimb = snake.ikl;
    }
}