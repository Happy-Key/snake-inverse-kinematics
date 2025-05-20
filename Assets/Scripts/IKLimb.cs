using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKLimb : MonoBehaviour
{
    public List<IKPoint> points;
    public int iterations;
    public float angleSnappiness;
    public float distMoESqrd;

    public bool independent;
    public bool isZLocked;

    public void FABRIK(Vector3 target)
    {
        Vector3 temp = points[points.Count - 1].position;
        for (int i = 0; i < iterations; i++)
        {
            bool areAnglesSatisfied = true;
            for (int j = 1; j < points.Count; j++)
            {
                if (points[j].isFixed) continue;
                Vector3 offset = (points[j].position - points[j - 1].position).normalized;
                Vector3 prevOffset;
                if (j != 1) prevOffset = (points[j - 1].position - points[j - 2].position).normalized;
                else prevOffset = (points[j - 1].position - target).normalized;
                float angle = Vector3.Angle(prevOffset, offset);
                if (angle > points[j - 1].angularConstraint)
                {
                    areAnglesSatisfied = false;
                    offset = Vector3.Slerp(offset, prevOffset, Mathf.Min(points[j - 1].angularConstraint, (angle / points[j - 1].angularConstraint) * angleSnappiness) / angle);
                }
                points[j].position = points[j - 1].position + offset * (points[j - 1].jointLength + points[j].jointLength) / 2f;
                if(isZLocked) points[j].position.z = 0f;
            }
            for (int j = points.Count - 2; j >= 0; j--)
            {
                if (points[j].isFixed) continue;
                Vector3 offset = (points[j].position - points[j + 1].position).normalized;
                if (j < points.Count - 2)
                {
                    Vector3 prevOffset = (points[j + 1].position - points[j + 2].position).normalized;
                    float angle = Vector3.Angle(prevOffset, offset);
                    if (angle > points[j].angularConstraint)
                    {
                        areAnglesSatisfied = false;
                        offset = Vector3.Slerp(offset, prevOffset, Mathf.Min(points[j].angularConstraint, (angle / points[j].angularConstraint) *  angleSnappiness) / angle);
                    }
                }
                points[j].position = points[j + 1].position + offset * (points[j].jointLength + points[j+1].jointLength) / 2f;
                if (isZLocked) points[j].position.z = 0f;
            }
            if (Vector3.SqrMagnitude(temp - points[points.Count - 1].position) < distMoESqrd && areAnglesSatisfied) break;
            temp = points[points.Count - 1].position;
        }
    }

    private void FixedUpdate()
    {
        points[0].position = transform.position;
        if (independent) FABRIK(2 * points[0].position - points[1].position);
    }

    public void SetRootPosition(Vector3 pos)
    {
        points[points.Count - 1].position = pos;
    } 

    public void AddPoint(IKPoint p)
    {
        points.Add(p);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawSphere(points[i].position, 0.1f);
            Gizmos.DrawWireSphere(points[i].position, points[i].jointLength / 2f);
            if (i != 0) Gizmos.DrawLine(points[i - 1].position, points[i].position);
        }
    }
}

[System.Serializable]
public class IKPoint
{
    public bool isFixed;
    public float jointLength;
    public Vector3 position;
    public float angularConstraint;

    public IKPoint(IKPoint p)
    {
        isFixed = p.isFixed;
        jointLength = p.jointLength;
        position = p.position;
        angularConstraint = p.angularConstraint;
    }
}