using UnityEngine;

public partial class InputSnapshot
{
    public enum KeyboardInputType
    {
        None,
        SingleKeyMove,
        ShiftKeyMove,
        AltKeyMove
    }

    /// <summary>
    /// 获取当前的键盘输入类型
    /// </summary>
    /// <returns>当前键盘输入类型的枚举值</returns>
    public KeyboardInputType GetKeyboardInputType()
    {
        bool shiftPressed = GetKeyState(KeyCode.LeftShift) || GetKeyState(KeyCode.RightShift);
        bool spacePressed = GetKeyState(KeyCode.Space);

        if (shiftPressed)
        {
            return KeyboardInputType.ShiftKeyMove;
        }
        else if (spacePressed)
        {
            return KeyboardInputType.AltKeyMove;
        }
        else if (GetKeyState(KeyCode.UpArrow) || GetKeyState(KeyCode.DownArrow) || GetKeyState(KeyCode.LeftArrow) || GetKeyState(KeyCode.RightArrow))
        {
            return KeyboardInputType.SingleKeyMove;
        }
        return KeyboardInputType.None;
    }

    /// <summary>
    /// 获取当前的轴输入变化
    /// </summary>
    /// <returns>轴输入的变化量</returns>
    public Vector2 GetKeyboardInputDelta()
    {
        return AxisInput;
    }
}