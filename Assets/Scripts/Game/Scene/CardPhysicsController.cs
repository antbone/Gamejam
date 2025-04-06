using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CardPhysicsController : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasHitGround = false;
    private const float DAMPING = 0.9f; // 碰撞后的速度衰减系数
    private const float SETTLE_TIME = 2.0f; // 卡牌稳定后等待回收的时间
    private bool isSettled = false;
    private static Transform deadHeight; // 死亡高度参考点
    private Table table; // 对Table的引用
    private string settleTimerId; // 稳定计时器ID
    private const float VELOCITY_THRESHOLD = 0.05f; // 判定为平稳的速度阈值
    private const float ANGULAR_VELOCITY_THRESHOLD = 0.05f; // 判定为平稳的角速度阈值
    private float stableTime = 0f; // 保持平稳的时间
    private const float REQUIRED_STABLE_TIME = 0.5f; // 需要保持平稳的时间
    private static PhysicMaterial cardMaterial; // 卡牌物理材质
    private static PhysicMaterial tableMaterial; // 桌面物理材质
    private static PhysicMaterial cardCardMaterial; // 卡牌与卡牌之间的物理材质

    // 卡牌旋转控制参数
    private const float LAYING_DOWN_TORQUE = 0.005f; // 使卡牌平躺的扭矩大小
    private const float UPRIGHT_THRESHOLD = 0.7f; // 判定为立起的阈值（与垂直方向的点积）
    private const float ROTATION_CHECK_INTERVAL = 0.1f; // 检查旋转状态的时间间隔
    private float lastRotationCheckTime = 0f; // 上次检查旋转状态的时间

    // 摩擦力参数
    private const float TABLE_FRICTION = 0.8f; // 与桌面接触时的摩擦力
    private const float CARD_FRICTION = 0.1f; // 与卡牌接触时的摩擦力
    private bool isTouchingTable = false; // 是否接触桌面
    private bool isTouchingCard = false; // 是否接触其他卡牌

    public static void SetDeadHeight(Transform height)
    {
        deadHeight = height;
    }

    public void SetTable(Table tableRef)
    {
        table = tableRef;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SetupPhysicsMaterial();
        // 为每个卡牌生成唯一的计时器ID
        settleTimerId = this.Hash("SettleTimer");
    }

    private void SetupPhysicsMaterial()
    {
        // 创建卡牌物理材质（低摩擦力）
        if (cardMaterial == null)
        {
            cardMaterial = new PhysicMaterial("CardMaterial");
            cardMaterial.bounciness = 0.1f; // 轻微弹性
            cardMaterial.dynamicFriction = CARD_FRICTION; // 非常低的动态摩擦力
            cardMaterial.staticFriction = CARD_FRICTION; // 非常低的静态摩擦力
            cardMaterial.frictionCombine = PhysicMaterialCombine.Minimum; // 使用最小值组合摩擦力
            cardMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
        }

        // 创建桌面物理材质（高摩擦力）
        if (tableMaterial == null)
        {
            tableMaterial = new PhysicMaterial("TableMaterial");
            tableMaterial.bounciness = 0.1f;
            tableMaterial.dynamicFriction = TABLE_FRICTION;
            tableMaterial.staticFriction = TABLE_FRICTION;
            tableMaterial.frictionCombine = PhysicMaterialCombine.Average;
            tableMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
        }

        // 创建卡牌与卡牌之间的物理材质（极低摩擦力）
        if (cardCardMaterial == null)
        {
            cardCardMaterial = new PhysicMaterial("CardCardMaterial");
            cardCardMaterial.bounciness = 0.1f;
            cardCardMaterial.dynamicFriction = CARD_FRICTION;
            cardCardMaterial.staticFriction = CARD_FRICTION;
            cardCardMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
            cardCardMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
        }

        // 应用物理材质到所有碰撞器
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.material = cardMaterial;
        }

        // 设置刚体属性
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.drag = 0.5f; // 适当的空气阻力
        rb.angularDrag = 0.5f;
    }

    // 设置卡牌与桌面的物理交互
    public static void SetupCardTableInteraction(GameObject table)
    {
        if (table == null) return;

        // 获取桌面的碰撞器
        Collider[] tableColliders = table.GetComponents<Collider>();
        foreach (Collider collider in tableColliders)
        {
            collider.material = tableMaterial;
        }
    }

    public void ResetState()
    {
        hasHitGround = false;
        isSettled = false;
        stableTime = 0f;
        lastRotationCheckTime = 0f;
        isTouchingTable = false;
        isTouchingCard = false;

        // 清除之前的计时器
        TM.SetEnd(settleTimerId);

        // 重置刚体状态
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
        }

        // 重置物理材质
        Collider[] colliders = GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.material = cardMaterial;
        }
    }

    void Update()
    {
        // 检查是否低于死亡高度
        if (deadHeight != null && transform.position.y < deadHeight.position.y)
        {
            if (table != null)
            {
                table.RemoveCard(this);
            }
            gameObject.OPPush();
            return;
        }

        // 检查卡牌是否已经稳定
        CheckStability();

        // 检查并调整卡牌的旋转状态
        CheckAndAdjustRotation();

        // 更新物理材质
        UpdatePhysicsMaterial();
    }

    // 更新物理材质
    private void UpdatePhysicsMaterial()
    {
        // 获取所有碰撞器
        Collider[] colliders = GetComponents<Collider>();

        // 如果同时接触桌面和卡牌，优先使用桌面材质（高摩擦力）
        if (isTouchingTable)
        {
            foreach (Collider collider in colliders)
            {
                collider.material = tableMaterial;
            }
        }
        // 如果只接触卡牌，使用卡牌与卡牌之间的材质（低摩擦力）
        else if (isTouchingCard)
        {
            foreach (Collider collider in colliders)
            {
                collider.material = cardCardMaterial;
            }
        }
        // 如果都不接触，使用默认卡牌材质
        else
        {
            foreach (Collider collider in colliders)
            {
                collider.material = cardMaterial;
            }
        }
    }

    // 检查并调整卡牌的旋转状态
    private void CheckAndAdjustRotation()
    {
        // 如果已经稳定，不需要再调整旋转
        if (isSettled) return;

        // 按时间间隔检查旋转状态
        if (Time.time - lastRotationCheckTime < ROTATION_CHECK_INTERVAL)
            return;

        lastRotationCheckTime = Time.time;

        // 获取卡牌的局部Y轴方向（卡牌的上方向）
        Vector3 cardUpDirection = transform.up;

        // 计算与垂直方向的点积（越接近1，越接近垂直）
        float dotProduct = Vector3.Dot(cardUpDirection, Vector3.up);
        float uprightDegree = Mathf.Abs(dotProduct);

        // 如果卡牌接近立起状态（点积接近1或-1）
        if (uprightDegree > UPRIGHT_THRESHOLD)
        {
            // 计算使卡牌平躺的扭矩方向
            // 使用叉积计算扭矩方向，确保方向与卡牌倾倒方向一致
            Vector3 torqueDirection;

            // 如果卡牌向上（点积接近1），则向下翻转
            if (dotProduct > 0)
            {
                // 使用叉积计算扭矩方向，确保方向与卡牌倾倒方向一致
                torqueDirection = Vector3.Cross(cardUpDirection, Vector3.left).normalized;
            }
            // 如果卡牌向下（点积接近-1），则向上翻转
            else
            {
                // 使用叉积计算扭矩方向，确保方向与卡牌倾倒方向一致
                torqueDirection = Vector3.Cross(Vector3.left, cardUpDirection).normalized;
            }

            // 如果扭矩方向太小，使用默认方向
            if (torqueDirection.magnitude < 0.1f)
            {
                // 根据卡牌朝向选择默认方向
                if (dotProduct > 0)
                {
                    torqueDirection = Vector3.right;
                }
                else
                {
                    torqueDirection = Vector3.left;
                }
            }

            // 计算扭矩大小，与立起程度成正比
            // 立起程度越高，扭矩越大
            float torqueMagnitude = LAYING_DOWN_TORQUE * (uprightDegree - UPRIGHT_THRESHOLD) / (1.0f - UPRIGHT_THRESHOLD);
            // 限制最大扭矩
            torqueMagnitude = Mathf.Min(torqueMagnitude, LAYING_DOWN_TORQUE * 2.0f);

            // 施加扭矩使卡牌平躺
            rb.AddTorque(torqueDirection * torqueMagnitude, ForceMode.Impulse);

            // 增加角速度阻尼，使旋转更平滑
            // 立起程度越高，阻尼越大
            float dampingFactor = 0.95f - (uprightDegree - UPRIGHT_THRESHOLD) * 0.2f;
            dampingFactor = Mathf.Clamp(dampingFactor, 0.8f, 0.95f);
            rb.angularVelocity *= dampingFactor;
        }
    }

    // 检查卡牌是否稳定
    private void CheckStability()
    {
        // 如果已经稳定，不需要再检查
        if (isSettled) return;

        // 检查速度和角速度是否低于阈值
        bool isVelocityStable = rb.velocity.magnitude < VELOCITY_THRESHOLD;
        bool isAngularVelocityStable = rb.angularVelocity.magnitude < ANGULAR_VELOCITY_THRESHOLD;

        // 检查卡牌是否接近平躺状态
        bool isLayingDown = IsCardLayingDown();

        // 如果速度和角速度都稳定，且卡牌接近平躺状态
        if (isVelocityStable && isAngularVelocityStable && isLayingDown)
        {
            // 增加稳定时间
            stableTime += Time.deltaTime;

            // 如果稳定时间达到要求
            if (stableTime >= REQUIRED_STABLE_TIME)
            {
                // 立即标记为稳定并注册到Table
                isSettled = true;
                if (table != null)
                {
                    table.RegisterSettledCard(this);
                }
            }
        }
        else
        {
            // 如果速度或角速度不稳定，或者卡牌不接近平躺状态，重置稳定时间
            stableTime = 0f;
        }
    }

    // 检查卡牌是否接近平躺状态
    private bool IsCardLayingDown()
    {
        // 获取卡牌的局部Y轴方向（卡牌的上方向）
        Vector3 cardUpDirection = transform.up;

        // 计算与垂直方向的点积（越接近0，越接近平躺）
        float dotProduct = Vector3.Dot(cardUpDirection, Vector3.up);

        // 如果点积接近0，说明卡牌接近平躺状态
        return Mathf.Abs(dotProduct) < 0.3f;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 检查是否与桌面碰撞
        if (collision.gameObject.CompareTag("Table"))
        {
            hasHitGround = true;
            isTouchingTable = true;

            // 减小碰撞后的速度
            rb.velocity *= DAMPING;
            rb.angularVelocity *= DAMPING;
        }
        // 检查是否与其他卡牌碰撞
        else if (collision.gameObject.GetComponent<CardPhysicsController>() != null)
        {
            isTouchingCard = true;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (hasHitGround && collision.gameObject.CompareTag("Table"))
        {
            // 在持续接触时，逐渐减小速度和角速度
            rb.velocity *= 0.98f;
            rb.angularVelocity *= 0.98f;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // 检查是否离开桌面
        if (collision.gameObject.CompareTag("Table"))
        {
            isTouchingTable = false;
        }
        // 检查是否离开其他卡牌
        else if (collision.gameObject.GetComponent<CardPhysicsController>() != null)
        {
            isTouchingCard = false;
        }
    }

    void OnDestroy()
    {
        // 确保在销毁时从Table中移除
        if (table != null)
        {
            table.RemoveCard(this);
        }

        // 清除计时器
        TM.SetEnd(settleTimerId);
    }
}