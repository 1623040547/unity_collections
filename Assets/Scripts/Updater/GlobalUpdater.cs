using UnityEngine;
public class GlobalUpdater : MonoBehaviour
{
    private void Awake()
    {
        // 确保只有一个InputManager实例
        if (FindObjectsOfType<GlobalUpdater>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    
    private void Update()
    {
        // 在每帧更新输入数据
        InputCollector.Instance.Update();
    }
}