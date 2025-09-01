using Doozy.Engine.UI;
using System;
using UnityEngine.Events;

public static class UIButtonUtil
{
	public static void AddOnPointerDownHandler(UIButton button, Action handler)
	{
		if (button == null)
		{
			return;
		}

		SetButtonSettings(button, true, false);
		button.OnPointerDown.OnTrigger.Action += (obj) => handler?.Invoke();
	}

	public static void AddOnPointerUpHandler(UIButton button, Action handler)
	{
		if (button == null)
		{
			return;
		}

		SetButtonSettings(button, false, true);
		button.OnPointerUp.OnTrigger.Action += (obj) => handler?.Invoke();
	}

	public static void AddOnClickHandler(UIButton button, Action handler)
	{
		if (button != null)
		{
			button.AllowMultipleClicks = false;
			button.OnClick.Enabled = true;
			button.OnClick.OnTrigger.Action += (obj) => handler?.Invoke();
		}
	}

	private static void SetButtonSettings(UIButton button, bool onPointerDownEnabled, bool onPointerUpEnabled)
	{
		button.AllowMultipleClicks = false;
		button.OnClick.Enabled = true;
		button.OnPointerDown.Enabled = onPointerDownEnabled;
		button.OnPointerUp.Enabled = onPointerUpEnabled;
	}
	
	public static void AddOnValueChangedHandler(UIToggle toggle, UnityAction<bool> handler)
	{
		if (toggle != null)
		{
			toggle.AllowMultipleClicks = false;
			toggle.OnClick.Enabled = true;
			toggle.OnValueChanged.AddListener(handler);
		}
	}
}

