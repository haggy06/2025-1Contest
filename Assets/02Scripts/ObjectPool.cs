using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField, Tooltip("처음 풀을 만들 때 몇 개를 만들어 둘 지 정하기")]
    private int initialNumber = 1;
    [SerializeField]
    private PoolObject[] initialObject;
    private void Start()
    {
        foreach (PoolObject obj in initialObject)
        {
            CreatePool(obj);
        }
    }

    private Dictionary<string, Stack<PoolObject>> poolDictionary = new Dictionary<string, Stack<PoolObject>>();


    public T GetPoolObject<T>(T poolObject) where T : PoolObject
    {
        PoolObject value;

        if (!poolDictionary.TryGetValue(poolObject.name, out Stack<PoolObject> pool)) // poolDictionary에서 poolObject에 해당하는 풀을 불러옴
        {
            //  poolDictionary에 poolObject에 해당하는 풀이 없을 경우
            print("풀 새로 제작");
            pool = CreatePool(poolObject); // 풀 새로 제작 후 참조
        }

        if (!pool.TryPop(out value)) // 불러온 풀에서 오브젝트 하나 꺼내옴
        {
            // 오브젝트를 꺼낼 수 없을 경우
            value = Instantiate(poolObject.gameObject).GetComponent<T>(); // 오브젝트를 하나 새로 만들고
            value.name = poolObject.name;
            value.parentPool = this; // 이 풀을 기억시켜둠
        }

        value.transform.parent = null;
        value.ExitFromPool();
        return (T)value;
    }

    public void SetPoolObject(PoolObject poolObject)
    {
        if (poolDictionary.TryGetValue(poolObject.name, out Stack<PoolObject> pool)) // poolDictionary에서 poolObject에 해당하는 풀을 불러옴
        {
            pool.Push(poolObject); // 풀에 반환된 오브젝트 넣어 둠
            if (poolObjectContainers.TryGetValue(poolObject.name, out Transform container)) // 해당하는 컨테이너를 가져와
            {
                poolObject.transform.parent = container; // 부모로 삼게 함
            }
            else // 알맞은 컨테이너가 없을 경우
            {
                Debug.LogWarning(poolObject.name + "에 해당하는 컨테이너 없음");

                container = new GameObject().transform;
                container.parent = transform;
                container.name = poolObject.name + " Pool";
                poolObjectContainers.Add(poolObject.name, container);

                poolObject.transform.parent = container; // 부모로 삼게 함
            }
        }
        else // poolDictionary에 poolObject에 해당하는 풀이 없을 경우
        {
            Debug.LogError("풀을 잘못 찾아오신 듯 합니다.");
        }
    }

    private Dictionary<string, Transform> poolObjectContainers = new Dictionary<string, Transform>(); // 오브젝트가 반환되었을 때 넣어둘 컨테이너들
    private Stack<PoolObject> CreatePool(PoolObject poolObject)
    {
        Stack<PoolObject> pool;

        if (poolDictionary.TryGetValue(poolObject.name, out pool)) // 이미 풀이 있을 경우 불러옴
        {
            Debug.Log("이미 " + poolObject.name + "에 대한 오브젝트 풀이 있음.");
        }
        else // 풀이 없을 경우
        {
            pool = new Stack<PoolObject>();
            poolDictionary.Add(poolObject.name, pool);

            Transform container = new GameObject().transform;
            container.parent = transform;
            container.name = poolObject.gameObject.name + " Pool";
            poolObjectContainers.Add(poolObject.name, container);

            for (int i = 0; i < initialNumber; i++) // 초기 생성 수만큼 반복
            {
                PoolObject obj = Instantiate(poolObject.gameObject, container).GetComponent<PoolObject>(); // 오브젝트를 컨테이너 자식으로 새로 만들고
                obj.name = poolObject.name;

                obj.parentPool = this; // 이 풀을 기억시킨 후
                SetPoolObject(obj); // 풀에 넣어둠
                obj.gameObject.SetActive(false);
            }
        }

        return pool;
    }
}
