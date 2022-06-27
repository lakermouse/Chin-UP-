using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace CatStudio.Character
{
	namespace Viewer
	{
		public class MotionViewerCell : MonoBehaviour
		{


			public delegate void DidClickedEvent(MotionViewerCell sender, MotionViewer.MotionInfo info);

			public DidClickedEvent DidClicked;


			[SerializeField]
			Text RefName;


			[SerializeField]
			Button RefButton;


			MotionViewer.MotionInfo RefMotionInfo;


			#region	MonoBehaviour

			void Awake()
			{

			}

			// Use this for initialization
			void Start()
			{

				RefButton.onClick.AddListener(() =>
				{

					DidClicked(this, this.RefMotionInfo);
				});

			}


			#endregion


			public void SetMotionInfo(MotionViewer.MotionInfo info)
			{
				RefMotionInfo = info;

				RefName.text = info.Title;
			}
		}

	}
}