using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Root.Screens.Shop.ManualLayout.ManualGroup
{

	public class ManualExpandingGroup : AbstractManualLayout
	{
		[SerializeField]
		private Orientation m_orientation;

		private List<RectTransform> m_transformsToRecalculate;

		[Button("Recalculate")]
		public override void Recalculate()
		{
			m_transformsToRecalculate = new List<RectTransform>();
			RecalculateSubLayouts();
			m_transformsToRecalculate.Clear();

			foreach (RectTransform rt in m_rectTransform)
			{
				if (!rt.gameObject.activeSelf)
				{
					continue;
				}

				ManualLayoutElement element = rt.GetComponent<ManualLayoutElement>();

				if (element != null)
				{
					if (element.IgnoreInLayout)
					{
						continue;
					}
				}

				m_transformsToRecalculate.Add(rt);
			}

			float averagePercent = (float) 1 / (m_transformsToRecalculate.Count + 1);
			float summingModifier = averagePercent;

			foreach (RectTransform rectTransform in m_transformsToRecalculate)
			{
				Vector2 newAnchor = m_orientation switch
				{
					Orientation.Horizontal => new Vector2(averagePercent, 0.5f),
					Orientation.Vertical => new Vector2(0.5f, averagePercent),
					_ => new Vector2(0.5f, 0.5f)
				};

				rectTransform.anchorMin = newAnchor;
				rectTransform.anchorMax = newAnchor;
				rectTransform.anchoredPosition = new Vector2(0f, 0f);
				averagePercent += summingModifier;
			}
		}

		private enum Orientation
		{
			Horizontal = 0,
			Vertical = 1
		}
	}

}