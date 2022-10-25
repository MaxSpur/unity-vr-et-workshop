using System;
using System.Collections;
using System.IO;
using UnityEngine;
using ViveSR.anipal.Eye;
using Random = UnityEngine.Random;

public class CubeSequenceSampling : MonoBehaviour
{
    private EyeTrackingSmplr eyeTrackingSmplr;
    
    public static string dirpathname = "subjData/";
    public static string dirpath;

    private Transform CubeContainerTrans;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        dirpath = Directory.GetParent(Application.dataPath).ToString() + Path.DirectorySeparatorChar + dirpathname;
        Directory.CreateDirectory($"{dirpath}");
        
        eyeTrackingSmplr = EyeTrackingSmplr.instance;

        yield return new WaitUntil(() => eyeTrackingSmplr.isReady);
        
        // Calibrate eye tracker once at the start - Comment out after first time
//        bool calibrationSuccess = false;
//        while (!calibrationSuccess)
//        {
//            int calibReturnCode = SRanipal_Eye_API.LaunchEyeCalibration(IntPtr.Zero);
//            print($"calibReturnCode: {calibReturnCode} == {(int) ViveSR.Error.WORK}");
//            calibrationSuccess = calibReturnCode == (int) ViveSR.Error.WORK;
//        }

        // Create floating cubes in a square formation around the room's origin
        //    Probably one of my most opaque piece of code ;) good luck
        Vector2[] moveVec = new[]
        {
            new Vector2(0,-1),
            new Vector2(1,0),
            new Vector2(0,1),
            new Vector2(-1,0),
        };
        
        CubeContainerTrans = new GameObject("CubeContainer").transform;
        
        Vector3 startPos = new Vector3(1.8f, 1.6f, 1.8f);
        
        for (int iBorder = 0; iBorder < 4; iBorder++)
        {
            float tmpVal = startPos.x; 
            startPos.x = -startPos.z;
            startPos.z = tmpVal;
            
            for (int iCube = 0; iCube < 4; iCube++)
            {
                Vector3 position = startPos;
                position.x += moveVec[iBorder].x * (3.6f/4f * iCube);
                position.z += moveVec[iBorder].y * (3.6f/4f * iCube);
                
                GameObject cube = CreateInteractiveCube(position, Random.rotation, Random.ColorHSV());
                cube.SetActive(false);
            }
        }
        
        // Show cubes one by one in a random order
        int itrial = 0;
        while (CubeContainerTrans.childCount > 0)
        {
            int iCube = (int)(Random.value * CubeContainerTrans.childCount);

            GameObject cubeGO = CubeContainerTrans.GetChild(iCube).gameObject;
            // Show cube
            cubeGO.SetActive(true);
            
            eyeTrackingSmplr.writer = new StreamWriter($"{dirpath}/Cube_{itrial++}.csv");
            eyeTrackingSmplr.isSampling = true;
            
            // Wait for cube to be destroyed
            yield return new WaitUntil(() => cubeGO == null);
            
            eyeTrackingSmplr.isSampling = false;
            eyeTrackingSmplr.writer.Close();
        }
    }

    private GameObject CreateInteractiveCube(Vector3 position, Quaternion rotation, Color col1)
    {
        GameObject cubeGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeGo.name = "CollidableCube";
        Transform cubeTrans = cubeGo.transform;

        cubeTrans.position = position;
        cubeTrans.rotation = rotation;
        cubeTrans.localScale *= .15f;
        
        // Assign this cube as a child of "CubeContainer" GameObject
        cubeTrans.SetParent(CubeContainerTrans);
        
        cubeGo.AddComponent<DestroyOnCollide>();
        cubeGo.GetComponent<MeshRenderer>().material.color = col1;
        
        return cubeGo;
    }
}
