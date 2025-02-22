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

    private static DragItemList _itemDataList;
    public static DragItemList itemDataList => _itemDataList;

    public static void RefreshGameData()
    {
        if (!FileSaveLoader<GameData>.TryLoadData(FileName.GameData.ToString(), out _gameData)) // ����� �����͸� �������ų�. ���� ��� �ϳ� ���� ����
        {
            ResetGameData();
        }
        else // ���� instanceID�� �ٲ���� item �ν��� �� �� ���� �־ �ʱ�ȭ������
        {
            _itemDataList = ResourceLoader<DragItemList>.ResourceLoad(FolderName.Ect, "ItemDataList");
            for (int i = 0; i < _gameData.itemStatus.Length; i++)
            {
                _gameData.itemStatus[i].item = _itemDataList.dataList[i];
            }
        }
    }
    public static void ResetGameData()
    {
        _itemDataList = ResourceLoader<DragItemList>.ResourceLoad(FolderName.Ect, "ItemDataList");
        Debug.Log(_itemDataList);

        _gameData = new GameData();
        for (int i = 0; i < _gameData.itemStatus.Length; i++)
        {
            _gameData.itemStatus[i] = new ItemCount(_itemDataList.dataList[i]);
        }
        _gameData.spyNPC.RandomFace();
        _gameData.spyNPC.RandomOrder();
        _gameData.spyNPC.npcType = NPCType.Spy;

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

    // AudioManager���� PlayerPrefs ����ϱ�� ����!
    /*
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
    */
}

public enum FileName
{
    GameData,
    SettingData
}