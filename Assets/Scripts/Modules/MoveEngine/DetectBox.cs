using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public enum Direction
{
    Up = 0,
    Right,
    Down,
    Left,
    RU,
    RD,
    LD,
    LU
}

public static class Vector2Ext
{
    public static Vector2 right = new Vector2(1, 0);
    public static Vector2 Toward(this Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return Vector2.up;
            case Direction.Right:
                return Vector2.right;
            case Direction.Down:
                return Vector2.down;
            case Direction.Left:
                return Vector2.left;
            case Direction.RU:
                return new Vector2(1, 1).normalized;
            case Direction.RD:
                return new Vector2(1, -1).normalized;
            case Direction.LD:
                return new Vector2(-1, -1).normalized;
            case Direction.LU:
                return new Vector2(-1, 1).normalized;
        }
        return Vector2.zero;
    }
    public static Direction Toward(this Vector2 vector)
    {
        float angle = Vector2.SignedAngle(right, vector);
        float curAngle = -157.5f;
        for (int i = 0; i <= 8; i++)
        {
            if (angle < curAngle + i * 45f)
            {
                switch (i)
                {
                    case 0:
                    case 8:
                        return Direction.Left;
                    case 1:
                        return Direction.LD;
                    case 2:
                        return Direction.Down;
                    case 3:
                        return Direction.RD;
                    case 4:
                        return Direction.Right;
                    case 5:
                        return Direction.RU;
                    case 6:
                        return Direction.Up;
                    case 7:
                        return Direction.LU;
                }
            }
        }
        return Direction.Right;
    }
}
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class DetectBox : MonoBehaviour
{
    private float detectbox_distanceOffset = 0.05f;
    public float distance
    {
        get => detectbox_distanceOffset;
        set
        {
            if (detectbox_distanceOffset != value)
            {
                detectbox_distanceOffset = value;
                UpdateCastPara();
            }
        }
    }
    private float detectbox_widthOffset = -0.02f;
    public float width
    {
        get => detectbox_widthOffset;
        set
        {
            if (detectbox_widthOffset != value)
            {
                detectbox_widthOffset = value;
                UpdateCastPara();
            }
        }
    }
    private float obliside_distanceOffset = 0.05f;
    public float obliside_Distance
    {
        get => obliside_distanceOffset;
        set
        {
            if (obliside_distanceOffset != value)
            {
                obliside_distanceOffset = value;
                UpdateCastPara();
            }
        }
    }
    private float obliside_length = 0.1f;
    public float length
    {
        get => obliside_length;
        set
        {
            if (obliside_length != value)
            {
                obliside_length = value;
                UpdateCastPara();
            }
        }
    }
    private Vector2[] startPoint;
    private Vector2[] endPoint;
    private Vector2[] castDir = new Vector2[4]{
            new Vector2(1,0),
            new Vector2(0,-1),
            new Vector2(-1,0),
            new Vector2(0,1)
        };
    private Vector2[] rectCenter;
    private Vector2 rectBoxSize => new Vector2(obliside_length, obliside_length);
    private float[] rayLength => new float[] { detectbox_widthOffset + cder.bounds.size.x, detectbox_widthOffset + cder.bounds.size.y, detectbox_widthOffset + cder.bounds.size.x, detectbox_widthOffset + cder.bounds.size.y };
    private BoxCollider2D cder;
    private Rigidbody2D rb;
    private float[] velocityOffset = new float[4];
    public float[] distance_offset { get => velocityOffset; }
    private void UpdateCastPara()
    {
        if (cder == null)
            cder = GetComponent<BoxCollider2D>();
        Vector2 colliderSize = cder.bounds.size;
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        velocityOffset[0] = (rb.velocity.y <= 0 ? 0 : rb.velocity.y * Time.fixedDeltaTime) + detectbox_distanceOffset;
        velocityOffset[1] = (rb.velocity.x <= 0 ? 0 : rb.velocity.x * Time.fixedDeltaTime) + detectbox_distanceOffset;
        velocityOffset[2] = (rb.velocity.y >= 0 ? 0 : -rb.velocity.y * Time.fixedDeltaTime) + detectbox_distanceOffset;
        velocityOffset[3] = (rb.velocity.x >= 0 ? 0 : -rb.velocity.x * Time.fixedDeltaTime) + detectbox_distanceOffset;

        startPoint = new Vector2[4]{
            new Vector2(transform.position.x-colliderSize.x/2-detectbox_widthOffset,
                transform.position.y+colliderSize.y/2+velocityOffset[0]),
            new Vector2(transform.position.x+colliderSize.x/2+velocityOffset[1],
                transform.position.y+colliderSize.y/2+detectbox_widthOffset),
            new Vector2(transform.position.x+colliderSize.x/2+detectbox_widthOffset,
                transform.position.y-colliderSize.y/2-velocityOffset[2]),
            new Vector2(transform.position.x-colliderSize.x/2-velocityOffset[3],
                transform.position.y-colliderSize.y/2-detectbox_widthOffset)
        };
        endPoint = new Vector2[4]{
            new Vector2(transform.position.x+colliderSize.x/2+detectbox_widthOffset,
                transform.position.y+colliderSize.y/2+velocityOffset[0]),
            new Vector2(transform.position.x+colliderSize.x/2+velocityOffset[1],
                transform.position.y-colliderSize.y/2-detectbox_widthOffset),
            new Vector2(transform.position.x-colliderSize.x/2-detectbox_widthOffset,
                transform.position.y-colliderSize.y/2-velocityOffset[2]),
            new Vector2(transform.position.x-colliderSize.x/2-velocityOffset[3],
                transform.position.y+colliderSize.y/2+detectbox_widthOffset)
        };
        rectCenter = new Vector2[4]{
            new Vector2(transform.position.x+colliderSize.x/2+obliside_distanceOffset,
                transform.position.y+colliderSize.y/2+obliside_distanceOffset),
            new Vector2(transform.position.x+colliderSize.x/2+obliside_distanceOffset,
                transform.position.y-colliderSize.y/2-obliside_distanceOffset),
            new Vector2(transform.position.x-colliderSize.x/2-obliside_distanceOffset,
                transform.position.y-colliderSize.y/2-obliside_distanceOffset),
            new Vector2(transform.position.x-colliderSize.x/2-obliside_distanceOffset,
                transform.position.y+colliderSize.y/2+obliside_distanceOffset)
        };
    }
    public void Init()
    {
        UpdateCastPara();
        dic = new Dictionary<Direction, List<Collider2D>>();
        enterEvent = new Dictionary<Direction, UnityAction<Vector2>>();
        exitEvent = new Dictionary<Direction, UnityAction<Vector2>>();
        for (int i = (int)Direction.Up; i < 8; i++)
        {
            dic.Add((Direction)i, new List<Collider2D>());
            enterEvent.Add((Direction)i, v => { });
            exitEvent.Add((Direction)i, v => { });
        }

    }
    private void RaycastDetct()
    {
        UpdateCastPara();
        bool[] isTouch = new bool[8];
        for (int i = (int)Direction.Up; i < 8; i++)
        {
            isTouch[i] = dic[(Direction)i].Count > 0;
            dic[(Direction)i].Clear();
        }
        for (int i = (int)Direction.Up; i <= (int)Direction.Left; i++)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint[i], castDir[i], rayLength[i], LayerMask.GetMask("Ground"));
            foreach (var e in hits)
            {
                if (e.collider != cder && !e.collider.isTrigger)
                {
                    dic[(Direction)i].Add(e.collider);
                }
            }
            if (dic[(Direction)i].Count > 0 && !isTouch[i])
            {
                enterEvent[(Direction)i].Invoke(dic[(Direction)i][0].ClosestPoint((startPoint[i] + endPoint[i]) / 2 + velocityOffset[i] * ((Direction)i).Toward()));
            }
            else if (dic[(Direction)i].Count == 0 && isTouch[i])
                exitEvent[(Direction)i].Invoke(rb.velocity);
        }
        int startIndex = (int)Direction.RU;
        for (int i = (int)Direction.RU; i <= (int)Direction.LU; i++)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll(rectCenter[i - startIndex], rectBoxSize, 0, Vector2.zero);
            foreach (var e in hits)
            {
                if (e.collider != cder)
                {
                    dic[(Direction)i].Add(e.collider);
                }
            }
        }
    }
    private void DrawRect(Vector2 center, float length)
    {
        Gizmos.DrawLine(new Vector3(center.x - length / 2, center.y + length / 2, transform.position.z),
            new Vector3(center.x + length / 2, center.y + length / 2, transform.position.z));
        Gizmos.DrawLine(new Vector3(center.x + length / 2, center.y + length / 2, transform.position.z),
            new Vector3(center.x + length / 2, center.y - length / 2, transform.position.z));
        Gizmos.DrawLine(new Vector3(center.x + length / 2, center.y - length / 2, transform.position.z),
            new Vector3(center.x - length / 2, center.y - length / 2, transform.position.z));
        Gizmos.DrawLine(new Vector3(center.x - length / 2, center.y - length / 2, transform.position.z),
            new Vector3(center.x - length / 2, center.y + length / 2, transform.position.z));
    }
    private void OnDrawGizmos()
    {
        if (dic == null)
        {
            Init();
        }
        for (int i = 0; i < 4; i++)
        {
            if (Application.isPlaying)
                Gizmos.color = dic[(Direction)i].Count > 0 ? Color.red : Color.green;
            else
                Gizmos.color = Color.cyan;
            Gizmos.DrawLine(startPoint[i], endPoint[i]);
        }
        for (int i = 0; i < 4; i++)
        {
            if (Application.isPlaying)
                Gizmos.color = dic[(Direction)(i + 4)].Count > 0 ? Color.red : Color.green;
            else
                Gizmos.color = Color.cyan;
            DrawRect(rectCenter[i], obliside_length);
        }
    }
    Dictionary<Direction, List<Collider2D>> dic;
    public Dictionary<Direction, UnityAction<Vector2>> enterEvent;
    public Dictionary<Direction, UnityAction<Vector2>> exitEvent;
    public List<Collider2D> this[Direction dir] => dic != null ? dic[dir] : new List<Collider2D>();
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (dic == null)
            Init();
    }
    private void FixedUpdate()
    {
        RaycastDetct();
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(DetectBox))]
    public class DetectBox_Inspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DetectBox script = target as DetectBox;
            script.distance = EditorGUILayout.FloatField("碰撞箱纵向偏移", script.distance);
            script.width = EditorGUILayout.FloatField("碰撞箱长宽偏移", script.width);
            script.obliside_Distance = EditorGUILayout.FloatField("斜侧方碰撞箱长度偏移", script.obliside_Distance);
            script.length = EditorGUILayout.FloatField("斜侧方碰撞箱长宽", script.length);
            GUI.enabled = false;
            GUILayout.Space(20);
            GUILayout.Label("上");
            if (script == null)
                Debug.Log("null");
            foreach (var e in script[Direction.Up])
                EditorGUILayout.ObjectField(e.gameObject, typeof(GameObject), true);
            GUILayout.Space(20);
            GUILayout.Label("右");
            foreach (var e in script[Direction.Right])
                EditorGUILayout.ObjectField(e.gameObject, typeof(GameObject), true);
            GUILayout.Space(20);
            GUILayout.Label("下");
            foreach (var e in script[Direction.Down])
                EditorGUILayout.ObjectField(e.gameObject, typeof(GameObject), true);
            GUILayout.Space(20);
            GUILayout.Label("左");
            foreach (var e in script[Direction.Left])
                EditorGUILayout.ObjectField(e.gameObject, typeof(GameObject), true);
            GUILayout.Space(20);
            GUILayout.Label("右上");
            foreach (var e in script[Direction.RU])
                EditorGUILayout.ObjectField(e.gameObject, typeof(GameObject), true);
            GUILayout.Space(20);
            GUILayout.Label("右下");
            foreach (var e in script[Direction.RD])
                EditorGUILayout.ObjectField(e.gameObject, typeof(GameObject), true);
            GUILayout.Space(20);
            GUILayout.Label("左下");
            foreach (var e in script[Direction.LD])
                EditorGUILayout.ObjectField(e.gameObject, typeof(GameObject), true);
            GUILayout.Space(20);
            GUILayout.Label("左上");
            foreach (var e in script[Direction.LU])
                EditorGUILayout.ObjectField(e.gameObject, typeof(GameObject), true);
            GUILayout.Space(20);
            GUI.enabled = true;
        }
    }
#endif
}