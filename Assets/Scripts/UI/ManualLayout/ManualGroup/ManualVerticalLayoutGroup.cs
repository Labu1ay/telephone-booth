using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Root.Screens.Shop.ManualLayout.ManualGroup
{

	public class ManualVerticalLayoutGroup : AbstractManualLayoutGroup
	{
		[Button("Manual Recalculating")]
		public override void Recalculate()
		{
			float positionX = 0;
			float positionY = Paddings.top * -1;

			float sizeX = 0;
			float sizeY = Paddings.top * -1;

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

					if (element.ElementFlexState == ManualLayoutElement.FlexState.Stretched)
					{
						element.SetWidth(0f);
					}

					Elements.Add(element);
				}

				Vector2 alignmentVector = new Vector2(0.0f, 1.0f);

				switch (LocalAlignment)
				{
					case Alignment.Start:
						alignmentVector.x = 0.0f;
						positionX = Paddings.left;
						break;
					case Alignment.Center:
						alignmentVector.x = 0.5f;
						positionX = 0;
						break;
					case Alignment.End:
						alignmentVector.x = 1.0f;
						positionX = Paddings.right * -1;
						break;
				}

				rt.anchorMin = alignmentVector;
				rt.anchorMax = alignmentVector;
				rt.pivot = alignmentVector;
				rt.anchoredPosition = new Vector2(positionX, positionY);

				positionY -= rt.sizeDelta.y + Spacing;

				if (sizeX < rt.sizeDelta.x)
				{
					sizeX = rt.sizeDelta.x;
				}

				sizeY = positionY;
			}

			m_rectTransform.sizeDelta = new Vector2(Mathf.Max(m_minSize.x, sizeX + Paddings.right + Paddings.left),
				Mathf.Max(m_minSize.y, sizeY * -1 - Spacing + Paddings.bottom));

			foreach (ManualLayoutElement element in Elements)
			{
				switch (element.ElementFlexState)
				{
					case ManualLayoutElement.FlexState.Normal:
						break;
					case ManualLayoutElement.FlexState.Stretched:
						element.SetWidth(m_rectTransform.sizeDelta.x, LocalAlignment);
						break;
				}
			}

			SetupAlignment();
		}
	}

}