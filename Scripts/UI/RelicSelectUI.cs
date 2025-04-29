using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelicSelectUI : MonoBehaviour
{
    [SerializeField] private List<RelicSelectButton> relicUIs;

    public void SetRandomItem(bool onlyFlower)
    {
        var (r1, r2, r3) = RelicManager.Instance.GetRandomRelic(onlyFlower);
        relicUIs[0].SetRelicData(r1);
        relicUIs[1].SetRelicData(r2);
        relicUIs[2].SetRelicData(r3);
    }
}
