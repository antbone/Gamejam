using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public Transform ld;//牌圈移动区域左下角点
    public Transform rd;//牌圈移动区域右下角点
    public Transform lu;//牌圈移动区域左上角点
    public Transform ru;//牌圈移动区域右上角点
    public float cardCircleRadius = 1;
    public SpriteRenderer cardCircle;//牌圈
    public Transform cardCircleLight;
    public float gradient = 0;//牌圈下落范围坡度(角度)
    public float height = 1;
    public float moveSpeedMul = 0.1f;
    private Vector3 cardCircleInitialPosition;
    private bool isMoving = false;
    private Vector2 movementBounds;
    void OnValidate()
    {
        ResetCom();
    }

    void Awake()
    {
        GameManager.Ins.table = this;
        cardCircleInitialPosition = cardCircle.transform.position;
        InitializeMovementBounds();
    }

    private void InitializeMovementBounds()
    {
        // 计算移动范围的宽度和深度
        float width = Mathf.Abs(rd.position.x - ld.position.x);
        float depth = Mathf.Abs(lu.position.z - ld.position.z);
        movementBounds = new Vector2(width, depth);

    }

    private Vector2 mousePosition;
    public void StartCardCircleMovement()
    {
        isMoving = true;
        mousePosition = Input.mousePosition;
        cardCircle.transform.position = new Vector3(transform.position.x, cardCircle.transform.position.y, transform.position.z);
    }

    public void StopCardCircleMovement()
    {
        isMoving = false;
    }

    public void ResetCom()
    {
        cardCircle.transform.position = new Vector3(transform.position.x, height + transform.position.y, transform.position.z);
        cardCircle.transform.localScale = new Vector3(2 * cardCircleRadius, 2 * cardCircleRadius, 1);

    }

    void Update()
    {
        if (!isMoving) return;
        Vector2 delta = Input.mousePosition.V2() - mousePosition;

        // 将0-1范围映射到实际移动范围
        Vector3 newPosition = cardCircle.transform.position + new Vector3(
            delta.x / movementBounds.x * moveSpeedMul,
            0,
            delta.y / movementBounds.y * moveSpeedMul
        );

        // 应用边界限制
        newPosition = ClampPositionWithinBounds(newPosition);

        // 更新牌圈位置
        cardCircle.transform.position = newPosition;
    }

    private Vector3 ClampPositionWithinBounds(Vector3 position)
    {
        // 获取边界矩形的范围
        float minX = Mathf.Min(ld.position.x, lu.position.x) + cardCircleRadius;
        float maxX = Mathf.Max(rd.position.x, ru.position.x) - cardCircleRadius;
        float minZ = Mathf.Min(ld.position.z, rd.position.z) + cardCircleRadius;
        float maxZ = Mathf.Max(lu.position.z, ru.position.z) - cardCircleRadius;

        // 限制位置在边界内
        return new Vector3(
            Mathf.Clamp(position.x, minX, maxX),
            position.y,
            Mathf.Clamp(position.z, minZ, maxZ)
        );
    }
}
