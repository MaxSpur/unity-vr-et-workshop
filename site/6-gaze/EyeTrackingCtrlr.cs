using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;
using ViveSR.anipal.Eye;
using System.Threading;

public enum Lateralisation
{
    left, right, comb
}

public class EyeTrackingCtrlr : MonoBehaviour {
    public static EyeTrackingCtrlr instance { get; private set; }
    public bool isReady { get; private set; }

    public static GazePoint gazePoint = new GazePoint();

    public Transform mainCamTrans;

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
    
    private static void EyeCallback(ref EyeData eye_data) {
        gazePoint = new GazePoint(eye_data);
    }

    private long preocu;
    private void Update() {
        RetrieveCameraData();
    }
    
    internal class MonoPInvokeCallbackAttribute : System.Attribute
    {
        public MonoPInvokeCallbackAttribute() { }
    }
    
    [NonSerialized]
    public long UnityTimeStamp;

    [MonoPInvokeCallback]
    public void RetrieveCameraData() {
        Vector3 cameraPosition = mainCamTrans.position;

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

        UnityTimeStamp = GetTimestamp();
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
