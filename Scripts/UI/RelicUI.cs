using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using R3;
using DG.Tweening;
using TMPro;

public class RelicUI : MonoBehaviour
{
    private Image _relicImage;
    private RelicData _relicData;
    
    public void SetRelicData(RelicData r)
    {
        this._relicData = r;
        _relicImage = this.GetComponent<Image>();
        _relicImage.sprite = _relicData.image;
        
        // イベントを登録
        // this.gameObject.AddDescriptionWindowEvent(r);
    }
}