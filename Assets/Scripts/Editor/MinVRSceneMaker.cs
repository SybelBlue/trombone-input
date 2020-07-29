using System.Collections.Generic;
using MinVR;
using UnityEditor;
using UnityEngine;
using Utils;


namespace Editor
{ 
    using static Utils;
    public static class MinVRSceneMaker
    {
        private const string BUTTON_ANALOG_IP = "127.0.0.1:3883";
        private const string TRACKER_IP = "127.0.0.1:3884";
        private const string VR_DEVICE_IP = "127.0.0.1";

        private const string HEAD_TRACKING_EVENT = "Head_Move";

        [MenuItem("MinVR/Populate Scene", priority = -100)]
        public static void PopulateScene()
        {
            PrepareCamera();
            PrepareAllVRDevices();
            PrepareAllVRPNInputs();
            GenerateVRDebug();
        }

        [MenuItem("MinVR/Generate VRDebug Object")]
        public static void GenerateVRDebug()
        {
            var obj = GetGameObject("VRDebug");

            var debugDrawAllTrackers = ForceComponent<DebugDrawAllTrackers>(ref obj);
            debugDrawAllTrackers.trackersToIgnore = new List<string>() { "Head" };
            Debug.LogWarning("Debug Draw All Trackers will not render without a cursor prefab.");

            var debugDrawEyes = ForceComponent<DebugDrawEyes>(ref obj);
            debugDrawEyes.sphereScale = 0.05f;
            debugDrawEyes.enabled = false;

            var fps = ForceComponent<FPS>(ref obj);
            fps.textColor = Color.white;
            fps.labelPosition = new Vector3(0, 2, 4);

            var fpsLabel = GetGameObject("Label");
            ForceComponent<TextMesh>(ref fpsLabel);

            fpsLabel.transform.parent = fps.transform;
        }

        [MenuItem("MinVR/Prepare Camera")]
        public static void PrepareCamera()
        {
            var cam = Camera.main ? Camera.main.gameObject : GetGameObject("Main Camera", tag: "MainCamera");



            var camera = ForceComponent<Camera>(ref cam);
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.cullingMask = ~0; // everything bit-mask

            camera.usePhysicalProperties = false;
            camera.fieldOfView = 27;

            camera.depth = -1;

            camera.renderingPath = RenderingPath.UsePlayerSettings;
            camera.targetTexture = null;

            camera.useOcclusionCulling = true;

            camera.allowHDR = true;
            camera.allowMSAA = true;
            camera.allowDynamicResolution = false;

            camera.targetDisplay = 0;
            camera.stereoTargetEye = StereoTargetEyeMask.Both;



            var vrMain = ForceComponent<VRMain>(ref cam);
            vrMain.editorCmdLineArgs = "-vrdevice Desktop";
            vrMain.debugLogEvents = false;



            var scrnshtUtls = ForceComponent<ScreenshotUtils>(ref cam);
            scrnshtUtls.renderingCamera = camera;
            scrnshtUtls.resWidth = 3840;
            scrnshtUtls.resHeight = 2160;



            ForceComponent<AudioListener>(ref cam);
        }

        [MenuItem("MinVR/Prepare VRPN Inputs.../All", priority = -10)]
        public static void PrepareAllVRPNInputs()
        {
            PrepareVRPNInputsParent();
            PrepareHeadTrackingInput();
            PrepareBlueStylusInput();
        }

        [MenuItem("MinVR/Prepare VRPN Inputs.../VRPN Inputs (Parent)")]
        public static GameObject PrepareVRPNInputsParent()
        {
            var vrpnObj = GetGameObject("VRPNInputs");

            ForceComponent<VRPNInput>(ref vrpnObj);

            return vrpnObj;
        }

        [MenuItem("MinVR/Prepare VRPN Inputs.../Head Tracking Input")]
        public static void PrepareHeadTrackingInput()
        {
            var obj = GetGameObject("Head Tracking Input");

            obj.transform.parent = PrepareVRPNInputsParent().transform;

            var tracker = ForceComponent<VRPNTracker>(ref obj);

            tracker.device = "head";

            tracker.server = TRACKER_IP;
            tracker.sensor = 0;
            tracker.eventName = HEAD_TRACKING_EVENT;
            tracker.applyUpdatesToGameObject = false;

            tracker.trackingMode = VRPNTracker.TrackingMode.SIX_DOF;
            tracker.reportType = VRPNTracker.ReportType.MOST_RECENT;
            tracker.threadingMode = VRPNTracker.ThreadingMode.MULTI_THREADED;

            tracker.positionAdjustment = Vector3.zero;
            tracker.rotationAdjustment = Vector3.zero;
            tracker.scaleAdjustment = Vector3.one * 3.28084f;
        }

        [MenuItem("MinVR/Prepare VRPN Inputs.../Blue Stylus Input")]
        public static void PrepareBlueStylusInput()
        {
            var obj = GetGameObject("Blue Stylus Input");

            obj.transform.parent = PrepareVRPNInputsParent().transform;

            var analog = ForceComponent<VRPNAnalog>(ref obj);

            analog.device = "OVRStylusBlue";

            analog.server = BUTTON_ANALOG_IP;
            analog.channel = 0;
            analog.eventName = "BlueStylusAnalog";
            analog.applyUpdatesToGameObject = false;
            analog.movementType = VRPNAnalog.MovementType.NONE;
            analog.axis = VRPNAnalog.Axis.X;
            analog.speed = 1;



            var backButton = ForceComponent<VRPNButton>(ref obj, nth: 0);

            backButton.device = "OVRStylusBlue";

            backButton.server = BUTTON_ANALOG_IP;
            backButton.button = 0;
            backButton.eventName = "BlueStylusBackBtn";
            backButton.applyUpdatesToGameObject = false;
            backButton.movementType = VRPNButton.MovementType.TRANSLATE;
            backButton.axis = VRPNButton.Axis.X;
            backButton.speed = 1;



            var frontButton = ForceComponent<VRPNButton>(ref obj, nth: 1);

            frontButton.device = "OVRStylusBlue";

            frontButton.server = BUTTON_ANALOG_IP;
            frontButton.button = 1;
            frontButton.eventName = "BlueStylusFrontBtn";
            frontButton.applyUpdatesToGameObject = false;
            frontButton.movementType = VRPNButton.MovementType.TRANSLATE;
            frontButton.axis = VRPNButton.Axis.X;
            frontButton.speed = 1;


            var tracker = ForceComponent<VRPNTracker>(ref obj);

            tracker.device = "stylusBlue"; // taken from sample in _EXT, but doesn't match others?!

            tracker.server = TRACKER_IP;
            tracker.sensor = 0;
            tracker.eventName = "BlueStylus_Move";
            tracker.applyUpdatesToGameObject = false;

            tracker.trackingMode = VRPNTracker.TrackingMode.SIX_DOF;
            tracker.reportType = VRPNTracker.ReportType.MOST_RECENT;
            tracker.threadingMode = VRPNTracker.ThreadingMode.MULTI_THREADED;

            tracker.positionAdjustment = Vector3.zero;
            tracker.rotationAdjustment = Vector3.zero;
            tracker.scaleAdjustment = Vector3.one * 3.28084f;
        }

        [MenuItem("MinVR/Prepare VR Devices.../All", priority = -10)]
        public static void PrepareAllVRDevices()
        {
            PrepareVRDevicesParent();

            PrepareDesktopVRDevice();
            
            PrepareCaveFrontTopWall();
            PrepareCaveFrontBottomWall();
            PrepareCaveLeftTopWall();
            PrepareCaveLeftBottomWall();
            PrepareCaveRightTopWall();
            PrepareCaveRightBottomWall();
            PrepareCaveTopFloor();
            PrepareCaveBottomFloor();
        }


        [MenuItem("MinVR/Prepare VR Devices.../VRDevices (Parent)")]
        public static GameObject PrepareVRDevicesParent()
            => GetGameObject("VRDevices");

        [MenuItem("MinVR/Prepare VR Devices.../Front Top Cave Wall")]
        public static void PrepareCaveFrontTopWall()
        {
            var corners = new TrackedProjectionScreen.ScreenCorners();
            corners.topLeft = new Vector3(-4, 7.708333f, 4);
            corners.topRight = new Vector3(3.91667f, 7.708333f, 4);
            corners.bottomRight = new Vector3(3.91667f, 3.291667f, 4);
            corners.bottomLeft = new Vector3(-4, 3.302083f, 4);


            PrepareCaveVRDevice(
                name: "CaveFrontWall_Top",
                windowRect: new RectInt(4480, 0, 1280, 720),
                corners,
                debugColor: new Color(0, 1, 0),
                numClients: 7,
                nodeType: VRDevice.VRNodeType.NetServer,
                unityButtonsAndMovesToEvents: true
                );
        }

        [MenuItem("MinVR/Prepare VR Devices.../Front Bottom Cave Wall")]
        public static void PrepareCaveFrontBottomWall()
        {
            var corners = new TrackedProjectionScreen.ScreenCorners();
            corners.topLeft = new Vector3(-4, 4.338542f, 4);
            corners.topRight = new Vector3(3.91667f, 4.291667f, 4);
            corners.bottomRight = new Vector3(3.91667f, 0, 4);
            corners.bottomLeft = new Vector3(-4, 0, 4);


            PrepareCaveVRDevice(
                name: "CaveFrontWall_Bottom",
                windowRect: new RectInt(4480, 720, 1280, 720),
                corners,
                debugColor: new Color(0, 1, 0)
                );
        }

        [MenuItem("MinVR/Prepare VR Devices.../Left Top Cave Wall")]
        public static void PrepareCaveLeftTopWall()
        {
            var corners = new TrackedProjectionScreen.ScreenCorners();
            corners.topLeft = new Vector3(-4, 7.66666f, -3.75f);
            corners.topRight = new Vector3(-4, 7.708333f, 4);
            corners.bottomRight = new Vector3(-4, 3.302083f, 4);
            corners.bottomLeft = new Vector3(-4, 3.307292f, -3.75f);


            PrepareCaveVRDevice(
                name: "CaveLeftWall_Top",
                windowRect: new RectInt(1920, 0, 1280, 720),
                corners,
                debugColor: new Color(0, 0.725f, 1)
                );
        }

        [MenuItem("MinVR/Prepare VR Devices.../Left Bottom Cave Wall")]
        public static void PrepareCaveLeftBottomWall()
        {
            var corners = new TrackedProjectionScreen.ScreenCorners();
            corners.topLeft = new Vector3(-4, 4.338542f, -3.75f);
            corners.topRight = new Vector3(-4, 4.338542f, 4);
            corners.bottomRight = new Vector3(-4, 0, 4);
            corners.bottomLeft = new Vector3(-4, 0, -3.802083f);


            PrepareCaveVRDevice(
                name: "CaveLeftWall_Bottom",
                windowRect: new RectInt(1920, 720, 1280, 720),
                corners,
                debugColor: new Color(0, 0.725f, 1)
                );
        }

        [MenuItem("MinVR/Prepare VR Devices.../Right Top Cave Wall")]
        public static void PrepareCaveRightTopWall()
        {
            var corners = new TrackedProjectionScreen.ScreenCorners();
            corners.topLeft = new Vector3(3.91667f, 7.66667f, 4);
            corners.topRight = new Vector3(3.91667f, 7.66667f, -3.729167f);
            corners.bottomRight = new Vector3(3.91667f, 3.260417f, -3.875f);
            corners.bottomLeft = new Vector3(3.91667f, 3.260417f, 4);


            PrepareCaveVRDevice(
                name: "CaveRightWall_Top",
                windowRect: new RectInt(3200, 0, 1280, 720),
                corners,
                debugColor: new Color(0.98f, 1, 0)
                );
        }

        [MenuItem("MinVR/Prepare VR Devices.../Right Bottom Cave Wall")]
        public static void PrepareCaveRightBottomWall()
        {
            var corners = new TrackedProjectionScreen.ScreenCorners();
            corners.topLeft = new Vector3(3.91667f, 4.3125f, 4);
            corners.topRight = new Vector3(3.91667f, 4.302083f, -3.75f);
            corners.bottomRight = new Vector3(3.91667f, -0.01041667f, -3.84375f);
            corners.bottomLeft = new Vector3(3.91667f, 0, 4);


            PrepareCaveVRDevice(
                name: "CaveRightWall_Bottom",
                windowRect: new RectInt(3200, 720, 1280, 720),
                corners,
                debugColor: new Color(0.99f, 1, 0)
                );
        }

        [MenuItem("MinVR/Prepare VR Devices.../Top Cave Floor")]
        public static void PrepareCaveTopFloor()
        {
            var corners = new TrackedProjectionScreen.ScreenCorners();
            corners.topLeft = new Vector3(-4, 0, -3.380208f);
            corners.topRight = new Vector3(-4.038f, 0, 4);
            corners.bottomRight = new Vector3(0.1041667f, 0, 4);
            corners.bottomLeft = new Vector3(0.125f, 0, -3.354167f);


            PrepareCaveVRDevice(
                name: "CaveFloor_Top",
                windowRect: new RectInt(5760, 0, 1280, 720),
                corners,
                debugColor: new Color(1, 0.19f, 0)
                );
        }

        [MenuItem("MinVR/Prepare VR Devices.../Bottom Cave Floor")]
        public static void PrepareCaveBottomFloor()
        {
            var corners = new TrackedProjectionScreen.ScreenCorners();
            corners.topLeft = new Vector3(-0.15625f, -4, -3.354167f);
            corners.topRight = new Vector3(-0.1822917f, -4, 4);
            corners.bottomRight = new Vector3(3.91667f, -4, 4);
            corners.bottomLeft = new Vector3(3.91667f, -4, -3.375f);


            PrepareCaveVRDevice(
                name: "CaveFloor_Bottom",
                windowRect: new RectInt(5760, 720, 1280, 720),
                corners,
                debugColor: new Color(1, 0.19f, 0)
                );
        }

        [MenuItem("MinVR/Prepare VR Devices.../Desktop", priority = 10)]
        public static void PrepareDesktopVRDevice()
        {
            var devices = PrepareVRDevicesParent();

            var obj = GetGameObject("Desktop");
            obj.transform.parent = devices.transform;
            
            var device = ForceComponent<VRDevice>(ref obj);
            device.vrNodeType = VRDevice.VRNodeType.StandAlone;
            device.serverIPAddress = VR_DEVICE_IP;
            device.serverPort = 3490;
            device.numClients = 1;

            device.windowXPos = 1700;
            device.windowYPos = 50;
            device.windowWidth = 200;
            device.windowHeight = 200;

            device.unityMouseBtnsToVREvents = true;
            device.unityMouseMovesToVREvents = true;


            var tracker = ForceComponent<FakeTrackingInput>(ref obj);
            tracker.fakeHeadTrackerEvent = HEAD_TRACKING_EVENT;
            tracker.initialHeadPos = new Vector3(0, 0, -2);

            tracker.fakeTracker1Event = "BlueStylus_Move";

            tracker.fakeTracker2Event = "Brush_Move";


            ForceComponent<TrackedDesktopCamera>(ref obj).trackingEvent = HEAD_TRACKING_EVENT;
        }

        public static void PrepareCaveVRDevice(
            string name,
            RectInt windowRect,
            TrackedProjectionScreen.ScreenCorners corners,
            Color debugColor,
            int numClients = 1,
            VRDevice.VRNodeType nodeType = VRDevice.VRNodeType.NetClient,
            bool unityButtonsAndMovesToEvents = false
            )
        {
            var devices = PrepareVRDevicesParent();

            var obj = GetGameObject(name);
            obj.transform.parent = devices.transform;

            var device = ForceComponent<VRDevice>(ref obj);
            device.vrNodeType = nodeType;
            device.serverIPAddress = VR_DEVICE_IP;
            device.serverPort = 3490;
            device.numClients = numClients;

            device.windowXPos = windowRect.x;
            device.windowYPos = windowRect.y;
            device.windowWidth = windowRect.width;
            device.windowHeight = windowRect.height;

            device.unityMouseBtnsToVREvents = unityButtonsAndMovesToEvents;
            device.unityMouseMovesToVREvents = unityButtonsAndMovesToEvents;

            var screen = ForceComponent<TrackedProjectionScreen>(ref obj);
            screen.trackingSpaceCorners = corners;
            screen.headTrackingEvent = HEAD_TRACKING_EVENT;
            screen.projectionType = TrackedProjectionScreen.ProjectionType.Perspective;
            screen.debugColor = debugColor;

            obj.SetActive(false);
        }
    }
}