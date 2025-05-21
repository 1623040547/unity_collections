using UnityEngine;

public partial class InputSnapshot
{

    public enum TouchType
    {
        None,
        SingleFingerSwipe,
        TwoFingerHoldAndSwipe,
        TwoFingerZoom
    }

    /// <summary>
    /// 获取当前的触摸类型
    /// </summary>
    /// <returns>当前触摸类型的枚举值</returns>
    public TouchType GetCurrentTouchType()
    {
        if (TouchCount == 1)
        {
            return TouchType.SingleFingerSwipe;
        }
        else if (TouchCount == 2)
        {
            if (IsOneFingerStationary())
            {
                return TouchType.TwoFingerHoldAndSwipe;
            }
            else
            {
                return TouchType.TwoFingerZoom;
            }
        }
        return TouchType.None;
    }

    /// <summary>
    /// 获取单指滑动的增量
    /// </summary>
    /// <param name="sensitivity">滑动灵敏度</param>
    /// <returns>滑动增量</returns>
    public Vector2 GetSingleFingerSwipeDelta(float sensitivity)
    {
        if (TouchCount == 1)
        {
            return Touches[0].deltaPosition * sensitivity;
        }
        return Vector2.zero;
    }

    /// <summary>
    /// 获取双指时一指长按+一指滑动的增量
    /// </summary>
    /// <param name="sensitivity">滑动灵敏度</param>
    /// <returns>滑动增量</returns>
    public Vector2 GetTwoFingerHoldAndSwipeDelta(float sensitivity)
    {
        if (TouchCount == 2)
        {
            // 假设第一个手指长按，第二个手指滑动
            return Touches[1].deltaPosition * sensitivity;
        }
        return Vector2.zero;
    }

    /// <summary>
    /// 获取双指缩放增量
    /// </summary>
    /// <param name="sensitivity">缩放灵敏度</param>
    /// <returns>缩放增量</returns>
    public float GetTwoFingerZoomDelta(float sensitivity)
    {
        if (TouchCount == 2)
        {
            Vector2 touch0PrevPos = Touches[0].position - Touches[0].deltaPosition;
            Vector2 touch1PrevPos = Touches[1].position - Touches[1].deltaPosition;

            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
            float touchDeltaMag = (Touches[0].position - Touches[1].position).magnitude;

            return (touchDeltaMag - prevTouchDeltaMag) * sensitivity;
        }
        return 0f;
    }

    /// <summary>
    /// 检查是否有一个手指保持静止
    /// </summary>
    /// <returns>如果有一个手指静止，返回 true；否则返回 false</returns>
    private bool IsOneFingerStationary()
    {
     return  Vector2.Distance(Touches[0].deltaPosition, Vector2.zero) < 0.5f || 
          Vector2.Distance(Touches[1].deltaPosition, Vector2.zero) < 0.5f;
    }
}