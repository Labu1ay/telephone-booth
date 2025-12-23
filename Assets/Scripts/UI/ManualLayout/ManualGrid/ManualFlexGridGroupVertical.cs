using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Root.Screens.Shop.ManualLayout.ManualGrid
{

	public class ManualFlexGridGroupVertical : AbstractManualLayout
	{
		[SerializeField]
		private RectOffset m_paddings;
		[SerializeField]
		private Vector2 m_spacing;

		private List<RectTransform> m_transformsToLayout = new List<RectTransform>();

		[Button("Recalculate")]
		public override void Recalculate()
		{
			RecalculateSubLayouts();

			float baseHeight = m_rectTransform.sizeDelta.y - m_paddings.bottom;

			m_transformsToLayout.Clear();

			foreach (RectTransform rt in m_rectTransform)
			{
				m_transformsToLayout.Add(rt);
			}

			float columnHeight = 0;
			float columnWidth = 0;
			float totalWidth = 0;

			for(int r = 0; r < m_transformsToLayout.Count; r++)
			{
				if (!m_transformsToLayout[r].gameObject.activeSelf)
				{
					continue;
				}

				ManualLayoutElement element = m_transformsToLayout[r].GetComponent<ManualLayoutElement>();

				if (element != null)
				{
					if (element.IgnoreInLayout)
					{
						continue;
					}

					//todo: реализовать
					throw new NotImplementedException($"{nameof(ManualFlexGridGroupVertical)} doesn't know how to work with non-ignored {nameof(ManualLayoutElement)}");
				}

				m_transformsToLayout[r].anchorMin = Vector2.up;
				m_transformsToLayout[r].anchorMax = Vector2.up;
				m_transformsToLayout[r].pivot = Vector2.up;

				if (columnHeight + m_transformsToLayout[r].sizeDelta.y + m_spacing.y > baseHeight)
				{
					totalWidth += columnWidth + m_spacing.x;
					columnHeight = 0;
					columnWidth = 0;
				}

				m_transformsToLayout[r].anchoredPosition = new Vector2(m_paddings.left + totalWidth,
					(m_paddings.top + columnHeight) * -1);

				columnHeight += m_transformsToLayout[r].sizeDelta.y + m_spacing.y;

				if (m_transformsToLayout[r].sizeDelta.x > columnWidth)
				{
					columnWidth = m_transformsToLayout[r].sizeDelta.x;
				}
			}

			m_rectTransform.sizeDelta = new Vector2(totalWidth + columnWidth + m_paddings.right + m_paddings.left,
				baseHeight + m_paddings.bottom);
		}
	}

}