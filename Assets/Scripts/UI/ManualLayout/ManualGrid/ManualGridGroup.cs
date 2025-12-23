using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Root.Screens.Shop.ManualLayout.ManualGrid
{

	public class ManualGridGroup : AbstractManualLayout
	{
		[SerializeField]
		private RectOffset m_paddings = new RectOffset();
		[SerializeField]
		private Vector2 m_cellSize;
		
		[SerializeField]
		private int m_constraintsCountCount;
		[SerializeField]
		private Vector2 m_spacing;

		[SerializeField]
		private bool m_isHorizontal;

		[Tooltip("Работает только для Horizontal Grids")]
		[SerializeField]
		private bool m_isFlow;

		[SerializeField]
		private bool m_ignoreDisabled;

		private List<RectTransform> m_transformsToLayout;
		
		public int ConstraintsCount
		{
			get => m_constraintsCountCount;
			set => m_constraintsCountCount = value;
		}

		public RectOffset Paddings
		{
			get => m_paddings;
			set => m_paddings = value;
		}

		public Vector2 CellSize
		{
			get => m_cellSize;
			set => m_cellSize = value;
		}
		
		public Vector2 Spacing
		{
			get => m_spacing;
			set => m_spacing = value;
		}

		public bool IsHorizontal
		{
			get => m_isHorizontal;
			set => m_isHorizontal = value;
		}

		public bool IsFlow
		{
			get => m_isFlow;
			set => m_isFlow = value;
		}

		private void Awake()
		{
			m_transformsToLayout = new List<RectTransform>();
		}

		[Button("Manual Recalculating")]
		public override void Recalculate()
		{
			if (m_constraintsCountCount <= 0)
			{
				Debug.LogError($"{nameof(ManualGridGroup)}.{nameof(Recalculate)}::Constraints count field has incorrect value");
				return;
			}

			int iX = 0;
			int iY = 0;

			int elementsCountX = 0;
			int elementsCountY = 0;

			RecalculateSubLayouts();

			if (m_transformsToLayout == null)
			{
				m_transformsToLayout = new List<RectTransform>();
			}
			
			m_transformsToLayout.Clear();

			float maxWidth = m_paddings.right + m_paddings.left + ConstraintsCount * m_cellSize.x +
				m_spacing.x * ConstraintsCount - m_spacing.x;

			foreach (RectTransform rt in m_rectTransform)
			{
				if (m_ignoreDisabled && !rt.gameObject.activeSelf)
				{
					continue;
				}

				m_transformsToLayout.Add(rt);
			}

			int layoutsCount = m_transformsToLayout.Count;
			int layoutsLeft = layoutsCount;
			
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
				
				rt.anchorMin = Vector2.up;
				rt.anchorMax = Vector2.up;
				rt.pivot = Vector2.up;
				rt.sizeDelta = new Vector2(m_cellSize.x, m_cellSize.y);

				if (layoutsLeft <= layoutsCount % m_constraintsCountCount && m_isFlow && layoutsCount > 1)
				{
					rt.anchoredPosition = new Vector2((((maxWidth + (layoutsLeft != 1 ? 
							m_spacing.x : 0)) / 100) * ((100 / (layoutsLeft + 1)) * (iX + 1))) - (m_cellSize.x / 2), 
						(m_paddings.top + m_cellSize.y * iY + iY * m_spacing.y) * -1);
				}
				else
				{
					rt.anchoredPosition = new Vector2(m_paddings.left + m_cellSize.x * iX + iX * m_spacing.x,
						(m_paddings.top + m_cellSize.y * iY + iY * m_spacing.y) * -1);
					layoutsLeft--;
				}
				
				if (iY == 0)
				{
					elementsCountX++;
				}

				if (iX == 0)
				{
					elementsCountY++;
				}
				
				if (iX < m_constraintsCountCount - 1)
				{
					iX++;
				}
				else
				{
					iY++;
					iX = 0;
				}
			}

			float sizeX = m_paddings.right + m_paddings.left + elementsCountX * m_cellSize.x + m_spacing.x * elementsCountX - m_spacing.x;
			float sizeY = m_paddings.bottom + m_paddings.top + elementsCountY * m_cellSize.y + m_spacing.y * elementsCountY - m_spacing.y;
			m_rectTransform.sizeDelta = new Vector2(sizeX, sizeY);
			RecalculateScroll();
		}
	}

}