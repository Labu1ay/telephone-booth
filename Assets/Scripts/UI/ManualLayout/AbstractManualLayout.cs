using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.Root.Screens.Shop.ManualLayout
{

	public abstract class AbstractManualLayout : MonoBehaviour
	{
		[Header("Extra Layout Settings"), SerializeField, PropertyOrder(1)]
		private bool m_recalculateOnEnable;

		[Header("Sub Layouts Settings"), SerializeField, PropertyOrder(1)]
		protected bool m_isSubLayoutsStatic = true;
		[SerializeField, PropertyOrder(1), ShowIf("m_isSubLayoutsStatic")]
		protected List<AbstractManualLayout> m_subLayouts = new List<AbstractManualLayout>();

		[Header("Scroll Settings"), SerializeField, PropertyOrder(1)]
		protected bool m_isNonStaticScrollView;
		[SerializeField, PropertyOrder(1), ShowIf("m_isNonStaticScrollView"), CanBeNull]
		protected RectTransform m_viewportRect;
		[SerializeField, PropertyOrder(1), ShowIf("m_isNonStaticScrollView"), CanBeNull]
		protected RectTransform m_scrollRect;
		[SerializeField, PropertyOrder(1), ShowIf("m_isNonStaticScrollView"), CanBeNull]
		protected RectTransform m_manualContentRect;
		[SerializeField, PropertyOrder(1), ShowIf("m_isNonStaticScrollView")]
		protected float m_maxWidth;
		[SerializeField, PropertyOrder(1), ShowIf("m_isNonStaticScrollView")]
		protected float m_maxHeight;

		[SerializeField]
		protected RectTransform m_rectTransform;

		private bool m_needToRecalculate;
		private bool m_wasMarked;
		
		public enum Alignment
		{
			Start = 0,
			Center = 1,
			End = 2
		}

		public abstract void Recalculate();

		/// <summary>
		/// Метод полезен при удалении элементов лэйаута, т.к. Unity удаляет объекты только в следующем кадре.
		/// При использовании такого способа могут быть визуальные дефекты при респавне элементов.
		/// Чтобы визуальных дефектов не было - нужно перед удалением элементов делать их неактивными и вызывать обычный Recalculate().
		/// </summary>
		public void MarkToRecalculateAtTheEndOfFrame()
		{
			if (!m_needToRecalculate)
			{
				RecalculateAfterEndOfFrameAsync().Forget();
				m_needToRecalculate = true;
			}
		}

		public void MarkOnceThenRecalculate()
		{
			if (m_wasMarked)
			{
				Recalculate();
			}
			else
			{
				MarkToRecalculateAtTheEndOfFrame();
				m_wasMarked = true;
			}
		}

		protected void RecalculateSubLayouts()
		{
			if (!m_isSubLayoutsStatic)
			{
				foreach (Transform t in transform)
				{
					AbstractManualLayout subLayout = t.GetComponent<AbstractManualLayout>();

					if (subLayout != null)
					{
						subLayout.Recalculate();
					}
				}
			}
			else
			{
				if (m_subLayouts.Count == 0)
				{
					return;
				}

				foreach (AbstractManualLayout layout in m_subLayouts)
				{
					layout.Recalculate();
				}
			}
		}

		protected void RecalculateScroll()
		{
			if (!m_isNonStaticScrollView || m_manualContentRect == null)
			{
				return;
			}

			Vector2 newSize = m_manualContentRect.sizeDelta;

			if (m_maxWidth != 0)
			{
				newSize.x = Mathf.Clamp(newSize.x,newSize.x, m_maxWidth);
			}

			if (m_maxHeight != 0)
			{
				newSize.y = Mathf.Clamp(newSize.y, newSize.y, m_maxHeight);
			}
			
			if (m_viewportRect != null)
			{
				m_viewportRect.sizeDelta = newSize;
			}

			if (m_scrollRect != null)
			{
				m_scrollRect.sizeDelta = newSize;
			}
		}

		protected virtual void Init()
		{
			m_rectTransform = GetComponent<RectTransform>();
			m_subLayouts = new List<AbstractManualLayout>();

			foreach (RectTransform tr in m_rectTransform)
			{
				AbstractManualLayout layout = tr.GetComponent<AbstractManualLayout>();

				if (layout != null)
				{
					m_subLayouts.Add(layout);
				}
			}
		}

		private void Reset()
		{
			Init();
		}

		private void OnEnable()
		{
			if (m_recalculateOnEnable)
			{
				Recalculate();
			}
		}

		private async UniTaskVoid RecalculateAfterEndOfFrameAsync()
		{
			await UniTask.Yield(PlayerLoopTiming.LastTimeUpdate);

			if (IsDestroyed())
			{
				return;
			}
			Recalculate();
			m_needToRecalculate = false;
		}

		private bool IsDestroyed()
		{
			return GetInstanceID() == 0 || m_rectTransform == null;
		}
	}

}