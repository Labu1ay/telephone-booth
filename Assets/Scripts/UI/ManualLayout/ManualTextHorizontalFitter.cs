using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace UI.Root.Screens.Shop.ManualLayout
{

	public class ManualTextHorizontalFitter : AbstractManualLayout
	{
		[SerializeField]
		private TextMeshProUGUI m_textToFit;
		[SerializeField]
		private float m_minWidth = 5f;

		protected override void Init()
		{
			base.Init();
			m_textToFit = GetComponent<TextMeshProUGUI>();
		}

		[Button("Manual Recalculating")]
		public override void Recalculate()
		{
			RecalculateSubLayouts();

			if (m_textToFit.text.Trim() != string.Empty)
			{
				m_rectTransform.sizeDelta = new Vector2(Mathf.Max(m_textToFit.preferredWidth, m_minWidth), m_rectTransform.sizeDelta.y);
			}
			else
			{
				m_rectTransform.sizeDelta = new Vector2(m_minWidth, m_rectTransform.sizeDelta.y);
			}
		}
	}

}