using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DataManager
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SaveGameData()
    {
        FileSaveLoader<GameData>.SaveData(FileName.GameData.ToString(), _gameData);
    }
    public static void RefreshGameData()
    {
        if (!FileSaveLoader<GameData>.TryLoadData(FileName.GameData.ToString(), out _gameData)) // 저장된 데이터를 가져오거나. 없을 경우 하나 새로 만듦
        {
            ResetGameData();
        }
    }
    public static void ResetGameData()
    {
        DragItemList ItemDataList = ResourceLoader<DragItemList>.ResourceLoad(FolderName.Ect, "ItemDataList");
        Debug.Log(ItemDataList);

        _gameData = new GameData();
        for (int i = 0; i < _gameData.itemStatus.Length; i++)
        {
            _gameData.itemStatus[i] = new ItemCount(ItemDataList.dataList[i]);
        }

        SaveGameData();
    }

    private static GameData _gameData = null;
    public static GameData GameData
    {
        get
        {
            if (_gameData == null) // 데이터를 불러오지 않았을 경우
            {
                RefreshGameData();
            }
            return _gameData;
        }
    }

    private static SettingData _settingData = null;
    public static SettingData SettingData
    {
        get
        {
            if (_settingData == null) // 데이터를 불러오지 않았을 경우
            {
                if (!FileSaveLoader<SettingData>.TryLoadData(FileName.SettingData.ToString(), out _settingData)) // 저장된 데이터를 가져오거나. 없을 경우 하나 새로 만듦
                {
                    _settingData = new SettingData();
                    FileSaveLoader<SettingData>.SaveData(_settingData.ToString(), _settingData);
                }
            }

            return _settingData;
        }
    }
    public static void SaveSettingData()
    {
        FileSaveLoader<SettingData>.SaveData(FileName.SettingData.ToString(), _settingData);
    }
}

public enum FileName
{
    GameData,
    SettingData
}