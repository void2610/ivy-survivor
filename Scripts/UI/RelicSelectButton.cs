using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelicSelectButton : MonoBehaviour
{
    private Button _button;
    private TMP_Text _relicNameText;
    private TMP_Text _relicDescriptionText;
    private Image _relicImage;
    
    public void SetRelicData(RelicData relicData)
    {
        _relicNameText.text = relicData.displayName;
        _relicDescriptionText.text = relicData.description;
        _relicImage.sprite = relicData.image;
        
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() =>
        {
            RelicManager.Instance.AddRelic(relicData);
            GameManager.Instance.ChangeState(GameManager.GameStateType.StageMoving);
        });
    }
    
    private void Awake()
    {
        _button = this.GetComponent<Button>();
        _relicNameText = this.transform.Find("RelicNameText").GetComponent<TMP_Text>();
        _relicDescriptionText = this.transform.Find("RelicDescriptionText").GetComponent<TMP_Text>();
        _relicImage = this.transform.Find("RelicImage").GetComponent<Image>();
    }
}
