using UnityEngine;
using System.Collections;

namespace CatStudio.Character
{
	namespace Viewer
	{
		public class EasyCameraControl : MonoBehaviour
		{

			[SerializeField]
			Transform ControlTransformH;


			[SerializeField]
			Transform ControlTransformV;


			[SerializeField]
			Transform ControlTranslation;


			[SerializeField]
			float SpeedRate = 1.0f;



			#region	MonoBehaviour

			void Awake()
			{

			}

			// Use this for initialization
			void Start()
			{

			}

			// Update is called once per frame
			void Update()
			{

			}

			void LateUpdate()
			{
				var vValue = Input.GetAxis("Vertical");
				var hValue = Input.GetAxis("Horizontal");

				var isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

				vValue *= SpeedRate;
				hValue *= SpeedRate;

				if(isShift)
				{	// シフト時はズーム操作
                    var next_pos = ControlTranslation.position - ControlTranslation.forward * Time.deltaTime * vValue;
                    // カメラ距離制限
                    var distance = Vector3.Distance(next_pos, Camera.main.transform.forward);
                    if(distance > 0.5f)
                    {
                        ControlTranslation.position = next_pos;
                    }
				}
				else
				{
					ControlTransformV.Rotate(new Vector3(vValue, 0.0f, 0.0f));
				}
				ControlTransformH.Rotate(new Vector3(0.0f, hValue, 0.0f));
			}

			#endregion

		}

	}
}