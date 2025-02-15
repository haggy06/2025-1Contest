using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataList", menuName = "Scriptable Objects/ItemDataList")]
public class DragItemList : ScriptableObject
{
    [SerializeField]
    private DragItem[] _dataList;
    public DragItem[] dataList => _dataList;
}
