using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoleComponent {
    public static string Role = "Role";
}

public class RoleAppear: MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetSkinColor(new Color(1.0f, 0.8f, 0.6f)); // 设置为接近人体的肤色
    }

    private void SetSkinColor(Color color)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
        else
        {
            Debug.LogWarning("Renderer component not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
