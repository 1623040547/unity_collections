using UnityEngine;

public class SphereCollisionDetector
{
    private float sphereRadius;
    private Rigidbody targetRigidbody;
    private Vector3 lastValidPosition;  // 添加最后有效位置的记录
    
    public SphereCollisionDetector(float radius, Rigidbody rb)
    {
        sphereRadius = radius;
        targetRigidbody = rb;
        lastValidPosition = rb.position;  // 初始化最后有效位置
    }
    
    /// <summary>
    /// 球形碰撞检测器
    /// 用于处理球形物体的移动碰撞检测，通过射线检测来预判移动路径上是否存在障碍物
    /// 
    /// 工作原理：
    /// 1. 从物体中心发射射线进行碰撞检测
    /// 2. 检测距离 = 预期移动距离 + 球体半径
    /// 3. 通过射线检测结果决定是否允许移动或计算安全移动距离
    /// 
    /// 使用要求：
    /// 1. 游戏对象必须添加以下组件：
    ///    - Rigidbody：用于物理移动
    ///    - SphereCollider：用于定义碰撞球体的大小
    ///    
    /// 2. Rigidbody组件推荐设置：
    ///    - Use Gravity = false：避免重力影响
    ///    - Is Kinematic = false：允许物理碰撞
    ///    - Freeze Rotation = true：防止物理系统影响旋转
    ///    
    /// 示例用法：
    /// <code>
    /// public class ObjectController : MonoBehaviour 
    /// {
    ///     private SphereCollisionDetector collisionDetector;
    ///     private Rigidbody rb;
    ///     public float moveSpeed = 5f;
    ///     
    ///     void Start()
    ///     {
    ///         rb = GetComponent<Rigidbody>();
    ///         SphereCollider sphereCollider = GetComponent<SphereCollider>();
    ///         collisionDetector = new SphereCollisionDetector(
    ///             sphereCollider.radius,
    ///             rb
    ///         );
    ///     }
    ///     
    ///     void Update()
    ///     {
    ///         // 获取移动向量（示例使用水平和垂直输入）
    ///         Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    ///         
    ///         // 转换为世界空间的移动方向（如果需要）
    ///         movement = transform.TransformDirection(movement);
    ///         
    ///         // 尝试移动
    ///         collisionDetector.TryMove(movement, moveSpeed, Time.deltaTime);
    ///     }
    /// }
    /// </code>
    /// </summary>
    
    /// <summary>
    /// 检查指定方向的移动是否会发生碰撞
    /// </summary>
    /// <param name="movement">移动向量</param>
    /// <param name="speed">移动速度</param>
    /// <param name="deltaTime">时间增量</param>
    /// <returns>如果会发生碰撞返回true，否则返回false</returns>
    public bool WillCollide(Vector3 movement, float speed, float deltaTime)
    {
        // 确保movement是标准化的向量
        movement.Normalize();
        
        // 计算目标位置
        Vector3 targetPosition = targetRigidbody.position + movement * speed * deltaTime;
        
        // 计算实际需要检测的距离：移动距离 + 碰撞器半径
        float detectionDistance = Vector3.Distance(targetRigidbody.position, targetPosition) + sphereRadius;
        
        // 使用射线检测前方是否有障碍物
        return Physics.Raycast(targetRigidbody.position, movement, detectionDistance);
    }
    
    /// <summary>
    /// 尝试移动到目标位置，如果不会发生碰撞则移动
    /// </summary>
    /// <param name="movement">移动向量</param>
    /// <param name="speed">移动速度</param>
    /// <param name="deltaTime">时间增量</param>
    /// <returns>如果成功移动返回true，否则返回false</returns>
    public bool TryMove(Vector3 movement, float speed, float deltaTime)
    {
        movement.Normalize();
        
        // 计算目标位置
        Vector3 targetPosition = targetRigidbody.position + movement * speed * deltaTime;
        
        // 计算实际需要检测的距离
        float detectionDistance = Vector3.Distance(targetRigidbody.position, targetPosition) + sphereRadius;
        
        // 使用射线检测前方是否有障碍物
        RaycastHit hit;
        if (Physics.Raycast(targetRigidbody.position, movement, out hit, detectionDistance))
        {
            return false;
        }
        
        // 如果没有检测到碰撞，更新位置和最后有效位置
        lastValidPosition = targetPosition;
        targetRigidbody.MovePosition(targetPosition);
        return true;
    }
    
    /// <summary>
    /// 获取最后一个有效的位置
    /// </summary>
    public Vector3 GetLastValidPosition()
    {
        return lastValidPosition;
    }
    
    /// <summary>
    /// 获取安全的移动距离
    /// </summary>
    /// <param name="movement">移动向量</param>
    /// <param name="speed">移动速度</param>
    /// <param name="deltaTime">时间增量</param>
    /// <returns>安全的移动距离，如果检测到碰撞则返回到碰撞点的距离减去球体半径</returns>
    public float GetSafeMovementDistance(Vector3 movement, float speed, float deltaTime)
    {
        movement.Normalize();
        
        // 计算预期移动距离
        float intendedDistance = speed * deltaTime;
        
        // 计算检测距离
        float detectionDistance = intendedDistance + sphereRadius;
        
        // 检测碰撞
        RaycastHit hit;
        if (Physics.Raycast(targetRigidbody.position, movement, out hit, detectionDistance))
        {
            // 返回到碰撞点的安全距离（减去球体半径）
            return Mathf.Max(0, hit.distance - sphereRadius);
        }
        
        // 如果没有碰撞，返回预期移动距离
        return intendedDistance;
    }
}