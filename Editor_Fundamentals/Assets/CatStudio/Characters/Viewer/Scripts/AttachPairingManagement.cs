using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CatStudio.Character
{
	namespace Viewer
	{
		public class AttachPairingManagement : MonoBehaviour
		{


			[System.Serializable]
			class AttachInformation
			{
				
				public string Id;


				[SerializeField] Transform AttachDestination;


				[SerializeField] GameObject AttachObject;


				bool IsValid()
				{
					if (this.AttachObject==null)
					{
						return false;
					}
					if (this.AttachDestination == null)
					{
						return false;
					}
					return true;
				}

				public void Attach()
				{
					if (IsValid())
					{
						AttachObject.SetActive(true);
						AttachObject.transform.SetParent(this.AttachDestination,false);
						AttachObject.transform.transform.localPosition = Vector3.zero;
						AttachObject.transform.transform.localRotation = Quaternion.identity;
					}
				}

				public void Detach()
				{
					if (IsValid())
					{
						
						AttachObject.transform.SetParent(null);
						AttachObject.SetActive(false);
					}
				}
			}


			[SerializeField]
			AttachInformation[] AttachInformations;


		
			public void Execute(string[] attachIds)
			{

				foreach (var info in AttachInformations)
				{
					info.Detach();
				}

				foreach (var info in AttachInformations)
				{					
					bool isFound = false;
					foreach (var id in attachIds)
					{
						if (info.Id.CompareTo(id)==0)
						{
							isFound = true;
							break;
						}
					}

					if (isFound)
					{
						info.Attach();
					}
				}
			}



		}

	}
}
