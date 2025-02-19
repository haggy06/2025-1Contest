using UnityEngine;

using System.IO;

public enum DataType
{
    SettingData,
    PlayData,
    EctData,

}
public static class FileSaveLoader<T> where T : class
{
    public static void SaveData(string fileName, T nData)
    {
        Debug.Log(fileName + " 파일을 저장합니다.");

        File.WriteAllText(Path.Combine(Application.persistentDataPath, fileName), JsonUtility.ToJson(nData));
    }

    public static bool TryLoadData(string fileName, out T data)
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, fileName))) // 경로에 파일이 존재할 경우
        {
            Debug.Log("저장된 " + fileName + " 파일을 가져오는 데 성공했습니다.");

            data = JsonUtility.FromJson<T>(File.ReadAllText(Path.Combine(Application.persistentDataPath, fileName)));
            if (data == null)
            {
                Debug.LogError("저장된 " + fileName + "파일이 손상되어 새로 만듭니다.");

                DelectData(fileName);
                return TryLoadData(fileName, out data);
            }

            return true;
        }
        else // 경로에 파일이 존재하지 않을 경우
        {
            Debug.Log("저장된 " + fileName + " 파일이 존재하지 않습니다.");

            data = null;
            return false;
        }
    }

    public static void DelectData(string fileName)
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, fileName.ToString()))) // 경로에 파일이 존재할 경우
        {
            File.Delete(Path.Combine(Application.persistentDataPath, fileName.ToString()));

            Debug.Log(fileName + " 파일을 삭제했습니다.");
        }
        else // 경로에 파일이 존재하지 않을 경우
        {
            Debug.Log(fileName + " 파일이 존재하지 않습니다.");
        }
    }
}
