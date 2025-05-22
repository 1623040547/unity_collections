using UnityEngine;

public partial class RoleMovement : MonoBehaviour
{
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.freezeRotation = true;

        capsuleCollider = GetComponent<CapsuleCollider>();
        if (capsuleCollider == null)
        {
            capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        }
        capsuleCollider.radius = 1.0f;
        capsuleCollider.height = 1.2f;
    }

    void Update()
    {
        HandleKeyboardInput();
        HandleTouchInput();
    }

    private void GetNewPosition(Vector2 delta, float speed)
    {
        // 计算角色自身的前进方向
        Vector3 forward = transform.forward;
        forward.y = 0; // 忽略Y轴，使其在地面上
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0; // 忽略Y轴，使其在地面上
        right.Normalize();

        Vector3 movement = forward * delta.y + right * delta.x;

        // 使用射线检测进行碰撞检测
        if (!WillCollide(movement, speed))
        {
            rb.MovePosition(rb.position + movement * speed * Time.deltaTime);
            Debug.Log("GetNewPosition MovePosition");
        } 
    }

    private bool WillCollide(Vector3 movement, float speed)
    {
        Vector3 start = transform.position + Vector3.up * (capsuleCollider.height / 2);
        float detectionDistance = speed * Time.deltaTime + capsuleCollider.radius + 2.0f;

        // 使用射线检测
        return Physics.Raycast(start, movement, out RaycastHit hit, detectionDistance);
    }
    void OnCollisionEnter(Collision collision) {
        Debug.Log($"RoleMovement OnCollisionEnter {collision.gameObject.name}");
        
        // 停止角色移动
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}

///Keyboard
public partial class RoleMovement : MonoBehaviour
{
    public float keyboardSpeed = 20f; // 键盘移动速度

    private void HandleKeyboardInput()
    {
        var snapshot = InputCollector.Instance.GetSnapshot();
        var keyboardInputType = snapshot.GetKeyboardInputType();
        switch (keyboardInputType)
        {
            case InputSnapshot.KeyboardInputType.ShiftKeyMove:
                GetNewPosition(snapshot.AxisInput, keyboardSpeed);
                break;
            case InputSnapshot.KeyboardInputType.SingleKeyMove:
            case InputSnapshot.KeyboardInputType.AltKeyMove:
            default:
                break;
        }
    }
}

///Touch
public partial class RoleMovement : MonoBehaviour
{
    public float touchSpeed = 3f; // 触控移动速度

    private void HandleTouchInput()
    {
        var snapshot = InputCollector.Instance.GetSnapshot();
        var touchType = snapshot.GetCurrentTouchType();
        switch (touchType)
        { 
            case InputSnapshot.TouchType.TwoFingerHoldAndSwipe:
                GetNewPosition(snapshot.GetTwoFingerHoldAndSwipeDelta(touchSpeed), touchSpeed);
                break;
            case InputSnapshot.TouchType.SingleFingerSwipe:
            case InputSnapshot.TouchType.TwoFingerZoom:
            default:
                break;
        }
    }
}