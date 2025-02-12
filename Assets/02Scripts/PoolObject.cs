using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    [SerializeField]
    private bool destroyWhenBomb = true;
    [HideInInspector]
    public ObjectPool parentPool;

    protected virtual void ObjectReturned()
    {
        gameObject.SetActive(false);
    }
    protected virtual void DestroyByBomb()
    {
        ReturnToPool();
    }

    public virtual void ReturnToPool()
    {
        if (transform.root != transform && transform.root.TryGetComponent<PoolObject>(out _))
        {
            Debug.Log("�ڽ� ������ PoolObject�� �ݳ����� ����");
            return;
        }

        ObjectReturned();

        try
        {
            parentPool.SetPoolObject(this);
        }
        catch (System.NullReferenceException) 
        {
            Debug.Log(name + "�� ����� Ǯ ����");
            Destroy(gameObject);
        }
    }
    public virtual void Init(Vector2 position, float angle = 0f)
    {
        transform.position = position;

        transform.eulerAngles = Vector3.forward * angle;
    }
    public virtual void ExitFromPool()
    {
        gameObject.SetActive(true);
    }
}