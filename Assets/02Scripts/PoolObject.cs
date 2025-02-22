using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    [HideInInspector]
    public ObjectPool parentPool;

    protected void Awake()
    {
        GameManager.Inst.Subscribe(EventType.DayStart, DayStart);
    }
    protected virtual void DayStart()
    {
        if (isOut)
            ReturnToPool();
    }

    protected virtual void ObjectReturned()
    {
        gameObject.SetActive(false);
    }

    public virtual void ReturnToPool()
    {
        ObjectReturned();
        isOut = false;

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

    protected bool isOut = false;
    public virtual void Init(Vector2 position, float angle = 0f)
    {
        transform.position = position;

        transform.eulerAngles = Vector3.forward * angle;
    }
    public virtual void ExitFromPool()
    {
        isOut = true;
        gameObject.SetActive(true);
    }
}