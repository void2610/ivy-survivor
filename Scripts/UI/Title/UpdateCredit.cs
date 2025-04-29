using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpdateCredit : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI text;
    
    private void Start()
    {
        // URL をリンクとして扱えるようにする
        text.text = ConvertUrlsToLinks(text.text);
    }

    /// <summary>
    /// クリック時にリンク部分を判定し、URLを開く
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        var linkIndex = TMP_TextUtilities.FindIntersectingLink(text, eventData.position, Camera.main);
        if (linkIndex != -1)
        {
            var linkInfo = text.textInfo.linkInfo[linkIndex];
            var url = linkInfo.GetLinkID();
            Application.OpenURL(url);
        }
    }

    /// <summary>
    /// テキスト内のURLをTextMeshProのリンク形式に変換
    /// </summary>
    private string ConvertUrlsToLinks(string textData)
    {
        return System.Text.RegularExpressions.Regex.Replace(
            textData,
            @"(http[s]?:\/\/[^\s]+)",
            "<link=\"$1\"><u>$1</u></link>"
        );
    }
}
