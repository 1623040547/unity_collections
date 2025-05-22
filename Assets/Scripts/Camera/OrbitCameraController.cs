using UnityEngine;

public partial class OrbitCameraController : MonoBehaviour
{
    public const float MinRadius = 10f;
    public const float MaxRadius = 40f;

    public Transform target; 
    public string targetName;
    public float radius = 20f; 


    private Rigidbody rb;
    private SphereCollisionDetector collisionDetector;

    private float currentHorizontalAngle = 0f; // 当前水平角度
    private float currentVerticalAngle = 0f; // 当前垂直角度

    private bool positionUpdate = false;

    private void Awake()
    { 
        if( targetName == null) {
            targetName = GamEntity.Role;
        }
        GameObject roleObject = GameObject.Find(targetName);
        if (roleObject != null)
        {
            target = roleObject.transform;
            Debug.Log($"OrbitCameraController Awake {target}");
        } 
        else 
        {
            Debug.LogError("Role object not found!");
        }
    }

    void InitSphereCollision() {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.freezeRotation = true;
        rb.mass = 1f;
        rb.drag = 10f;
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if(sphereCollider == null) {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
        }
        sphereCollider.radius = 3f;
        collisionDetector = new SphereCollisionDetector(sphereCollider.radius, rb);
    }

    void Start()
    {
        InitSphereCollision();
    }

    void Update()
    {

        HandleKeyboardInput();
        HandleTouchInput();
        if(!positionUpdate) 
        {
          GetOnlyRoleMove();
        }
        positionUpdate = false;
    }

    ///仅检测目标物体平移
    private void GetOnlyRoleMove() {
        positionUpdate = true;

        Vector3 offset = new Vector3(
            Mathf.Sin(currentHorizontalAngle) * Mathf.Cos(currentVerticalAngle),
            Mathf.Sin(currentVerticalAngle),
            Mathf.Cos(currentHorizontalAngle) * Mathf.Cos(currentVerticalAngle)
        ) * radius;

        Vector3 targetPosition = target.position + offset;
        Vector3 direction = (targetPosition - transform.position).normalized;
        float detectionDistance = 5.0f; // 设定检测距离
        // 使用射线检测相机和目标之间的障碍物 
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, detectionDistance))
        {  
            // 如果检测到障碍物，停止移动
            Debug.Log("Obstacle detected, stopping movement.");
            radius = Vector3.Distance(transform.position, target.position);
        }
        else
        {
            if(radius < MaxRadius) {
                ///变化后位置与目标位置间的距离
                float distance = Vector3.Distance(transform.position, target.position);
                if(distance < MaxRadius) {
                    radius = distance; 
                    targetPosition = transform.position;
                }
                else 
                {
                    radius = MaxRadius; 
                    targetPosition = transform.position; 
                }
            } 
            // 根据垂直角度调整“上”方向
            Vector3 upDirection = currentVerticalAngle > Mathf.PI / 2 && currentVerticalAngle < 3 * Mathf.PI / 2 ? Vector3.down : Vector3.up;
            transform.position = targetPosition;
            transform.LookAt(target, upDirection);
        }
    }

    ///检测相机绕物体旋转
    private void GetNewPosition(Vector2 delta, float speed) {
        positionUpdate = true;
        currentHorizontalAngle = Mathf.Repeat(currentHorizontalAngle + delta.x * speed * Time.deltaTime, Mathf.PI * 2);
        currentVerticalAngle = Mathf.Repeat(currentVerticalAngle + delta.y * speed * Time.deltaTime, Mathf.PI * 2);
        Vector3 offset = new Vector3(
            Mathf.Sin(currentHorizontalAngle) * Mathf.Cos(currentVerticalAngle),
            Mathf.Sin(currentVerticalAngle),
            Mathf.Cos(currentHorizontalAngle) * Mathf.Cos(currentVerticalAngle)
        ) * radius;
        
        Vector3 targetPosition = target.position + offset;
        Vector3 direction = (targetPosition - transform.position).normalized;
        float detectionDistance = 5.0f; // 设定检测距离
    
        // 使用射线检测相机和目标之间的障碍物 
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, detectionDistance))
        {  
            // 如果检测到障碍物，停止移动
            Debug.Log("Obstacle detected, stopping movement.");
            radius = Mathf.Max(Vector3.Distance(transform.position, target.position), MinRadius);
        }
        else
        {
            transform.position = targetPosition;
        }
        
        // 根据垂直角度调整“上”方向
        Vector3 upDirection = currentVerticalAngle > Mathf.PI / 2 && currentVerticalAngle < 3 * Mathf.PI / 2 ? Vector3.down : Vector3.up;
        // 直接设置旋转，使目标物体始终在视野中心
        transform.LookAt(target, upDirection);
    }
 
    void OnCollisionEnter(Collision collision) {
        Debug.Log($"OrbitCameraController OnCollisionEnter {collision.gameObject.name}");
    }
} 

///Keyboard
public partial class OrbitCameraController : MonoBehaviour {
    public float keyboardSpeed = 20f; // 旋转速度

    private void HandleKeyboardInput()
    {
        var snapshot = InputCollector.Instance.GetSnapshot();
        var keyboardInputType = snapshot.GetKeyboardInputType();
        switch (keyboardInputType)
        {
            case InputSnapshot.KeyboardInputType.SingleKeyMove:
                GetNewPosition(snapshot.AxisInput, keyboardSpeed);
                break;
            case InputSnapshot.KeyboardInputType.ShiftKeyMove:
            case InputSnapshot.KeyboardInputType.AltKeyMove:
            default:
                break;
        }
    }
}

///Touch
public partial class OrbitCameraController : MonoBehaviour {
    public float touchSpeed = 2f; // 旋转速度
    private void HandleTouchInput()
    {
        var snapshot = InputCollector.Instance.GetSnapshot();
        var touchType = snapshot.GetCurrentTouchType();
        switch (touchType)
        {
            case InputSnapshot.TouchType.SingleFingerSwipe:
                GetNewPosition(snapshot.GetSingleFingerSwipeDelta(touchSpeed), touchSpeed);
                break;
            case InputSnapshot.TouchType.TwoFingerHoldAndSwipe:
            case InputSnapshot.TouchType.TwoFingerZoom:
            default:
                break;
        }
    }
}