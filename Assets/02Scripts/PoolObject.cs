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
            Debug.Log("자식 형태의 PoolObject는 반납되지 않음");
            return;
        }

        ObjectReturned();

        try
        {
            parentPool.SetPoolObject(this);
        }
        catch (System.NullReferenceException) 
        {
            Debug.Log(name + "에 연결된 풀 없음");
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