using UnityEngine;
using UnityEngine.UI;

public class EndCanvasManager : CanvasManager
{
    [SerializeField]
    private Image endImage;
    [SerializeField]
    private Sprite[] endScenes;

    [Space(5)]
    [SerializeField]
    private GameObject mainMoveBtn;
    protected new void Awake()
    {
        base.Awake();
        mainMoveBtn.SetActive(false);

        int spriteIndex;
        print(GameManager.Inst.endingType);
        switch (GameManager.Inst.endingType)
        {
            case StoryTalkIndex.TrapEnd:
                spriteIndex = 0;
                break;
            case StoryTalkIndex.BankruptcyEnd: 
                spriteIndex = 1; 
                break;
            case StoryTalkIndex.NormalEnd: 
                spriteIndex = 2;
                break;
            case StoryTalkIndex.HappyEnd: 
                spriteIndex = 3; 
                break;
            
            default: 
                spriteIndex = 0; 
                break;
        }

        endImage.sprite = endScenes[spriteIndex];

        Invoke("MainMove", 3f);
    }

    private void MainMove()
    {
        DataManager.ResetGameData();
        mainMoveBtn.SetActive(true);
    }
}
