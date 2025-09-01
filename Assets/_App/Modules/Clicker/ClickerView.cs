using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.Progress;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;
using Zenject;
using DG.Tweening;

public class ClickerView : MonoBehaviour
{
    public UIButton ClickButton;
    
    [SerializeField] private TextMeshProUGUI _energyText;
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private Progressor _progressor;
    [SerializeField] private GameObject _star;
    
    private Sequence _starAnimation;
    private Vector3 _starOriginalPosition;
    
    [Inject] private ClickerService _service;
    
    void Awake()
    {
        _service.Initialize(this);
        
        _starOriginalPosition = _star.transform.localPosition;
        _star.SetActive(false);
        
        ClickButton.OnClick.OnTrigger.Event.AddListener(PlayStarAnimation);
    }
    
    public void SetEnergy(int energy, int maxEnergy, bool instantSetProgress = false)
    {
        float progress = (float)energy / maxEnergy;
        
        if (instantSetProgress)
        {
            _progressor.InstantSetProgress(progress);
        }
        else
        {
            _progressor.SetProgress(progress);
        }
        
        _energyText.text = $"{energy}/{maxEnergy}";
    }
    
    public void SetCoin(int coin)
    {
        _coinText.text = coin.ToString();
    }
    
    public void SetClickButtonEnabled(bool interactable)
    {
        ClickButton.Interactable = interactable;
    }
    
    private void PlayStarAnimation()
    {
        _starAnimation?.Kill();
        _star.transform.localPosition = _starOriginalPosition;
        _star.transform.localScale = Vector3.zero;
        _star.SetActive(true);
        
        _starAnimation = DOTween.Sequence()
            .Append(_star.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack))
            .Join(_star.transform.DOLocalMoveY(_starOriginalPosition.y + 400f, 0.3f).SetEase(Ease.OutQuart))
            .Append(_star.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack))
            .OnComplete(() => _star.SetActive(false));
    }
    
    void OnDestroy()
    {
        ClickButton?.OnClick.OnTrigger.Event.RemoveListener(PlayStarAnimation);
        _starAnimation?.Kill();
    }
}
