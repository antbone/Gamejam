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
    public float gradient = 30;//牌圈下落范围坡度(角度)
    public float height = 1;
    public float moveSpeedMul = 0.1f;
    private bool isMoving = false;
    private Vector2 movementBounds;
    public Transform cardFolder;
    public GameObject cardPrefab;
    void OnValidate()
    {
        ResetCom();
    }

    void Awake()
    {
        GameManager.Ins.table = this;
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
        // 设置牌圈位置和大小
        cardCircle.transform.position = new Vector3(transform.position.x, height + transform.position.y, transform.position.z);
        cardCircle.transform.localScale = new Vector3(2 * cardCircleRadius, 2 * cardCircleRadius, 1);

        // 设置聚光灯
        if (cardCircleLight != null)
        {
            Light spotLight = cardCircleLight.GetComponent<Light>();
            if (spotLight != null && spotLight.type == LightType.Spot)
            {
                // 计算聚光灯的高度
                // 根据gradient角度和圆半径，使用正切函数计算高度
                float lightHeight = cardCircleRadius / Mathf.Tan(gradient * Mathf.Deg2Rad);

                // 设置聚光灯位置
                cardCircleLight.position = cardCircle.transform.position + Vector3.up * lightHeight;

                // 确保聚光灯正对下方
                cardCircleLight.rotation = Quaternion.Euler(90, 0, 0);

                // 设置聚光灯角度
                // 聚光灯的spotAngle是光锥的总角度，所以是gradient的两倍
                spotLight.spotAngle = gradient * 2;

                // 计算从光源到桌面的总距离
                float totalHeight = lightHeight + height + (cardCircle.transform.position.y - transform.position.y);

                // 设置光照范围，确保能照到桌面
                // 由于光线是倾斜的，需要考虑实际的斜线距离
                float maxRadius = Mathf.Max(
                    Vector2.Distance(new Vector2(ld.position.x, ld.position.z), new Vector2(rd.position.x, rd.position.z)),
                    Vector2.Distance(new Vector2(ld.position.x, ld.position.z), new Vector2(lu.position.x, lu.position.z))
                ) * 0.5f;

                // 使用勾股定理计算从光源到桌面最远点的距离
                float maxDistance = Mathf.Sqrt(totalHeight * totalHeight + maxRadius * maxRadius);

                // 设置range为最大距离，加上一些余量
                spotLight.range = maxDistance * 1.1f;
            }
        }
    }

    void Update()
    {
        if (!isMoving) return;
        Vector2 delta = Input.mousePosition.V2() - mousePosition;
        mousePosition = Input.mousePosition;
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
