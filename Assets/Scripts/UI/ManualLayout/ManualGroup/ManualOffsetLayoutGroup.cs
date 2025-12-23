using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Root.Screens.Shop.ManualLayout.ManualGroup
{

	public class ManualOffsetLayoutGroup : AbstractManualLayout
	{
		[SerializeField]
		private Vector2 m_cellSize;
		[SerializeField]
		private Vector2 m_cellPositionOffset;
		[SerializeField]
		private Vector2 m_cellSizeOffset;
		[SerializeField]
		private RectOffset m_paddings = new RectOffset();
		[SerializeField]
		private Alignment m_localAlignmentHorizontal;
		[SerializeField]
		private Alignment m_localAlignmentVertical;
		
		public Vector2 CellSize
		{
			get => m_cellSize;
			set => m_cellSize = value;
		}
		
		public Vector2 CellPositionOffset
		{
			get => m_cellPositionOffset;
			set => m_cellPositionOffset = value;
		}
		
		public Vector2 CellSizeOffset
		{
			get => m_cellSizeOffset;
			set => m_cellSizeOffset = value;
		}
		
		public RectOffset Paddings
		{
			get => m_paddings;
			set => m_paddings = value;
		}
		
		public Alignment LocalAlignmentHorizontal
		{
			get => m_localAlignmentHorizontal;
			set => m_localAlignmentHorizontal = value;
		}

		public Alignment LocalAlignmentVertical
		{
			get => m_localAlignmentVertical;
			set => m_localAlignmentVertical = value;
		}
		
		[Button("Manual Recalculating")]
		public override void Recalculate()
		{
			float positionX = m_paddings.left;
			float positionY = m_paddings.bottom;

			float sizeX = m_paddings.left;
			float sizeY = m_paddings.bottom;

			Vector2 cellCurrentSize = m_cellSize;
			Vector2 alignmentVector = new Vector2(0.0f, 0.0f);

			alignmentVector.x = m_localAlignmentHorizontal switch
			{
				Alignment.Start => 0.0f,
				Alignment.Center => 0.5f,
				Alignment.End => 1.0f,
				_ => 0.0f
			};

			alignmentVector.y = m_localAlignmentVertical switch
			{
				Alignment.Start => 0.0f,
				Alignment.Center => 0.5f,
				Alignment.End => 1.0f,
				_ => 0.0f
			};
			
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
				}
				
				rt.anchorMin = alignmentVector;
				rt.anchorMax = alignmentVector;
				rt.pivot = alignmentVector;
				rt.sizeDelta = cellCurrentSize;
				rt.anchoredPosition = new Vector2(positionX, positionY);

				positionX += m_cellPositionOffset.x;
				positionY += m_cellPositionOffset.y;

				sizeX = Mathf.Abs(positionX) + cellCurrentSize.x;
				sizeY = Mathf.Abs(positionY) + cellCurrentSize.y;

				cellCurrentSize += m_cellSizeOffset;
			}
			
			m_rectTransform.sizeDelta = new Vector2(sizeX + Paddings.right - Mathf.Abs(m_cellPositionOffset.x),
				sizeY + Paddings.top - Mathf.Abs(m_cellPositionOffset.y));
		}
	}
}