using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Root.Screens.Shop.ManualLayout.ManualGroup
{

	public class ManualHorizontalLayoutGroup : AbstractManualLayoutGroup
	{
		[Button("Manual Recalculating")]
		public override void Recalculate()
		{
			float positionX = Paddings.left;
			float positionY = 0;

			float sizeX = Paddings.left;
			float sizeY = 0;

			Elements = new List<ManualLayoutElement>();

			RecalculateSubLayouts();

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

					Elements.Add(element);
				}

				Vector2 alignmentVector = new Vector2(0.0f, 0.0f);

				switch (LocalAlignment)
				{
					case Alignment.Start:
						alignmentVector.y = 0.0f;
						positionY = Paddings.top;
						break;
					case Alignment.Center:
						alignmentVector.y = 0.5f;
						positionY = 0;
						break;
					case Alignment.End:
						alignmentVector.y = 1.0f;
						positionY = Paddings.bottom;
						break;
				}

				rt.anchorMin = alignmentVector;
				rt.anchorMax = alignmentVector;
				rt.pivot = alignmentVector;
				rt.anchoredPosition = new Vector2(positionX, positionY);

				positionX += rt.sizeDelta.x + Spacing;

				if (sizeY < rt.sizeDelta.y)
				{
					sizeY = rt.sizeDelta.y;
				}

				sizeX = positionX;
			}

			m_rectTransform.sizeDelta = new Vector2(Mathf.Max(m_minSize.x, sizeX + Paddings.right - Spacing),
				Mathf.Max(m_minSize.y, sizeY + Paddings.bottom + Paddings.top));

			foreach (ManualLayoutElement element in Elements)
			{
				switch (element.ElementFlexState)
				{
					case ManualLayoutElement.FlexState.Normal:
						break;
					case ManualLayoutElement.FlexState.Stretched:
						element.SetHeight(m_rectTransform.sizeDelta.y);
						break;
				}
			}

			SetupAlignment();
		}
	}
}