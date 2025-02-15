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
        if (!FileSaveLoader<GameData>.TryLoadData(FileName.GameData.ToString(), out _gameData)) // ����� �����͸� �������ų�. ���� ��� �ϳ� ���� ����
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
            if (_gameData == null) // �����͸� �ҷ����� �ʾ��� ���
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
            if (_settingData == null) // �����͸� �ҷ����� �ʾ��� ���
            {
                if (!FileSaveLoader<SettingData>.TryLoadData(FileName.SettingData.ToString(), out _settingData)) // ����� �����͸� �������ų�. ���� ��� �ϳ� ���� ����
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