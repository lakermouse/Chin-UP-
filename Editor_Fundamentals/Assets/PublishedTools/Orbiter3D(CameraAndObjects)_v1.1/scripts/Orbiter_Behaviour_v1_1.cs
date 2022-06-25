using UnityEngine;

public class Orbiter_Behaviour_v1_1 : MonoBehaviour
{
    //Orbiter v1.1
    [Header("OBJECT TO ORBIT AROUND")]
    public GameObject objectToOrbitAround;
    [Header("CUSTOMIZATION")]
    public bool objectLooksAtCenter = true;
    public float distToOrbitCent = 1;
    public float orbitSpeed = 1;
    [Range(-1, 1)] public float inclination = 0;
    public bool upAndDown;
    private float upAndDownLerpAmount;
    private bool upAndDownGoingUp;
    private float upAndDownInclination;
    public float upAndDownSpeed = 0.4f;


    private GameObject rotatorCubeInst;
    private GameObject inclinationCubeInst;
    


    private void InitData()
    {
        rotatorCubeInst = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rotatorCubeInst.name = name + "_rotatorCube";
        rotatorCubeInst.transform.position = objectToOrbitAround.transform.position;
        rotatorCubeInst.transform.parent = objectToOrbitAround.transform;

        if (rotatorCubeInst.GetComponent<MeshRenderer>() != null)
        {
            Destroy(rotatorCubeInst.GetComponent<MeshRenderer>());
        }

        if (rotatorCubeInst.GetComponent<BoxCollider>() != null)
        {
            Destroy(rotatorCubeInst.GetComponent<BoxCollider>());
        }


        inclinationCubeInst = GameObject.CreatePrimitive(PrimitiveType.Cube);
        inclinationCubeInst.name = name + "_inclinationCube";
        inclinationCubeInst.transform.position = objectToOrbitAround.transform.position;
        inclinationCubeInst.transform.parent = rotatorCubeInst.transform;

        if (inclinationCubeInst.GetComponent<MeshRenderer>() != null)
        {
            Destroy(inclinationCubeInst.GetComponent<MeshRenderer>());
        }

        if (inclinationCubeInst.GetComponent<BoxCollider>() != null)
        {
            Destroy(inclinationCubeInst.GetComponent<BoxCollider>());
        }
    }
    private void OnUpdateRotationManagement()
    {
        rotatorCubeInst.transform.Rotate(0, orbitSpeed * 10 * Time.deltaTime, 0);


        //Inclination
        if (upAndDown)
        {
            if (upAndDownGoingUp)
            {
                upAndDownLerpAmount += Time.deltaTime * upAndDownSpeed;
                if(upAndDownLerpAmount >= 1)
                {
                    upAndDownGoingUp = false;
                }
            }

            else
            {
                upAndDownLerpAmount -= Time.deltaTime * upAndDownSpeed;
                if(upAndDownLerpAmount <= 0)
                {
                    upAndDownGoingUp = true;
                }
            }


            upAndDownInclination = Mathf.Lerp(-inclination, inclination, Mathf.SmoothStep(0,1,upAndDownLerpAmount));


            inclinationCubeInst.transform.eulerAngles = new Vector3
         (upAndDownInclination * 90,
         inclinationCubeInst.transform.eulerAngles.y,
         inclinationCubeInst.transform.eulerAngles.z
         );
        }


        else
        {
            inclinationCubeInst.transform.eulerAngles = new Vector3
           (inclination * 90,
           inclinationCubeInst.transform.eulerAngles.y,
           inclinationCubeInst.transform.eulerAngles.z
           );
        }

        transform.position = objectToOrbitAround.transform.position + inclinationCubeInst.transform.forward * distToOrbitCent;


        //Looking at center
        if (objectLooksAtCenter)
        {
            transform.LookAt(inclinationCubeInst.transform.position);
        }


        else
        {
            //nothing
        }
    }






    private void Start()
    {
        InitData();
    }
    private void Update()
    {
        OnUpdateRotationManagement();
    }
}
