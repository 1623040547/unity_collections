using UnityEngine;

public partial class CameraController : MonoBehaviour
{
    // 垂直旋转的最大和最小角度限制
    public float minVerticalAngle = -80f;  // 修改为更合理的角度范围
    public float maxVerticalAngle = 180f;   // 修改为更合理的角度范围

    // 当前垂直旋转角度
    private float currentVerticalRotation = 0f;  // 从0开始

    // 相机的最小和最大视野
    public float minFOV = 15f;
    public float maxFOV = 90f;

    private Rigidbody rb;
    private SphereCollisionDetector collisionDetector;

    void Start()
    {
        // 获取或添加Rigidbody组件
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        // 设置Rigidbody属性
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.freezeRotation = true; // 防止物理系统影响旋转
        rb.mass = 1f;
        rb.drag = 10f; // 添加一些阻力以使移动更平滑

        // 初始化球形碰撞检测器
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        float radius = sphereCollider != null ? sphereCollider.radius : 2f;
        collisionDetector = new SphereCollisionDetector(radius, rb);
    }

    void Update()
    {
        HandleTouchInput();
        HandleKeyboardInput();
    }

    private void HandleHorizontalRotation(float horizontal)
    {
        float horizontalRotation = horizontal * keyboardRotationSpeed * Time.deltaTime;
        transform.Rotate(0, horizontalRotation, 0, Space.World);
    }

    private void HandleVerticalRotation(float vertical)
    {
        // 累加垂直旋转值
        currentVerticalRotation -= vertical * keyboardRotationSpeed * Time.deltaTime;

        // 限制垂直旋转角度在合理范围内
        currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, minVerticalAngle, maxVerticalAngle);

        // 应用旋转，保持y和z轴的当前值不变
        Vector3 angles = transform.eulerAngles;
        transform.localRotation = Quaternion.Euler(currentVerticalRotation, angles.y, 0);
    }
}

///屏幕触控
public partial class CameraController : MonoBehaviour
{

    public float touchRotationSpeed = 0.2f;

    public float touchMoveSpeed = 2f;

    public float touchZoomSpeed = 0.01f;

    private void HandleTouchInput()
    {
        var snapshot = InputCollector.Instance.GetSnapshot();
        var touchType = snapshot.GetCurrentTouchType();

        switch (touchType)
        {
            case InputSnapshot.TouchType.SingleFingerSwipe:
                HandleTouchRotation(snapshot.GetSingleFingerSwipeDelta(touchRotationSpeed));
                break;
            case InputSnapshot.TouchType.TwoFingerHoldAndSwipe:
                HandleTouchMovement(snapshot.GetTwoFingerHoldAndSwipeDelta(touchMoveSpeed));
                break;
            case InputSnapshot.TouchType.TwoFingerZoom:
                HandleTouchZoom(snapshot.GetTwoFingerZoomDelta(touchZoomSpeed));
                break;
            default:
                break;
        }
    }

    private void HandleTouchMovement(Vector2 translationDelta)
    {
        Vector3 movement = new Vector3(translationDelta.x, 0f, translationDelta.y);
        movement = transform.TransformDirection(movement);

        // 使用碰撞检测器进行移动
        collisionDetector.TryMove(movement, keyboardMoveSpeed, Time.fixedDeltaTime);
    }

    private void HandleTouchRotation(Vector2 rotationDelta)
    {
        HandleHorizontalRotation(rotationDelta.x);
        HandleVerticalRotation(rotationDelta.y);
    }

    private void HandleTouchZoom(float zoomDelta)
    {
        // 调整相机的视野
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - zoomDelta, minFOV, maxFOV);
    }
}

///键盘触控
public partial class CameraController : MonoBehaviour
{


    // 相机移动速度
    public float keyboardMoveSpeed = 20f;

    // 相机旋转速度
    public float keyboardRotationSpeed = 20f;
    // 相机视野缩放灵敏度
    public float keyboardZoomSensitivity = 4f;

    private void HandleKeyboardInput()
    {
        var snapshot = InputCollector.Instance.GetSnapshot();
        var keyboardInputType = snapshot.GetKeyboardInputType();
        switch (keyboardInputType)
        {
            case InputSnapshot.KeyboardInputType.SingleKeyMove:
                HandleKeyboardRotation(snapshot.GetKeyboardInputDelta() * keyboardMoveSpeed);
                break;
            case InputSnapshot.KeyboardInputType.ShiftKeyMove:
                HandleKeyboardMovement(snapshot.GetKeyboardInputDelta() * keyboardMoveSpeed * 1.5f); // 加速移动
                break;
            case InputSnapshot.KeyboardInputType.AltKeyMove:
                HandleKeyboardZoom(snapshot.GetKeyboardInputDelta().y * keyboardZoomSensitivity);
                break;
            default:
                break;
        }
    }

    private void HandleKeyboardMovement(Vector2 translationDelta)
    {
        Vector3 movement = new Vector3(translationDelta.x, 0f, translationDelta.y);
        movement = transform.TransformDirection(movement);

        // 使用碰撞检测器进行移动
        collisionDetector.TryMove(movement, keyboardMoveSpeed, Time.fixedDeltaTime);
    }

    private void HandleKeyboardRotation(Vector2 rotationDelta)
    {

        HandleHorizontalRotation(rotationDelta.x);
        HandleVerticalRotation(rotationDelta.y);
    }

    private void HandleKeyboardZoom(float zoomDelta)
    {
        // 调整相机的视野
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - zoomDelta, minFOV, maxFOV);
    }


}
