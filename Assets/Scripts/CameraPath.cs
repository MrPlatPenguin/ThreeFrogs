using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public class CameraPath : MonoBehaviour
{
    [SerializeField] Transform pathPoint, playerPoint;

    [SerializeField] Camera cam;
    [SerializeField] float speed = 1;
    [SerializeField] float zoomSpeed = 1;

    public CameraTrackSegement[] cameraTrack;
    CameraTrackSegement CurrentSegment => cameraTrack[currentSegmentIndex];
    public Line[] blockerLines;

    int currentSegmentIndex = 0;

    [SerializeField] bool showPath = true;
    [SerializeField] bool showBounds = true;

    private void Start()
    {
        SetUpCamera();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        int[] indexsToSearch = GetJoiningPaths();
        Vector2 targetPos = GetClosestPoint(indexsToSearch) + CurrentSegment.Offset;
        pathPoint.position = Vector2.MoveTowards(pathPoint.position, targetPos, speed * Time.deltaTime);
        float targetCamSize = cameraTrack[currentSegmentIndex].CameraSizeOverride;
        cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, targetCamSize, zoomSpeed * Time.deltaTime);
    }

    int[] GetJoiningPaths()
    {
        List<int> indexs = new List<int>() { currentSegmentIndex };

        int nextIndex = Mathf.Clamp(currentSegmentIndex + 1, 0, cameraTrack.Length - 1);
        if (nextIndex != currentSegmentIndex)
            indexs.Insert(0, nextIndex);

        int prevIndex = Mathf.Clamp(currentSegmentIndex - 1, 0, cameraTrack.Length - 1);
        if (prevIndex != currentSegmentIndex)
            indexs.Add(prevIndex);
        return indexs.ToArray().Concat(CurrentSegment.ConnectedSegments).ToArray();
    }

    void SetUpCamera()
    {
        JumpCamera();
    }

    void JumpCamera()
    {
        int[] indexsToSearch = new int[cameraTrack.Length];
        for (int i = 0; i < indexsToSearch.Length; i++)
        {
            indexsToSearch[i] = cameraTrack.Length - 1 - i;
        }

        pathPoint.position = GetClosestPoint(indexsToSearch) + CurrentSegment.Offset;
        float targetCamSize = cameraTrack[currentSegmentIndex].CameraSizeOverride;
        cam.orthographicSize = targetCamSize;
    }

    private void OnDrawGizmos()
    {
        if (showPath)
        {
            foreach (CameraTrackSegement segment in cameraTrack)
            {
                segment.DrawPath();
            }
            foreach (Line line in blockerLines)
            {
                Gizmos.color = Color.red;
                line.DrawLine();
            }
        }
        if (showBounds)
        {
            foreach (CameraTrackSegement segment in cameraTrack)
            {
                segment.DrawBoarders();
            }
        }
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(pathPoint.position, 1f);
    }

    Vector2 GetClosestPoint(int[] searchArray)
    {
        float closestDistance = Mathf.Infinity;
        Vector2 closestPoint = playerPoint.position;
        int newIndex = currentSegmentIndex;
        foreach (int i in searchArray)
        {
            Vector2 playerPos = (Vector2)playerPoint.position;
            Vector2 pointOnTrack = cameraTrack[i].GetClosestPointOnSegment(playerPos);

            bool pointBlocked = false;

            //foreach (int blockerIndex in cameraTrack[i].Blockers)
            //{
            //    Line line = blockerLines[blockerIndex];
            foreach (Line line in blockerLines)
            {
                if (LinesIntersect(playerPos, pointOnTrack, line.PointA, line.PointB))
                {
                    pointBlocked = true;
                    break;
                }
            }
            if (pointBlocked)
                continue;

            float distanceToPoint = Vector2.Distance(pointOnTrack, playerPos);

            if (distanceToPoint < closestDistance)
            {
                closestDistance = distanceToPoint;
                closestPoint = pointOnTrack;
                newIndex = i;
            }
        }
        SetSegmentIndex(newIndex);
        return closestPoint;
    }

    void SetSegmentIndex(int index)
    {
        if (currentSegmentIndex == index)
            return;
        currentSegmentIndex = index;
    }

    // Checks if two lines intersect
    public bool LinesIntersect(Vector2 lineAStart, Vector2 lineAEnd, Vector2 lineBStart, Vector2 lineBEnd)
    {
        // Calculate directions of the lines
        Vector2 dirA = lineAEnd - lineAStart;
        Vector2 dirB = lineBEnd - lineBStart;

        // Calculate the determinant
        float det = (-dirB.x * dirA.y + dirA.x * dirB.y);

        // Parallel lines check (if determinant is close to 0, lines are parallel or coincident)
        if (Mathf.Abs(det) < 1e-10)
        {
            return false;
        }

        // Calculate how much to scale direction A and B to find intersection point
        Vector2 diff = lineAStart - lineBStart;
        float s = (-dirA.y * diff.x + dirA.x * diff.y) / det;
        float t = (dirB.x * diff.y - dirB.y * diff.x) / det;

        // If s and t are between 0 and 1, lines intersect within the line segments
        return s >= 0 && s <= 1 && t >= 0 && t <= 1;
    }
}

public enum CameraTrackDimension
{
    Point,
    One,
    Two
}

[System.Serializable]
public class Line
{
    public Vector2 PointA;
    public Vector2 PointB;

    public void DrawLine()
    {
        Gizmos.DrawLine(PointA, PointB);
    }
}

[System.Serializable]
public class CameraPreset
{
    const float DEFAULTCAMSIZE = 25;
    const float ASPECT = 16f / 9f;

    [SerializeField] float size = 0f;
    public Vector2 Offset = Vector2.zero;

    public float OrthographicSize => size > 0 ? size : DEFAULTCAMSIZE;
    public float CamWidth => OrthographicSize * ASPECT;
    public float CamHeight => OrthographicSize;
}

[System.Serializable]
public class CameraTrackSegement
{
    public CameraTrackDimension Dimensions;
    public Line line;
    public int[] ConnectedSegments;
    public Vector2 PointA => line.PointA;
    public Vector2 PointB => line.PointB;
    public Vector2 PointC => new Vector2(PointA.x, PointB.y);
    public Vector2 PointD => new Vector2(PointB.x, PointA.y);
    [SerializeField] int[] _blockedBy;
    public int[] Blockers => _blockedBy;
    [SerializeField] CameraPreset cameraData;

    public Vector2 Offset => new Vector2(cameraData.Offset.x * cameraData.CamWidth, cameraData.Offset.y * cameraData.CamHeight);
    public float CameraSizeOverride => cameraData.OrthographicSize;

    [SerializeField] bool showPath = true;
    [SerializeField] bool showBounds = true;

    public Vector2 GetClosestPointOnSegment(Vector2 origin)
    {
        Vector2 closestPoint;
        switch (Dimensions)
        {
            case CameraTrackDimension.Point:
                closestPoint = line.PointA;
                break;
            case CameraTrackDimension.One:
                closestPoint = ClosestPointOnLine(origin);
                break;
            case CameraTrackDimension.Two:
                closestPoint = ClosestPointInBox(origin);
                break;
            default:
                closestPoint = origin;
                break;
        }
        return closestPoint;
    }

    Vector2 ClosestPointOnLine(Vector2 origin)
    {
        Vector2 ab = PointB - PointA;
        Vector2 ap = origin - PointA;
        float proj = Vector2.Dot(ap, ab);
        float abLenSq = ab.sqrMagnitude;
        float t = proj / abLenSq;
        return Vector2.Lerp(PointA, PointB, t);
    }

    Vector2 ClosestPointInBox(Vector2 origin)
    {
        float[] xs = new float[2] { PointA.x, PointB.x};
        float[] ys = new float[2] { PointA.y, PointB.y};

        Vector2 closestPoint = new Vector2();

        closestPoint.x = Mathf.Clamp(origin.x, Mathf.Min(xs), Mathf.Max(xs));
        closestPoint.y = Mathf.Clamp(origin.y, Mathf.Min(ys), Mathf.Max(ys));

        return closestPoint;
    }

    public void DrawPath()
    {
        if (!showPath)
            return;
        switch (Dimensions)
        {
            case CameraTrackDimension.Point:
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(PointA, 0.5f);
                break;
            case CameraTrackDimension.One:
                Gizmos.color = Color.white;
                Gizmos.DrawLine(PointA, PointB);
                break;
            case CameraTrackDimension.Two:
                Gizmos.color = Color.white;
                DrawRect(PointA, PointB, PointC, PointD);
                break;
            default:
                break;
        }
    }

    public void DrawBoarders()
    {
        if (!showBounds)
            return;
        switch (Dimensions)
        {
            case CameraTrackDimension.Point:
                Gizmos.color = new Color(255, 69, 0);
                DrawAroundPoint(PointA);
                break;
            case CameraTrackDimension.One:
                Gizmos.color = new Color(255, 69, 0);
                DrawAroundLine(PointA, PointB);
                break;
            case CameraTrackDimension.Two:
                Gizmos.color = new Color(255, 69, 0);
                DrawAroundSquare(PointA, PointB, PointC, PointD);
                break;
            default:
                break;
        }
    }

    void DrawRect(Vector2 pointA, Vector2 pointB, Vector2 pointC, Vector2 pointD)
    {
        Gizmos.DrawLine(pointA, pointD);
        Gizmos.DrawLine(pointD, pointB);
        Gizmos.DrawLine(pointB, pointC);
        Gizmos.DrawLine(pointC, pointA);
    }

    void DrawAroundPoint(Vector2 originA)
    {
        float camWidth = cameraData.CamWidth;
        float camHeight = cameraData.CamHeight;

        originA += Offset;

        Vector2 BLa = originA + new Vector2(-camWidth, -camHeight);
        Vector2 TRa = originA + new Vector2(camWidth, camHeight);
        Vector2 BRa = originA + new Vector2(camWidth, -camHeight);
        Vector2 TLa = originA + new Vector2(-camWidth, camHeight);

        DrawRect(BLa, TRa, BRa, TLa);
    }

    void DrawAroundSquare(Vector2 originBottomLeft, Vector2 originTopRight, Vector2 originBottomRight, Vector2 originTopleft)
    {
        originBottomLeft += Offset;
        originTopRight += Offset;
        originBottomRight += Offset;
        originTopleft += Offset;

        Vector2 up = (originBottomLeft - originBottomRight).normalized;

        Vector2 right = (originTopRight - originBottomRight).normalized;

        up *= cameraData.CamHeight;
        right *= cameraData.CamWidth;
        Vector2 bottomLeft = originBottomLeft + (-right + up);
        Vector2 topRight = originTopRight + (right + -up);
        Vector2 bottomRight = originBottomRight + (-right + -up) ;
        Vector2 topLeft = originTopleft + (right + up);
        DrawRect(bottomLeft, topRight, bottomRight, topLeft);
    }

    void DrawAroundLine(Vector2 originA, Vector2 originB)
    {
        float camWidth = cameraData.CamWidth;
        float camHeight = cameraData.CamHeight;

        originA += Offset;
        originB += Offset;


        Vector2 BLa = originA + new Vector2(-camWidth, -camHeight);
        Vector2 TRa = originA + new Vector2(camWidth, camHeight);
        Vector2 BRa = originA + new Vector2(camWidth, -camHeight);
        Vector2 TLa = originA + new Vector2(-camWidth, camHeight);

        // The camera around point A
        //DrawRect(BLa, TRa, BRa, TLa);

        Vector2 BLb = originB + new Vector2(-camWidth, -camHeight);
        Vector2 TRb = originB + new Vector2(camWidth, camHeight);
        Vector2 BRb = originB + new Vector2(camWidth, -camHeight);
        Vector2 TLb = originB + new Vector2(-camWidth, camHeight);

        // The camera around point B
        //DrawRect(BLb, TRb, BRb, TLb);

        if ((originB.x >= originA.x && originB.y <= originA.y))
        {
            // A points
            Gizmos.DrawLine(BLa, TLa);
            Gizmos.DrawLine(TLa, TRa);
            // Connecting Points
            Gizmos.DrawLine(BLa, BLb);
            Gizmos.DrawLine(TRa, TRb);
            // B Points
            Gizmos.DrawLine(BLb, BRb);
            Gizmos.DrawLine(BRb, TRb);
        }
        else if ((originB.x <= originA.x && originB.y >= originA.y))
        {
            // A points
            Gizmos.DrawLine(BLa, BRa);
            Gizmos.DrawLine(BRa, TRa);
            // Connecting Points
            Gizmos.DrawLine(BLa, BLb);
            Gizmos.DrawLine(TRa, TRb);
            // B Points
            Gizmos.DrawLine(BLb, TLb);
            Gizmos.DrawLine(TLb, TRb);
        }

        else if ((originB.x >= originA.x && originB.y >= originA.y))
        {
            // A points
            Gizmos.DrawLine(BRa, BLa);
            Gizmos.DrawLine(BLa, TLa);
            // Connecting Points
            Gizmos.DrawLine(TLa, TLb);
            Gizmos.DrawLine(BRa, BRb);
            // B Points
            Gizmos.DrawLine(BRb, TRb);
            Gizmos.DrawLine(TRb, TLb);
        }

        else if ((originB.x <= originA.x && originB.y <= originA.y))
        {
            // A points
            Gizmos.DrawLine(BRa, TRa);
            Gizmos.DrawLine(TRa, TLa);
            // Connecting Points
            Gizmos.DrawLine(TLa, TLb);
            Gizmos.DrawLine(BRa, BRb);
            // B Points
            Gizmos.DrawLine(BRb, BLb);
            Gizmos.DrawLine(BLb, TLb);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CameraPath))]
public class CameraPathEditor : Editor
{
    CameraPath linkedObject;

    private void OnEnable()
    {
        linkedObject = target as CameraPath;
    }

    protected virtual void OnSceneGUI()
    {
        EditorGUI.BeginChangeCheck();

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.green;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 17;

        int segmentCount = linkedObject.cameraTrack.Length;
        Line[] newSegmentLines = new Line[segmentCount];

        for (int i = 0; i < newSegmentLines.Length; i++)
        {
            newSegmentLines[i] = new Line();
            CameraTrackSegement segement = linkedObject.cameraTrack[i];
            Vector2 labelPos;
            if (linkedObject.cameraTrack[i].Dimensions != CameraTrackDimension.Point)
            {
                labelPos = Vector2.Lerp(segement.PointA, segement.PointB, 0.5f);
            }
            else
            {
                labelPos = segement.PointA;
            }
            Handles.Label(labelPos, i.ToString(), style);
            newSegmentLines[i].PointA = Handles.PositionHandle(segement.line.PointA, Quaternion.identity);
            if (linkedObject.cameraTrack[i].Dimensions != CameraTrackDimension.Point)
                newSegmentLines[i].PointB = Handles.PositionHandle(segement.line.PointB, Quaternion.identity);

        }

        style.normal.textColor = Color.red;

        Line[] newBlockerLines = new Line[linkedObject.blockerLines.Length];
        for (int i = 0; i < newBlockerLines.Length; i++)
        {
            newBlockerLines[i] = new Line();
            newBlockerLines[i].PointA = Handles.PositionHandle(linkedObject.blockerLines[i].PointA, Quaternion.identity);
            newBlockerLines[i].PointB = Handles.PositionHandle(linkedObject.blockerLines[i].PointB, Quaternion.identity);
            Vector2 labelPos = Vector2.Lerp(newBlockerLines[i].PointA, newBlockerLines[i].PointB, 0.5f);
            Handles.Label(labelPos, i.ToString(), style);
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(linkedObject, "Change Look At Target Position");

            for (int i = 0; i < newBlockerLines.Length; i++)
            {
                linkedObject.blockerLines[i].PointA = Handles.PositionHandle(newBlockerLines[i].PointA, Quaternion.identity);
                linkedObject.blockerLines[i].PointB = Handles.PositionHandle(newBlockerLines[i].PointB, Quaternion.identity);
            }
            for (int i = 0; i < segmentCount; i++)
            {
                linkedObject.cameraTrack[i].line.PointA = Handles.PositionHandle(newSegmentLines[i].PointA, Quaternion.identity);
                if (linkedObject.cameraTrack[i].Dimensions != CameraTrackDimension.Point)
                    linkedObject.cameraTrack[i].line.PointB = Handles.PositionHandle(newSegmentLines[i].PointB, Quaternion.identity);
            }
        }
    }
}
#endif // UNITY_EDITOR