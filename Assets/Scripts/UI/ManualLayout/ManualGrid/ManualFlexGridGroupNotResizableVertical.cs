using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Root.Screens.Shop.ManualLayout.ManualGrid
{
	/// <summary>
	/// Грид-группа, выстраивающая элементы любой ширины в сетку, самостоятельно не меняет ширину своего rect transform,
	/// опирается на свою текущую ширину и ширину элементов для определения количества "строк"
	/// </summary>
	public class ManualFlexGridGroupNotResizableVertical : AbstractManualLayout
	{
		[SerializeField]
		private RectOffset m_paddings;
		[SerializeField]
		private float m_cellHeight;
		[SerializeField]
		private Vector2 m_spacing;
		[SerializeField, Tooltip("Если true - контейнер расширится под самую широкую строку (в случае если есть элемент, который в одиночку не помещается в одну строку контейнера по ширине")]
		private bool m_allowWidthResize;
		
		private List<RectTransform> m_transformsToLayout;
		
		[Button("Manual Recalculating")]
		public override void Recalculate()
		{
			m_transformsToLayout ??= new List<RectTransform>();
			
			RecalculateSubLayouts();
			
			m_transformsToLayout.Clear();
			
			foreach (RectTransform rt in m_rectTransform)
			{
				if (!rt.gameObject.activeSelf)
				{
					continue;
				}

				m_transformsToLayout.Add(rt);
			}

			float currentX = m_paddings.left;
			float currentY = -m_paddings.top;
			float maxWidth = m_rectTransform.rect.width - m_paddings.left - m_paddings.right;
			float lineHeight = 0f;

			int calculatedTransformsCount = 0;

			foreach (RectTransform rt in m_transformsToLayout)
			{
				if (!rt.gameObject.activeSelf)
				{
					continue;
				}
				
				ManualLayoutElement element = rt.GetComponent<ManualLayoutElement>();

				if (element != null && element.IgnoreInLayout)
				{
					continue;
				}

				calculatedTransformsCount++;

				rt.sizeDelta = new Vector2(rt.sizeDelta.x, m_cellHeight);
				
				if (currentX + rt.rect.width > maxWidth)
				{
					currentX = m_paddings.left;               
					currentY -= rt.rect.height + m_spacing.y;
				}

				if (m_allowWidthResize && currentX + rt.rect.width > maxWidth)
				{
					m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentX + rt.rect.width);
				}

				lineHeight = Mathf.Max(lineHeight, rt.rect.height);

				rt.anchoredPosition = new Vector2(currentX, currentY);

				currentX += rt.rect.width + m_spacing.x;
			}

			float height = calculatedTransformsCount == 0 ? 0f : -currentY + lineHeight + m_paddings.bottom;
			m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}
	}

}