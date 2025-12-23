using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Root.Screens.Shop.ManualLayout
{

	public class ManualLayoutElement : AbstractManualLayout
	{
		[SerializeField]
		private bool m_ignoreInLayout;
		[SerializeField]
		private FlexState m_flexState;
		//note: локальные отступы не влияют на размеры Layout Group, вместо этого они влияют только на размер Layout Element
		[SerializeField, InspectorName("Left"), ShowIf("@this.m_flexState == FlexState.Stretched"), FoldoutGroup("Local Paddings")]
		private float m_leftPadding;
		[SerializeField, InspectorName("Right"), ShowIf("@this.m_flexState == FlexState.Stretched"), FoldoutGroup("Local Paddings")]
		private float m_rightPadding;

		public FlexState ElementFlexState => m_flexState;
		
		public bool IgnoreInLayout => m_ignoreInLayout;

		public override void Recalculate()
		{
			RecalculateSubLayouts();
		}

		private void Reset()
		{
			m_rectTransform = GetComponent<RectTransform>();
		}

		public void SetWidth(float width)
		{
			m_rectTransform.sizeDelta = new Vector2(width, m_rectTransform.sizeDelta.y);
		}
		
		public void SetWidth(float width, Alignment localAlignment)
		{
			m_rectTransform.sizeDelta = new Vector2(width, m_rectTransform.sizeDelta.y);

			if (m_leftPadding == 0 && m_rightPadding == 0)
			{
				return;
			}
			
			Vector2 currentPos = m_rectTransform.anchoredPosition;
			Vector2 currentSize = m_rectTransform.sizeDelta;
			Vector2 newPos = currentPos;
			Vector2 newSize = new Vector2(currentSize.x - (m_leftPadding + m_rightPadding), currentSize.y);
			
			switch (localAlignment)
			{
				case Alignment.Start:
					newPos.x += m_leftPadding;
					break;
				case Alignment.Center:
					newPos.x += m_leftPadding * 0.5f;
					newPos.x -= m_rightPadding * 0.5f;
					break;
				case Alignment.End:
					newPos.x -= m_rightPadding;
					break;
				default:
					return;
			}
			
			m_rectTransform.sizeDelta = newSize;
			m_rectTransform.anchoredPosition = newPos;
		}

		public void SetHeight(float height)
		{
			m_rectTransform.sizeDelta = new Vector2(m_rectTransform.sizeDelta.x, height);
		}
		
		public enum FlexState
		{
			Normal = 0,
			Stretched = 1
		}
	}

}