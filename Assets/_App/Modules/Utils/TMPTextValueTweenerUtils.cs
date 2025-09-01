using System;
using DG.Tweening;
using TMPro;

public class TMPTextValueTweenUtils
{
	public static void TweenValue(TextMeshProUGUI text, int fromValue, int toValue, float duration,
		Action onComplete = null)
	{
		DOTween.To(() => fromValue, x => text.text = x.ToString(), toValue, duration).SetEase(Ease.InCubic)
			.OnComplete(() => { onComplete?.Invoke(); });
	}
}

