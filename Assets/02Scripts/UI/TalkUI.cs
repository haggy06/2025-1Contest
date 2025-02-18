using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;
    public TextMeshProUGUI text => _text;
    [SerializeField]
    private Image face;

    public void ChangeFace(Sprite face)
    {
        if (this.face == null)
            return;

        this.face.sprite = face;
    }
}
