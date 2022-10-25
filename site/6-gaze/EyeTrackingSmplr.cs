using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using ViveSR.anipal.Eye;
using System.Threading;

public class EyeTrackingSmplr : MonoBehaviour {
    public static EyeTrackingSmplr instance { get; private set; }
    public bool isReady { get; private set; }

    public static GazePoint gazePoint = new GazePoint();

    public Transform mainCamTrans;

    public bool isSampling;
    public StreamWriter writer;

    private void Awake()
    {
        instance = this;
    }

    private IEnumerator Start() {
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        
        gazePoint = new GazePoint();

        isReady = false;

        while (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
            SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) {
            yield return null;
        }

        SRanipal_Eye.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));

        isReady = true;
        Debug.Log("Eye tracking ready.");
    }
    
    internal class MonoPInvokeCallbackAttribute : System.Attribute
    {
        public MonoPInvokeCallbackAttribute() { }
    }
    
    [MonoPInvokeCallback]
    private static void EyeCallback(ref EyeData eye_data) {
        gazePoint = new GazePoint(eye_data);

        if (instance.isSampling && instance.writer != null && instance.writer.BaseStream.CanWrite)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            
            Vector3 meanBasePoint = gazePoint.CombGaze.gaze_origin_mm * 0.001f;
            Vector3 leftBasePoint = gazePoint.LeftGaze.gaze_origin_mm * 0.001f;
            Vector3 rightBasePoint = gazePoint.RightGaze.gaze_origin_mm * 0.001f;
    
            float letPupilDiam = gazePoint.LeftGaze.pupil_diameter_mm; 
            float rightPupilDiam = gazePoint.RightGaze.pupil_diameter_mm; 
    
            Vector3 meanGazeDirection = gazePoint.CombGaze.gaze_direction_normalized;
            Vector3 leftGazeDirection = gazePoint.LeftGaze.gaze_direction_normalized;
            Vector3 rightGazeDirection = gazePoint.RightGaze.gaze_direction_normalized;
    
            meanGazeDirection.x *= -1;
            leftGazeDirection.x *= -1;
            rightGazeDirection.x *= -1;
    
            bool valC = gazePoint.valid(Lateralisation.comb);
            bool valL = gazePoint.valid(Lateralisation.left);
            bool valR = gazePoint.valid(Lateralisation.right);
    
            instance.writer.WriteLine(
                $"{gazePoint.data.timestamp},{instance.UnityTimeStamp}," +
                $"{instance.cameraPosition.x},{instance.cameraPosition.y},{instance.cameraPosition.z}," +
                $"{instance.cameraQuaternion.x},{instance.cameraQuaternion.y}," +
                $"{instance.cameraQuaternion.z},{instance.cameraQuaternion.w}," +
                $"{meanBasePoint.x},{meanBasePoint.y},{meanBasePoint.z}," +
                $"{meanGazeDirection.x},{meanGazeDirection.y},{meanGazeDirection.z}," +
                $"{leftBasePoint.x},{leftBasePoint.y},{leftBasePoint.z}," +
                $"{leftGazeDirection.x},{leftGazeDirection.y},{leftGazeDirection.z}," +
                $"{rightBasePoint.x},{rightBasePoint.y},{rightBasePoint.z}," +
                $"{rightGazeDirection.x},{rightGazeDirection.y},{rightGazeDirection.z}," +
                $"{letPupilDiam},{rightPupilDiam}," +
                $"{valC},{valL},{valR}", false
             );
        }
    }

    private long preocu;
    private void Update() {
        RetrieveCameraData();
    }
    
    private void LateUpdate() {
        RetrieveCameraData();
    }
    
    private long UnityTimeStamp;
    private Vector3 cameraPosition;
    private Quaternion cameraQuaternion;

    public void RetrieveCameraData() {
        cameraPosition = mainCamTrans.position;
        cameraQuaternion = mainCamTrans.rotation;

        UnityTimeStamp = GetTimestamp();

        SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out gazePoint.LeftWorldRay, gazePoint.data);
        gazePoint.LeftWorldRay = new Ray(cameraPosition, mainCamTrans.transform.TransformDirection(gazePoint.LeftWorldRay.direction));
        
        SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out gazePoint.RightWorldRay, gazePoint.data);
        gazePoint.RightWorldRay = new Ray(cameraPosition, mainCamTrans.transform.TransformDirection(gazePoint.RightWorldRay.direction));
        
        SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out gazePoint.CombWorldRay, gazePoint.data);
        gazePoint.CombWorldRay = new Ray(cameraPosition, mainCamTrans.transform.TransformDirection(gazePoint.CombWorldRay.direction));

        gazePoint.LeftCollide = Physics.Raycast(gazePoint.LeftWorldRay, out RaycastHit hitL) ? hitL.transform : null;
        gazePoint.RightCollide = Physics.Raycast(gazePoint.RightWorldRay, out RaycastHit hitR) ? hitR.transform : null;
        gazePoint.CombinedCollide = Physics.Raycast(gazePoint.CombWorldRay, out RaycastHit hitC) ? hitC.transform : null;

        if (gazePoint.LeftCollide != null && gazePoint.LeftCollide.name == "CollidableCube")
            gazePoint.LeftCollide.SendMessage("OnTriggerEnter", new Collider(), SendMessageOptions.DontRequireReceiver);
        if (gazePoint.RightCollide != null && gazePoint.RightCollide.name == "CollidableCube")
            gazePoint.RightCollide.SendMessage("OnTriggerEnter", new Collider(), SendMessageOptions.DontRequireReceiver);
        if (gazePoint.CombinedCollide != null && gazePoint.CombinedCollide.name == "CollidableCube")
            gazePoint.CombinedCollide.SendMessage("OnTriggerEnter", new Collider(), SendMessageOptions.DontRequireReceiver);
    }

    public class GazePoint {
        public GazePoint() // Empty ctor
        {
            data = new EyeData();
        }

        public GazePoint(EyeData gaze) {
            LeftGaze = gaze.verbose_data.left;
            RightGaze = gaze.verbose_data.right;
            CombGaze = gaze.verbose_data.combined.eye_data;
            
            data = gaze;

            LeftCollide = null;
            RightCollide = null;
            CombinedCollide = null;
        }

        public EyeData data; // readonly
        public readonly SingleEyeData LeftGaze;
        public readonly SingleEyeData RightGaze;
        public readonly SingleEyeData CombGaze;

        public Ray LeftWorldRay;
        public Ray RightWorldRay;
        public Ray CombWorldRay;

        public Transform LeftCollide;
        public Transform RightCollide;
        public Transform CombinedCollide;

        public bool valid(Lateralisation later) {
            switch (later) {
                case Lateralisation.left:
                    return LeftGaze.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_ORIGIN_VALIDITY);
                case Lateralisation.right:
                    return RightGaze.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_ORIGIN_VALIDITY);
                default:
                    return CombGaze.GetValidity(SingleEyeDataValidity.SINGLE_EYE_DATA_GAZE_ORIGIN_VALIDITY);
            }
        }
    }

    public static long GetTimestamp()
    {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
}
