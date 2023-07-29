using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonImageEffect : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Image buttonImage;
    public Sprite selectedImage;

    private Sprite normalImage;

    void Start()
    {
        normalImage = buttonImage.sprite;
    }

    public void OnSelect(BaseEventData eventData)
    {
        buttonImage.sprite = selectedImage;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        buttonImage.sprite = normalImage;
    }
}

