using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeRenderer : MonoBehaviour
{
    public SnakeGameManager gm;
    
    [Header("Rendering")]
    private ComputeShader cs;
    public ComputeShader highQuality;
    public ComputeShader lowQuality;
    //public float resolution;
    public RenderTexture renderTexture;

    [Header("Aesthetic")]
    public float snakeSoftness;

    private void Awake()
    {
        cs = highQuality;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (cs == highQuality) cs = lowQuality;
            else if (cs == lowQuality) cs = highQuality;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (renderTexture == null || renderTexture.width != source.width || renderTexture.height != source.height)
        {
            //int trueRes = (int)Mathf.Pow(2, resolution);
            renderTexture = new RenderTexture(source.width, source.height, 0);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }
        Graphics.CopyTexture(source, renderTexture);

        cs.SetFloat("time", Time.time);

        cs.SetFloat("resolutionX", renderTexture.width);
        cs.SetFloat("resolutionY", renderTexture.height);
        cs.SetFloat("softness", snakeSoftness);

        cs.SetVector("lineStart", gm.snake.transform.position);
        cs.SetVector("lineEnd", gm.snake.transform.position + gm.snake.direction);

        List<IKPoint> p = gm.snakeLimb.points;
        Circle[] segments = new Circle[p.Count];
        for (int i = 0; i < p.Count; i++)
        {
            segments[i] = new Circle(p[i].position, p[i].jointLength);
        }
        ComputeBuffer snakeBuffer = new ComputeBuffer(p.Count, sizeof(float) * 4);
        snakeBuffer.SetData(segments);
        cs.SetBuffer(0, "segments", snakeBuffer);
        cs.SetFloat("numSegments", p.Count);

        Circle[] apples = new Circle[gm.apples.Count];
        for (int i = 0; i < gm.apples.Count; i++)
        {
            apples[i] = new Circle(gm.apples[i], gm.appleRadius);
        }
        ComputeBuffer appleBuffer = new ComputeBuffer(gm.apples.Count, sizeof(float) * 4);
        appleBuffer.SetData(apples);
        cs.SetBuffer(0, "apples", appleBuffer);
        cs.SetFloat("numApples", apples.Length);

        cs.SetFloat("minX", gm.minX);
        cs.SetFloat("maxX", gm.maxX);
        cs.SetFloat("minY", gm.minY);
        cs.SetFloat("maxY", gm.maxY);

        cs.SetTexture(0, "Result", renderTexture);

        cs.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);

        Graphics.Blit(renderTexture, destination);

        snakeBuffer.Dispose();
        appleBuffer.Dispose();
    }
}

struct Circle
{
    public Vector3 position;
    public float radius;

    public Circle(Vector3 position, float radius)
    {
        this.position = position;
        this.radius = radius;
    }
}
