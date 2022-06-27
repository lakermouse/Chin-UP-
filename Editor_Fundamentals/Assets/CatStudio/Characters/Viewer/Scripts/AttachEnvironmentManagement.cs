using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



namespace CatStudio.Character
{
	namespace Viewer
	{
		public class AttachEnvironmentManagement : MonoBehaviour
		{


			[System.Serializable]
			public class AttachObjectSceneIdSetting
			{
				
				public string Id;

				public string[] AttachIds;
			}


			[SerializeField]
			AttachObjectSceneIdSetting[] AttachObjectSceneIdSettings;



			[SerializeField]
			AttachPairingManagement AttachObjectManager;




			#region MyRegion

			private void Awake()
			{

			}

			#endregion


			public void Execute(string id)
			{

				var list = AttachObjectSceneIdSettings.ToList();

				if (id != null)
				{
					var setting = list.Find(o => o.Id.CompareTo(id) == 0);
					if (setting != null)
					{
						AttachObjectManager.Execute(setting.AttachIds);
					}
					else
					{
						AttachObjectManager.Execute(new string[] { });
					}

				}
				else
				{

				}

			}

		}

	}

}