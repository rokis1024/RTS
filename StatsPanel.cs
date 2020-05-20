using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatsPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private CanvasGroup statsPanel;
    public int statNr;

    void Start()
    {
        if(statNr <= 0)
        {
            statsPanel = GameObject.Find("StatsPanel").GetComponent<CanvasGroup>();
        }
        else
        {
            statsPanel = GameObject.Find("StatsPanel2").GetComponent<CanvasGroup>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        statsPanel.alpha = 1;
        statsPanel.interactable = true;
        statsPanel.blocksRaycasts = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        statsPanel.alpha = 0;
        statsPanel.interactable = false;
        statsPanel.blocksRaycasts = false;
    }
}
