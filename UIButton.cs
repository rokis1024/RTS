using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int index = 0;
    private bool workerSelected = false;
    private ButtonSystem BS;

    void Start()
    {
        BS = FindObjectOfType<ButtonSystem>();
    }

    public void SetButton(int indexValue, bool isWorkerSelected)
    {
        this.index = indexValue;
        this.workerSelected = isWorkerSelected;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        BS.ShowText(index, workerSelected);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BS.ClearText();
    }
}
