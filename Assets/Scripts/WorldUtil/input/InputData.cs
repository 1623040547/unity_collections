using UnityEngine;
using System;

/// <summary>
/// 表示用户输入的原始数据快照
/// 不可变的数据结构，确保数据的一致性
/// </summary>
public partial class InputSnapshot
{
    public readonly Vector2 AxisInput;
    public readonly Touch[] Touches;
    public readonly int TouchCount;
    private readonly int KeyStates;
    
    internal InputSnapshot(Vector2 axisInput, Touch[] touches, int keyStates)
    {
        AxisInput = axisInput;
        TouchCount = touches.Length;
        // 创建触摸数据的副本，防止外部修改
        Touches = new Touch[TouchCount];
        if (TouchCount > 0)
        {
            Array.Copy(touches, Touches, TouchCount);
        }
        KeyStates = keyStates;
    }
    
    public bool GetKeyState(KeyCode key)
    {
        int keyIndex = (int)key % 32;
        return (KeyStates & (1 << keyIndex)) != 0;
    }
}

/// <summary>
/// 输入数据收集器
/// 负责收集和管理输入数据
/// </summary>
public class InputCollector
{
    private static InputCollector instance;
    private Touch[] touchBuffer = new Touch[0];
    private int keyStates;
    private Vector2 axisInput;  // 添加轴输入缓存
    
    public static InputCollector Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new InputCollector();
            }
            return instance;
        }
    }
    
    private InputCollector() { }

    public void Update()
    {
        // 更新轴输入
        axisInput = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        // 更新触摸缓冲区
        if (Input.touchCount > 0)
        {
            if (touchBuffer.Length != Input.touchCount)
            {
                touchBuffer = new Touch[Input.touchCount];
            }
            for (int i = 0; i < Input.touchCount; i++)
            {
                touchBuffer[i] = Input.GetTouch(i);
            }
        }
        else if (touchBuffer.Length != 0)
        {
            touchBuffer = new Touch[0];
        }

        // 更新按键状态
        SetKeyState(KeyCode.LeftShift, Input.GetKey(KeyCode.LeftShift));
        SetKeyState(KeyCode.RightShift, Input.GetKey(KeyCode.RightShift)); 
        // 检测Alt键
        SetKeyState(KeyCode.LeftAlt, Input.GetKey(KeyCode.LeftAlt)); 
        SetKeyState(KeyCode.RightAlt, Input.GetKey(KeyCode.RightAlt)); 
        // 检测箭头键
        SetKeyState(KeyCode.UpArrow, Input.GetKey(KeyCode.UpArrow));
        SetKeyState(KeyCode.DownArrow, Input.GetKey(KeyCode.DownArrow));  
        SetKeyState(KeyCode.LeftArrow, Input.GetKey(KeyCode.LeftArrow)); 
        SetKeyState(KeyCode.RightArrow, Input.GetKey(KeyCode.RightArrow)); 
        ///空格键
        SetKeyState(KeyCode.Space, Input.GetKey(KeyCode.Space)); 
    }

    private void SetKeyState(KeyCode key, bool state)
    {
        int keyIndex = (int)key % 32;
        if (state)
            keyStates |= (1 << keyIndex);
        else
            keyStates &= ~(1 << keyIndex);
    }
    
    /// <summary>
    /// 获取当前输入状态的快照
    /// </summary>
    public InputSnapshot GetSnapshot()
    {
        return new InputSnapshot(
            axisInput,  // 使用缓存的轴输入
            touchBuffer,
            keyStates
        );
    }
}