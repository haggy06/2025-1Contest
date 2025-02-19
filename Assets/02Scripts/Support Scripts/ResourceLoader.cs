using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;

public static class ResourceLoader<T> where T : UnityEngine.Object
{
    private static Dictionary<Tuple<FolderName, string>, T> resourceCache = new Dictionary<Tuple<FolderName, string>, T>(); // 불러온 리소스  
    public static T ResourceLoad(FolderName folder, string resourceName)
    {
        T value;
        Tuple<FolderName, string> key = new Tuple<FolderName, string>(folder, resourceName);

        if (!resourceCache.TryGetValue(key, out value)) // 캐싱된 리소스가 있을 경우 불러옴
        { // 캐싱된 리소스가 없을 경우
            value = Resources.Load<T>(Path.Combine(folder.ToString(), resourceName)); // 리소스 로드

            resourceCache.Add(key, value); // 불러온 리소스 캐싱
        }

        if (value == null)
            Debug.LogError(resourceName + " 로드 실패");

        return value;
    }
}

public enum FolderName
{
    Player,

    BGM,
    Death,

    Singleton,

    Ect
}