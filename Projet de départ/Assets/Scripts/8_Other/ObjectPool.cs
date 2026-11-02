using UnityEngine;

// Object pool.
//
// Vous n'avez pas à toucher cette classe pour le travail.
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int instancesCount = 10;

    private void Awake()
    {
        for (var i = 0; i < instancesCount; i++)
        {
            Create(isActive: false);
        }
    }

    public GameObject Get()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var instance = transform.GetChild(i).gameObject;
            if (instance.activeSelf) continue;
            instance.SetActive(true);
            return instance;
        }

        return Create(isActive: true);
    }

    public GameObject Place(Vector3 position)
    {
        return Place(position, Quaternion.identity);
    }

    public GameObject Place(Vector3 position, Quaternion rotation)
    {
        var instance = Get();
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        return instance;
    }

    public T Get<T>() where T : Component
    {
        var instance = Get().GetComponent<T>();
        Debug.Assert(instance != null, $"Object from pool {name} doesn't have a component of type {typeof(T)}.");
        return instance;
    }

    public T Place<T>(Vector3 position) where T : Component
    {
        return Place<T>(position, Quaternion.identity);
    }

    public T Place<T>(Vector3 position, Quaternion rotation) where T : Component
    {
        var instance = Get<T>();
        instance.transform.position = position;
        instance.transform.rotation = rotation;
        return instance;
    }

    public void Release(GameObject instance)
    {
        // Disable the instance.
        instance.SetActive(false);
        
        // Put object in pool, in case it was moved.
        instance.transform.parent = transform;
    }

    private GameObject Create(bool isActive)
    {
        var instance = Instantiate(prefab, transform);
        instance.SetActive(isActive);
        return instance;
    }
}