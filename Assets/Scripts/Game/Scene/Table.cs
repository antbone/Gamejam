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
    public Material darkMaterial;
    public Material lightMaterial;
    public Material activeMaterial;
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
    [HideInInspector]
    public List<CardPhysicsController> allSpawnedCards = new List<CardPhysicsController>(); // 记录所有释放的卡牌
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
            Random.Range(90f - 75f, 90f + 75f),  // X轴在90度附近±5度
            Random.Range(-75f, 75f), // Y轴在0度附近±10度
            Random.Range(-75f, 75f)  // Z轴在0度附近±10度
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

        // 初始化卡牌状态
        physicsController.InitializeState();

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
        // CT.DelayCmd(() =>
        // {
        //     // 只添加水平方向的力
        //     rb.AddForce(horizontalDirection * INITIAL_FORCE * 0.3f, ForceMode.Impulse);
        //     // 添加较小的随机旋转，主要保持水平状态
        //     rb.AddTorque(new Vector3(
        //         Random.Range(-torque, torque) * 0.1f, // 较小的X轴旋转，保持接近90度
        //         Random.Range(-torque, torque) * 0.2f, // 适度的Y轴旋转
        //         Random.Range(-torque, torque) * 0.2f  // 适度的Z轴旋转
        //     ), ForceMode.Impulse);

        // });

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

        // 检查所有已生成卡牌的状态（不仅仅是稳定的卡牌）
        foreach (var card in allSpawnedCards)
        {
            if (card != null && !cardsBelowDeadHeight.Contains(card))
            {
                UpdateCardState(card);
            }
        }

        // 检查所有卡牌是否都已稳定
        if (allCardsSpawned && !allCardsSettled && !isTransitioning)
        {
            CheckAllCardsSettled();
        }
    }

    // 更新卡牌状态
    private void UpdateCardState(CardPhysicsController card)
    {
        if (card == null) return;

        // 获取卡牌的9个关键点
        Transform[] keyPoints = GetCardKeyPoints(card);
        if (keyPoints == null || keyPoints.Length != 9) return;

        // 检查卡牌是否被覆盖
        bool isCovered = IsCardCovered(card, keyPoints);
        if (isCovered)
        {
            card.SetState(CardPhysicsController.CardState.Covered);
            return;
        }

        // 检查卡牌是否与其他卡牌联动
        bool isLinked = IsCardLinked(card);
        if (isLinked)
        {
            card.SetState(CardPhysicsController.CardState.Linked);
            return;
        }

        // 如果既没有被覆盖也没有联动，则为启用状态
        card.SetState(CardPhysicsController.CardState.Active);
    }

    // 获取卡牌的9个关键点
    private Transform[] GetCardKeyPoints(CardPhysicsController card)
    {
        Transform[] keyPoints = new Transform[9];
        Transform cardTransform = card.transform;

        // 查找卡牌上的9个子物体
        for (int i = 0; i < cardTransform.childCount; i++)
        {
            Transform child = cardTransform.GetChild(i);
            string childName = child.name.ToLower();

            if (childName == "lu") keyPoints[0] = child;
            else if (childName == "lc") keyPoints[1] = child;
            else if (childName == "ld") keyPoints[2] = child;
            else if (childName == "mu") keyPoints[3] = child;
            else if (childName == "mc") keyPoints[4] = child;
            else if (childName == "md") keyPoints[5] = child;
            else if (childName == "ru") keyPoints[6] = child;
            else if (childName == "rc") keyPoints[7] = child;
            else if (childName == "rd") keyPoints[8] = child;
        }

        // 检查是否所有点都找到了
        for (int i = 0; i < 9; i++)
        {
            if (keyPoints[i] == null)
            {
                Debug.LogWarning($"卡牌 {card.name} 缺少关键点 {i}");
                return null;
            }
        }

        return keyPoints;
    }

    // 检查卡牌是否被覆盖
    private bool IsCardCovered(CardPhysicsController card, Transform[] keyPoints)
    {
        if (keyPoints == null || keyPoints.Length != 9) return false;

        // 检查每个关键点上方是否有其他卡牌
        int coveredPoints = 0;
        foreach (Transform point in keyPoints)
        {
            // 在XZ平面上获取点的位置
            Vector3 pointPos = new Vector3(point.position.x, 0, point.position.z);

            // 检查是否有其他卡牌在这个点的上方
            bool hasCardAbove = false;
            foreach (var otherCard in allSpawnedCards)
            {
                if (otherCard == card) continue;

                // 获取其他卡牌的边界
                Rect otherBounds = GetCardBounds(otherCard);

                // 检查点是否在其他卡牌的XZ投影范围内
                if (IsPointInBoundsXZ(pointPos, otherBounds))
                {
                    // 检查点的高度是否低于其他卡牌
                    // 使用卡牌的transform.position.y作为高度参考
                    if (point.position.y < otherCard.transform.position.y)
                    {
                        hasCardAbove = true;
                        break;
                    }
                }
            }

            if (hasCardAbove)
            {
                coveredPoints++;
            }
        }

        // 如果所有点都被覆盖，则卡牌被覆盖
        return coveredPoints == 9;
    }

    // 检查卡牌是否与其他卡牌联动
    private bool IsCardLinked(CardPhysicsController card)
    {
        if (card == null) return false;

        // 获取卡牌的边界
        Rect cardBounds = GetCardBounds(card);

        // 检查是否与其他卡牌在XZ平面上有重叠
        foreach (var otherCard in allSpawnedCards)
        {
            if (otherCard == card) continue;

            // 获取其他卡牌的边界
            Rect otherBounds = GetCardBounds(otherCard);

            // 检查两个卡牌在XZ平面上是否有重叠
            if (DoBoundsOverlapXZ(cardBounds, otherBounds))
            {
                // 检查当前卡牌是否被覆盖
                Transform[] currentKeyPoints = GetCardKeyPoints(card);
                bool isCurrentCardCovered = IsCardCovered(card, currentKeyPoints);

                // 检查其他卡牌是否被覆盖
                Transform[] otherKeyPoints = GetCardKeyPoints(otherCard);
                bool isOtherCardCovered = IsCardCovered(otherCard, otherKeyPoints);

                // 只有当两个卡牌都没有被覆盖时，才认为它们联动
                if (!isCurrentCardCovered && !isOtherCardCovered)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // 获取卡牌在XZ平面上的矩形范围
    private Rect GetCardBounds(CardPhysicsController card)
    {
        if (card == null) return new Rect();

        // 获取卡牌的9个关键点
        Transform[] keyPoints = GetCardKeyPoints(card);
        if (keyPoints == null || keyPoints.Length != 9)
        {
            // 如果没有关键点，使用卡牌的碰撞器或默认尺寸
            Collider collider = card.GetComponent<Collider>();
            if (collider != null)
            {
                Bounds bounds = collider.bounds;
                return new Rect(bounds.min.x, bounds.min.z, bounds.size.x, bounds.size.z);
            }
            else
            {
                // 使用默认尺寸
                Vector3 position = card.transform.position;
                return new Rect(position.x - 0.3f, position.z - 0.45f, 0.6f, 0.9f);
            }
        }

        // 计算关键点在XZ平面上的最小和最大坐标
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        foreach (Transform point in keyPoints)
        {
            minX = Mathf.Min(minX, point.position.x);
            maxX = Mathf.Max(maxX, point.position.x);
            minZ = Mathf.Min(minZ, point.position.z);
            maxZ = Mathf.Max(maxZ, point.position.z);
        }

        // 返回矩形范围
        return new Rect(minX, minZ, maxX - minX, maxZ - minZ);
    }

    // 检查点是否在边界的XZ投影范围内
    private bool IsPointInBoundsXZ(Vector3 point, Rect bounds)
    {
        return point.x >= bounds.xMin && point.x <= bounds.xMax &&
               point.z >= bounds.yMin && point.z <= bounds.yMax;
    }

    // 检查两个边界在XZ平面上是否有重叠
    private bool DoBoundsOverlapXZ(Rect bounds1, Rect bounds2)
    {
        return !(bounds1.xMax < bounds2.xMin || bounds1.xMin > bounds2.xMax ||
                 bounds1.yMax < bounds2.yMin || bounds1.yMin > bounds2.yMax);
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
