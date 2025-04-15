using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableSpell : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Spell Spell { get; private set; }

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Transform _originalParent;

    private Canvas _canvas;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _canvas = GetComponentInParent<Canvas>();
    }

    public void SetSpell(Spell spell)
    {
        Spell = spell;
    }

    public void Enable(Spell spell)
    {
        Spell = spell;
        gameObject.SetActive(true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalParent = transform.parent;
        transform.SetParent(_canvas.transform);
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(_originalParent);
        _canvasGroup.blocksRaycasts = true;
        _rectTransform.anchoredPosition = Vector2.zero;
    }
}
