using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HouseComponent {
    public static string WallX1 = "Wall_X1";
    public static string WallX2 = "Wall_X2";
    public static string WallZ1 = "Wall_Z1";
    public static string WallZ2 = "Wall_Z2";
    public static string Plane = "Plane";
}

public class HouseAppear : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetColor(HouseComponent.WallX1, new Color(0.8f, 0.6f, 0.4f)); // 柔和的棕色
        SetColor(HouseComponent.WallX2, new Color(0.6f, 0.8f, 0.4f)); // 柔和的绿色
        SetColor(HouseComponent.WallZ1, new Color(0.4f, 0.6f, 0.8f)); // 柔和的蓝色
        SetColor(HouseComponent.WallZ2, new Color(0.8f, 0.4f, 0.6f)); // 柔和的粉色
        SetColor(HouseComponent.Plane, new Color(0.9f, 0.9f, 0.9f));  // 柔和的白色
    }

    private void SetColor(string objectName, Color color)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj != null)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
        else
        {
            Debug.LogWarning($"Object {objectName} not found!");
        }
    }
}
