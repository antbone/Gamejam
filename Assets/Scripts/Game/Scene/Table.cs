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
    private float cardSpawnTimer = 0;
    private const float SPAWN_INTERVAL = 0.2f;
    public Transform deadHeight;
    private const float INITIAL_FORCE = 1f; // 初始向上的力
    public float torque = 1f; // 最大扭矩
    private int cardsSpawned = 0;
    private List<CardPhysicsController> settledCards = new List<CardPhysicsController>();
    private bool isTransitioning = false;
    private const float TRANSITION_DELAY = 3.0f;
    private bool allCardsSpawned = false; // 标记是否所有卡牌都已生成
    private bool allCardsSettled = false; // 标记是否所有卡牌都已稳定
    private List<CardPhysicsController> allSpawnedCards = new List<CardPhysicsController>(); // 记录所有释放的卡牌
    private List<CardPhysicsController> cardsBelowDeadHeight = new List<CardPhysicsController>(); // 记录掉出deadHeight的卡牌

    void OnValidate()
    {
        ResetCom();
    }

    void Awake()
    {
        GameManager.Ins.table = this;
        InitializeMovementBounds();
        // 确保桌面有Table标签
        gameObject.tag = "Table";
        // 设置卡牌的死亡高度
        CardPhysicsController.SetDeadHeight(deadHeight);
        // 设置卡牌与桌面的物理交互
        CardPhysicsController.SetupCardTableInteraction(gameObject);
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

    // 获取光锥在桌面上的投影半径
    private float GetLightProjectionRadius()
    {
        float lightHeight = cardCircleRadius / Mathf.Tan(gradient * Mathf.Deg2Rad);
        float totalHeight = lightHeight + height + (cardCircle.transform.position.y - transform.position.y);
        return totalHeight * Mathf.Tan((gradient * 2) * 0.5f * Mathf.Deg2Rad);
    }

    // 在圆形区域内获取均匀随机点
    private Vector2 GetRandomPointInCircle(float radius)
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float r = radius * Mathf.Sqrt(Random.Range(0f, 1f)); // 使用平方根确保均匀分布
        return new Vector2(r * Mathf.Cos(angle), r * Mathf.Sin(angle));
    }

    // 注册已稳定的卡牌
    public void RegisterSettledCard(CardPhysicsController card)
    {
        if (!settledCards.Contains(card))
        {
            settledCards.Add(card);
            Debug.Log($"卡牌已稳定，当前稳定卡牌数量: {settledCards.Count}");

            // 检查是否所有卡牌都已稳定
            CheckAllCardsSettled();
        }
    }

    // 移除已回收的卡牌
    public void RemoveCard(CardPhysicsController card)
    {
        if (settledCards.Contains(card))
        {
            settledCards.Remove(card);
            Debug.Log($"卡牌已回收，当前稳定卡牌数量: {settledCards.Count}");

            // 检查是否所有卡牌都已回收
            CheckAllCardsSettled();
        }

        // 记录掉出deadHeight的卡牌
        if (!cardsBelowDeadHeight.Contains(card))
        {
            cardsBelowDeadHeight.Add(card);
            Debug.Log($"卡牌掉出deadHeight，当前掉出卡牌数量: {cardsBelowDeadHeight.Count}");
        }
    }

    // 检查是否所有卡牌都已稳定或回收
    private void CheckAllCardsSettled()
    {
        // 如果所有卡牌都已生成且不在过渡状态
        if (allCardsSpawned && !isTransitioning)
        {
            // 计算有效卡牌数量（所有释放的卡牌减去掉出deadHeight的卡牌）
            int validCardsCount = allSpawnedCards.Count - cardsBelowDeadHeight.Count;

            // 检查是否所有有效卡牌都已稳定
            if (settledCards.Count >= validCardsCount && validCardsCount > 0)
            {
                allCardsSettled = true;
                TransitionToResultPhase();
            }
        }
    }

    // 过渡到结果阶段
    private void TransitionToResultPhase()
    {
        isTransitioning = true;
        Debug.Log("所有卡牌已稳定，3秒后进入结果阶段");

        // 回收所有还存留的卡牌
        RecycleAllCards();

        // 使用TM.SetTimer替代协程
        TM.SetTimer(this.Hash("TransitionToResult"), TRANSITION_DELAY, null, (s) =>
        {
            // 进入结果阶段
            GameFlowManager.Instance.TransitionTo(GameState.Result);
        });
    }

    private void SpawnCard()
    {
        // 检查是否已达到最大卡牌数量
        if (cardsSpawned >= CardInventory.Ins.cards.list.Count)
        {
            Debug.Log("已达到最大卡牌数量，无法继续生成");
            allCardsSpawned = true; // 标记所有卡牌都已生成
            return;
        }

        if (cardPrefab == null || cardFolder == null) return;

        // 在牌圈范围内随机一个位置
        Vector2 randomOffset = GetRandomPointInCircle(cardCircleRadius);
        Vector3 spawnPos = cardCircle.transform.position + new Vector3(randomOffset.x, 0, randomOffset.y);

        // 使用对象池创建卡牌，初始旋转角度在(90, 0, 0)附近
        Quaternion baseRotation = Quaternion.Euler(90, 0, 0);
        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(85f, 95f),  // X轴在90度附近±5度
            Random.Range(-10f, 10f), // Y轴在0度附近±10度
            Random.Range(-10f, 10f)  // Z轴在0度附近±10度
        );

        // 先创建卡牌但不设置位置和旋转
        GameObject card = cardPrefab.OPGet(cardFolder);

        // 确保有Rigidbody组件
        Rigidbody rb = card.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = card.AddComponent<Rigidbody>();
        }

        // 设置物理属性
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        rb.mass = 0.1f;
        rb.drag = 0.3f;
        rb.angularDrag = 0.3f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // 添加物理控制器组件
        CardPhysicsController physicsController = card.GetComponent<CardPhysicsController>();
        if (physicsController == null)
        {
            physicsController = card.AddComponent<CardPhysicsController>();
        }

        // 设置Table引用
        physicsController.SetTable(this);

        // 重置物理控制器状态
        physicsController.ResetState();

        // 计算聚光灯在桌面上的投影范围
        float projectionRadius = GetLightProjectionRadius();

        // 在聚光灯投影范围内随机选择一个目标点
        Vector2 targetOffset = GetRandomPointInCircle(projectionRadius);
        Vector3 targetPos = cardCircleLight.position + new Vector3(targetOffset.x, -height, targetOffset.y);

        // 计算水平方向的初始力，使卡牌朝向目标点
        Vector3 direction = (targetPos - spawnPos).normalized;
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z).normalized;

        // 先设置位置和旋转
        card.transform.position = spawnPos;
        card.transform.rotation = baseRotation * randomRotation;

        // 使用TM.SetTimer替代协程，延迟一帧应用力
        TM.SetTimer(this.Hash("ApplyForces"), 0.0f, null, (s) =>
        {
            // 只添加水平方向的力
            rb.AddForce(horizontalDirection * INITIAL_FORCE * 0.3f, ForceMode.Impulse);

            // 添加较小的随机旋转，主要保持水平状态
            rb.AddTorque(new Vector3(
                Random.Range(-torque, torque) * 0.1f, // 较小的X轴旋转，保持接近90度
                Random.Range(-torque, torque) * 0.2f, // 适度的Y轴旋转
                Random.Range(-torque, torque) * 0.2f  // 适度的Z轴旋转
            ), ForceMode.Impulse);
        });

        // 增加已生成卡牌计数
        cardsSpawned++;
        Debug.Log($"已生成卡牌: {cardsSpawned}/{CardInventory.Ins.cards.list.Count}");

        // 记录所有释放的卡牌
        allSpawnedCards.Add(physicsController);

        // 检查是否已达到最大卡牌数量
        if (cardsSpawned >= CardInventory.Ins.cards.list.Count)
        {
            allCardsSpawned = true; // 标记所有卡牌都已生成
            Debug.Log("所有卡牌已生成完毕，等待卡牌稳定");
        }
    }

    void Update()
    {
        if (!isMoving) return;
        Vector2 delta = Input.mousePosition.V2() - mousePosition;
        mousePosition = Input.mousePosition;
        Vector3 newPosition = cardCircle.transform.position + new Vector3(
            delta.x / movementBounds.x * moveSpeedMul,
            0,
            delta.y / movementBounds.y * moveSpeedMul
        );

        // 应用边界限制
        newPosition = ClampPositionWithinBounds(newPosition);

        // 更新牌圈位置
        cardCircle.transform.position = newPosition;

        // 处理卡牌生成
        if (Input.GetMouseButton(0) && !isTransitioning) // 左键按住且不在过渡状态
        {
            cardSpawnTimer += Time.deltaTime;
            if (cardSpawnTimer >= SPAWN_INTERVAL)
            {
                SpawnCard();
                cardSpawnTimer = 0;
            }
        }
        else
        {
            cardSpawnTimer = SPAWN_INTERVAL; // 确保松开后立即可以生成
        }

        // 检查所有卡牌是否都已稳定
        if (allCardsSpawned && !allCardsSettled && !isTransitioning)
        {
            CheckAllCardsSettled();
        }
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

    // 回收所有卡牌
    public void RecycleAllCards()
    {
        foreach (var card in settledCards.ToArray())
        {
            if (card != null)
            {
                card.gameObject.OPPush();
            }
        }
        settledCards.Clear();

        // 清空所有卡牌记录
        allSpawnedCards.Clear();
        cardsBelowDeadHeight.Clear();
    }
}
