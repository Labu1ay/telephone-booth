using System;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace UI.Root.Screens.Shop.ManualLayout
{

	public class ManualTextFitter : AbstractManualLayout
	{
		[SerializeField]
		private TextMeshProUGUI m_textToFit;
		[SerializeField]
		private float m_minWidth = 5f;
		[SerializeField]
		private float m_minHeight = 5f;
		[SerializeField, Tooltip("Как будет рассчитываться ширина и высота контейнера текста относительно переноса строк.")]
		private WrapMode m_wrapMode;
		[SerializeField, CanBeNull]
		private ExtraWrappingContent[] m_extraWrappingContent;
		[SerializeField]
		private bool m_extraPaddings;
		[SerializeField, ShowIf(nameof(m_extraPaddings))]
		private Vector2 m_paddings;
		[SerializeField]
		private bool m_recalculateSubLayouts;
		
		protected override void Init()
		{
			base.Init();
			m_textToFit = GetComponent<TextMeshProUGUI>();
		}
		
		[Button("Manual Recalculating")]
		public override void Recalculate()
		{
			Vector2 sizeDelta = m_rectTransform.sizeDelta;
			float x = sizeDelta.x;
			float y = sizeDelta.y;
			Vector2 preferredSize = m_textToFit.GetPreferredValues();
			bool isStringEmpty = m_textToFit.text?.Trim() != string.Empty;
			bool isExtraWrapping = m_extraWrappingContent != null && m_extraWrappingContent.Length > 0;

			if (m_recalculateSubLayouts)
			{
				RecalculateSubLayouts();
			}
			
			switch (m_wrapMode)
			{
				case WrapMode.Vertical:
					y = isStringEmpty ? Mathf.Max(preferredSize.y, m_minHeight) : m_minHeight;

					if (isExtraWrapping)
					{
						y += m_extraWrappingContent.Where(extraWrappingItem => extraWrappingItem.RectTransform.gameObject.activeSelf).Sum(extraWrappingItem => 
							(extraWrappingItem.RectTransform.sizeDelta.y + extraWrappingItem.ExtraPadding));
					}

					if (m_extraPaddings)
					{
						y += m_paddings.y;
					}
					break;
				case WrapMode.Horizontal:
					x = isStringEmpty ? Mathf.Max(preferredSize.x, m_minWidth) : m_minWidth;
					
					if (isExtraWrapping)
					{
						x += m_extraWrappingContent.Where(extraWrappingItem => extraWrappingItem.RectTransform.gameObject.activeSelf).Sum(extraWrappingItem => 
							(extraWrappingItem.RectTransform.sizeDelta.x + extraWrappingItem.ExtraPadding));
					}
					
					if (m_extraPaddings)
					{
						x += m_paddings.x;
					}
					break;
				case WrapMode.Both:
					x = isStringEmpty ? Mathf.Max(preferredSize.x, m_minWidth) : m_minWidth;
					y = isStringEmpty ? Mathf.Max(preferredSize.y, m_minHeight) : m_minHeight;
					
					if (isExtraWrapping)
					{
						foreach (ExtraWrappingContent extraWrappingItem in m_extraWrappingContent)
						{
							if (!extraWrappingItem.RectTransform.gameObject.activeSelf)
							{
								continue;
							}
							
							Vector2 delta = extraWrappingItem.RectTransform.sizeDelta;
							x += (delta.x + extraWrappingItem.ExtraPadding);
							y += (delta.y + extraWrappingItem.ExtraPadding);
						}
					}
					
					if (m_extraPaddings)
					{
						y += m_paddings.y;
						x += m_paddings.x;
					}
					break;
				default:
					x = m_minWidth;
					y = m_minHeight;
					break;
			}

			m_rectTransform.sizeDelta = new Vector2(x,y);
		}

		private enum WrapMode
		{
			Vertical,
			Horizontal,
			Both
		}

		[Serializable]
		private class ExtraWrappingContent
		{
			[SerializeField]
			private RectTransform m_rectTransform;
			[SerializeField]
			private float m_extraPadding = 0;

			public RectTransform RectTransform => m_rectTransform;
			public float ExtraPadding => m_extraPadding;
		}
	}
}