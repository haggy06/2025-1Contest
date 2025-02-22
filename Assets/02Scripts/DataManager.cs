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
        if (!FileSaveLoader<GameData>.TryLoadData(FileName.GameData.ToString(), out _gameData)) // 저장된 데이터를 가져오거나. 없을 경우 하나 새로 만듦
        {
            ResetGameData();
        }
        else // 가끔 instanceID가 바뀌는지 item 인식이 안 될 때가 있어서 초기화시켜줌
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
            if (_gameData == null) // 데이터를 불러오지 않았을 경우
            {
                RefreshGameData();
            }
            return _gameData;
        }
    }

    // AudioManager에서 PlayerPrefs 사용하기로 결정!
    /*
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
    */
}

public enum FileName
{
    GameData,
    SettingData
}