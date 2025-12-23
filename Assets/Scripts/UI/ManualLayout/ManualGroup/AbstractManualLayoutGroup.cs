using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Root.Screens.Shop.ManualLayout.ManualGroup
{

	public abstract class AbstractManualLayoutGroup : AbstractManualLayout
	{
		[SerializeField]
		protected RectOffset Paddings = new RectOffset();
		[SerializeField]
		protected int Spacing;
		[SerializeField]
		protected Vector2 m_minSize;

		protected List<ManualLayoutElement> Elements;

		[SerializeField]
		protected Alignment LocalAlignment;
		[SerializeField]
		private Alignment m_globalAlignmentHorizontal;
		[SerializeField]
		private Alignment m_globalAlignmentVertical;

		[SerializeField]
		private bool m_resetPositionOnRecalculate;

		[SerializeField]
		private bool m_isCenteredWhileLessThanMinSize; //TODO: Заменить на более удобную систему выбора условий центрирования

		[SerializeField, ShowIf("m_isCenteredWhileLessThanMinSize")]
		private float m_alignAreaMinWidth;
		[SerializeField, ShowIf("m_isCenteredWhileLessThanMinSize")]
		private float m_alignAreaMinHeight;

		public Alignment GroupGlobalAlignmentHorizontal
		{
			get
			{
				return m_globalAlignmentHorizontal;
			}
			set
			{
				m_globalAlignmentHorizontal = value;
			}
		}
		
		public Alignment GroupGlobalAlignmentVertical
		{
			get
			{
				return m_globalAlignmentVertical;
			}
			set
			{
				m_globalAlignmentVertical = value;
			}
		}

		public Alignment GroupLocalAlignment
		{
			get
			{
				return LocalAlignment;
			}
			set
			{
				LocalAlignment = value;
			}
		}
		
		public int GroupSpacing
		{
			get
			{
				return Spacing;
			}
			set
			{
				Spacing = value;
			}
		}

		public RectOffset GroupPaddings
		{
			get
			{
				return Paddings;
			}
			set
			{
				Paddings = GroupPaddings;
			}
		}

		protected void SetupAlignment()
		{
			Vector2 pivot = new Vector2(GetHorizontalAlignmentValue(m_globalAlignmentHorizontal),
				GetVerticalAlignmentValuer(m_globalAlignmentVertical));
			Vector2 anchorMin = new Vector2(GetHorizontalAlignmentValue(m_globalAlignmentHorizontal),
				GetVerticalAlignmentValuer(m_globalAlignmentVertical));
			Vector2 anchorMax = new Vector2(GetHorizontalAlignmentValue(m_globalAlignmentHorizontal),
				GetVerticalAlignmentValuer(m_globalAlignmentVertical));

			if (m_isCenteredWhileLessThanMinSize)
			{
				if (m_rectTransform.sizeDelta.x < m_alignAreaMinWidth)
				{
					pivot.x = GetHorizontalAlignmentValue(Alignment.Center);
					anchorMin.x = GetHorizontalAlignmentValue(Alignment.Center);
					anchorMax.x = GetHorizontalAlignmentValue(Alignment.Center);
				}

				if (m_rectTransform.sizeDelta.y < m_alignAreaMinHeight)
				{
					pivot.y = GetVerticalAlignmentValuer(Alignment.Center);
					anchorMin.y = GetVerticalAlignmentValuer(Alignment.Center);
					anchorMax.y = GetVerticalAlignmentValuer(Alignment.Center);
				}
			}

			m_rectTransform.pivot = pivot;
			m_rectTransform.anchorMin = anchorMin;
			m_rectTransform.anchorMax = anchorMax;

			if (m_resetPositionOnRecalculate)
			{
				m_rectTransform.anchoredPosition = Vector2.zero;
			}
		}

		private float GetHorizontalAlignmentValue(Alignment alignment)
		{
			switch (alignment)
			{
				case Alignment.Start:
					return 0f;
				case Alignment.Center:
					return 0.5f;
				case Alignment.End:
					return 1f;
				default:
					throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null);
			}
		}

		private float GetVerticalAlignmentValuer(Alignment alignment)
		{
			switch (alignment)
			{
				case Alignment.Start:
					return 1f;
				case Alignment.Center:
					return 0.5f;
				case Alignment.End:
					return 0f;
				default:
					throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null);
			}
		}

		public void SetPaddings(RectOffset rectOffset)
		{
			Paddings = rectOffset;
		}

		public void SetSpacing(int spacing)
		{
			Spacing = spacing;
		}

	}

}
