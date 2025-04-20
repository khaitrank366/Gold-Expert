using UnityEngine;
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    // Property để truy cập instance
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                // Nếu chưa có instance, tạo một đối tượng mới
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).ToString());
                    _instance = singletonObject.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    // Khởi tạo
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;  // Gán this thành instance của T
        }
        else if (_instance != this)
        {
            Destroy(gameObject);  // Nếu instance đã tồn tại, hủy đi object mới
        }

        DontDestroyOnLoad(gameObject);  // Giữ Singleton sống khi chuyển cảnh
    }
}