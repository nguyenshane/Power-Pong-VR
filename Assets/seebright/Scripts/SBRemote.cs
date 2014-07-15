using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

/**
 * Seebright Remote v1.0 - Seebright, Inc
 *
 *  Copyright 2014 Seebright. 
 *  All rights reserved.
 * 
 * @author      John Murray - <john@seebright.com>
 * @author      Scott Holman - <scott@seebright.com>
 * @version     1.0
 * @data		2014
 * 
 */

public class SBRemote
{

	// These are the current representations of the button states on remote
	
	public const string JOY_HORIZONTAL = "joyHorizontal";
	public const string JOY_VERTICAL = "joyVertical";
	public const string BUTTON_SELECT = "buttonSelect";
	public const string BUTTON_BACK = "buttonBack";
	public const string BUTTON_OPTION = "buttonOption";
	public const string BUTTON_UP = "buttonUp";
	public const string BUTTON_DOWN = "buttonDown";
	public const string BUTTON_TRIGGER = "buttonTrigger";
	public const float MAX_JOY_HORIZONTAL = 32767.0f;
	public const float MAX_JOY_VERTICAL = 32767.0f;
	public string CURSOR_TRACKER_NAME = "metaioTracker";
	
	//Operating System Suffix
	private static string operatingSystem = "_OSX";

	protected static float prev_joy_x = joy_x;
	protected static float prev_joy_y = joy_y;
	protected static short joy_x;
	protected static short joy_y;
	protected static short quat_x;
	protected static short quat_y;
	protected static short quat_z;
	protected static short quat_w;
	protected static bool button_select;
	protected static bool button_back;
	protected static bool button_option;
	protected static bool button_up;
	protected static bool button_down;
	protected static bool button_trigger;
	public static Quaternion remoteOrientation = new Quaternion ();
	public static string curData = "";
	private Vector2 pivotPoint;
	protected static bool down_button_select;
	protected static bool down_button_back;
	protected static bool down_button_option;
	protected static bool down_button_up;
	protected static bool down_button_down;
	protected static bool down_button_trigger;
	protected static bool up_button_select;
	protected static bool up_button_back;
	protected static bool up_button_option;
	protected static bool up_button_up;
	protected static bool up_button_down;
	protected static bool up_button_trigger;



	Ray ray;
	Vector3 rayPoint;
	Vector3 rayDest;
	Vector3 rayDir;
	
	Quaternion myRot;

	public static bool remoteStatus;
	private static String remoteUUID;

	private static IntPtr remoteData = IntPtr.Zero;
	GCHandle gch;


	public SBRemote ()
	{

	}

	private static byte[] myBytes= new byte[31];

	public static void remoteLateUpdate ()
	{
		//iPhone Remote Control Handler
		bool dataAvail = false;
		#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		
		bool prev_button_select = button_select;
		bool prev_button_back = button_back;
		bool prev_button_option = button_option;
		bool prev_button_up = button_up;
		bool prev_button_down = button_down;
		bool prev_button_trigger = button_trigger;
		#endif
		#if (UNITY_IPHONE) && !UNITY_EDITOR
		dataAvail = _getPacket(ref remoteData);
		#elif UNITY_ANDROID && !UNITY_EDITOR
		dataAvail = _getPacket();
		#endif
		#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		if(remoteStatus&& remoteData!=IntPtr.Zero && dataAvail) {
			
			Marshal.Copy (remoteData,
			              myBytes,
			              0,
			              30);
			joy_x = System.BitConverter.ToInt16(myBytes,20);
			joy_y = System.BitConverter.ToInt16(myBytes,22);
			
			int remoteOrientationXint = System.BitConverter.ToInt32(myBytes, 3);
			remoteOrientation.y = remoteOrientationXint/(2^32);
			int remoteOrientationYint = System.BitConverter.ToInt32(myBytes, 7);
			remoteOrientation.x = remoteOrientationYint/(2^32);
			int remoteOrientationZint = System.BitConverter.ToInt32(myBytes, 11);
			remoteOrientation.z = remoteOrientationZint/(2^32);
			int remoteOrientationWint = System.BitConverter.ToInt32(myBytes, 15);
			remoteOrientation.w = remoteOrientationWint/(2^32);
			byte _buttons = myBytes[19];
			button_select=(_buttons&1)==1;
			button_back=((_buttons&1<<2)>>2)==1;
			button_option=((_buttons&1<<3)>>3)==1;
			button_trigger=((_buttons&1<<4)>>4)==1;
			button_down=((_buttons&1<<6)>>6)==1;
			button_up=((_buttons&1<<7)>>7)==1;
			
			SBCursors.cursorPeriscopePosition.x = (System.BitConverter.ToInt16(myBytes,24)) / 960.0f;
			SBCursors.cursorPeriscopePosition.y = 1- ((System.BitConverter.ToInt16(myBytes,26)) / 540.0f);
			SBCursors.cursorPeriscopePosition.z = (System.BitConverter.ToInt16(myBytes,28)) / 1000f;
			
			//curData = string.Format("{0} {1} {2} {3} {4} {5} {6} {7}", 
			//                        joy_x,joy_y,button_select,button_option,button_back,button_up,button_down,button_trigger);
			
		} else
			curData = "Waiting...";
		
		up_button_select = (!prev_button_select && button_select);
		up_button_back = (!prev_button_back && button_back);
		up_button_option = (!prev_button_option && button_option);
		up_button_up = (!prev_button_up && button_up);
		up_button_down = (!prev_button_down && button_down);
		up_button_trigger = (!prev_button_trigger && button_trigger);
		
		down_button_select = (prev_button_select && !button_select && !down_button_select);
		down_button_back = (prev_button_back && !button_back && !down_button_back);
		down_button_option = (prev_button_option && !button_option && !down_button_option);
		down_button_up = (prev_button_up && !button_up && !down_button_up);
		down_button_down = (prev_button_down && !button_down && !down_button_down);
		down_button_trigger = (prev_button_trigger && !button_trigger && !down_button_trigger);
		#endif
		
	}

	public static float GetAxis (string axis)
	{
		float retVal = 0.0f;
		switch (axis) {
			#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR    
		case JOY_HORIZONTAL:
			retVal = joy_x/MAX_JOY_HORIZONTAL;
			break;
		case JOY_VERTICAL:
			retVal = joy_y/MAX_JOY_VERTICAL;
			break;
			#else
		case JOY_HORIZONTAL:
			retVal = Input.GetAxis ("Seebright_Horizontal" + operatingSystem);
			break;
		case JOY_VERTICAL:
			retVal = Input.GetAxis ("Seebright_Vertical" + operatingSystem);
			break;
			#endif
		default:
			retVal = 0.0f;
			break;
		}
		return retVal;
	}
	
	public static bool GetButton (string buttonName)
	{
		
		bool retVal = false;    
		switch (buttonName) {
			#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR    
		case BUTTON_SELECT:
			retVal=button_select;
			break;
		case BUTTON_BACK:
			retVal=button_back;
			break;
		case BUTTON_OPTION:
			retVal=button_option;
			break;
		case BUTTON_UP:
			retVal=button_up;
			break;
		case BUTTON_DOWN:
			retVal=button_down;
			break;
		case BUTTON_TRIGGER:
			retVal=button_trigger;
			break;
			#else
		case BUTTON_SELECT:
			retVal = Input.GetButton ("Seebright_selectButton" + operatingSystem);
			break;
		case BUTTON_BACK:
			retVal = Input.GetButton ("Seebright_backButton" + operatingSystem);
			break;
		case BUTTON_OPTION:
			retVal = Input.GetButton ("Seebright_optionButton" + operatingSystem);
			break;
		case BUTTON_DOWN:
			retVal = Input.GetButton ("Seebright_downButton" + operatingSystem);
			break;
		case BUTTON_UP:
			retVal = Input.GetButton ("Seebright_upButton" + operatingSystem);
			break;
		case BUTTON_TRIGGER:
			retVal = Input.GetButton ("Seebright_triggerButton" + operatingSystem);
			break;
			#endif
		default:
			retVal = false;
			break;
		}
		
		return retVal;
	}
	
	public static bool GetButtonUp (string buttonName)
	{
		bool retVal = false;    
		switch (buttonName) {
			#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR    
		case BUTTON_SELECT:
			retVal=up_button_trigger;
			break;
		case BUTTON_BACK:
			retVal=up_button_back    ;
			break;
		case BUTTON_OPTION:
			retVal=up_button_option;
			break;
		case BUTTON_UP:
			retVal=up_button_up;
			break;
		case BUTTON_DOWN:
			retVal=up_button_down;
			break;
		case BUTTON_TRIGGER:
			retVal=up_button_trigger;
			break;
			#else
		case BUTTON_SELECT:
			retVal = Input.GetButtonUp ("Seebright_selectButton" + operatingSystem);
			break;
		case BUTTON_BACK:
			retVal = Input.GetButtonUp ("Seebright_backButton" + operatingSystem);
			break;
		case BUTTON_OPTION:
			retVal = Input.GetButtonUp ("Seebright_optionButton" + operatingSystem);
			break;
		case BUTTON_UP:
			retVal = Input.GetButtonUp ("Seebright_upButton" + operatingSystem);
			break;
		case BUTTON_DOWN:
			retVal = Input.GetButtonUp ("Seebright_downButton" + operatingSystem);
			break;
		case BUTTON_TRIGGER:
			retVal = Input.GetButtonUp ("Seebright_triggerButton" + operatingSystem);
			break;
			#endif
		default:
			retVal = false;
			break;
		}
		return retVal;
	}
	
	public static bool GetButtonDown (string buttonName)
	{
		bool retVal = false;    
		switch (buttonName) {
			#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR    
		case BUTTON_SELECT:
			retVal=down_button_select;
			break;
		case BUTTON_BACK:
			retVal=down_button_back;
			break;
		case BUTTON_OPTION:
			retVal=down_button_option;
			break;
		case BUTTON_UP:
			retVal=down_button_up;
			break;
		case BUTTON_DOWN:
			retVal=down_button_down;
			break;
		case BUTTON_TRIGGER:
			retVal=down_button_trigger;
			break;
			#else
		case BUTTON_SELECT:
			retVal = Input.GetButtonDown ("Seebright_selectButton" + operatingSystem);
			break;
		case BUTTON_BACK:
			retVal = Input.GetButtonDown ("Seebright_backButton" + operatingSystem);
			break;
		case BUTTON_OPTION:
			retVal = Input.GetButtonDown ("Seebright_optionButton" + operatingSystem);
			break;
		case BUTTON_UP:
			retVal = Input.GetButtonDown ("Seebright_upButton" + operatingSystem);
			break;
		case BUTTON_DOWN:
			retVal = Input.GetButtonDown ("Seebright_downButton" + operatingSystem);
			break;
		case BUTTON_TRIGGER:
			retVal = Input.GetButtonDown ("Seebright_triggerButton" + operatingSystem);
			break;
			#endif
		default:
			retVal = false;
			break;
		}
		
		return retVal;
	}
	
	public static float GetJoystickDelta (string axis)
	{
		float delta = 0;
		if (axis == JOY_HORIZONTAL)
			delta = joy_x - prev_joy_x;
		else if (axis == JOY_VERTICAL)
			delta = joy_y - prev_joy_y;
		return delta;
	}
	
	public static Quaternion GetOrientation ()
	{
		return remoteOrientation;
	}

	
	// Provide hoods for communication with native code
	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	private static extern bool _getPacket(ref IntPtr ptrData);
	
	[DllImport ("__Internal")]
	private static extern string _StartService();
	
	[DllImport ("__Internal")]
	private static extern bool _remoteConnected();
	
	[DllImport ("__Internal")]
	private static extern void _getPeriphID();
	
	[DllImport ("__Internal")]
	private static extern void _launchApplication(string appID);

	[DllImport ("__Internal")]
	private static extern void _StartTracking();

	[DllImport ("__Internal")]
	private static extern void _StopTracking();
	#elif UNITY_ANDROID && !UNITY_EDITOR
	
	#endif
	#if UNITY_ANDROID  && !UNITY_EDITOR
	private static AndroidJavaClass ajc;
	
	private static void _StartService() {
		ajc = new AndroidJavaClass("com.seebright.seebrighthook.Bridge");
		ajc.CallStatic("startService");
		remoteData = Marshal.AllocHGlobal(31);
		return;
	}
	private static bool _remoteConnected() {
		if(ajc!=null)
			return ajc.CallStatic<bool>("remoteConnected");
		else
			return false;
	}
	
	private static bool _getPacket() {
		if(ajc!=null && _remoteConnected()) {
			AndroidJavaObject ret = ajc.CallStatic<AndroidJavaObject>("getPacket");
			if(ret.GetRawObject().ToInt32() != 0) {
				Marshal.Copy (AndroidJNIHelper.ConvertFromJNIArray<byte[]>(ret.GetRawObject()), 0,
				              remoteData,
				              20);
				return true;
			} else
				return false;
		} else 
			return false;
	}
	
	private static void _StopService() {
		ajc.CallStatic("stopService");
		ajc.Dispose();
	}
	#endif
	
	// Use this for initialization
	public static void InitializeRemote ()
	{
		// Prevent screen from dimming
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		// Start connection to the remote
		StartService ();
		if(SeebrightSDK.singleton.enableBallTracking)
		{
			StartTracking();
		}

		//Set the Operating System Suffix
		#if UNITY_EDITOR_OSX
		operatingSystem="_OSX";
		#elif UNITY_EDITOR
		operatingSystem = "_WIN";
		#endif
	}
	
	public static void StartService() {
		#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		_StartService();
		#endif
	}
	
	public static void StopService() {
		#if (UNITY_ANDROID) && !UNITY_EDITOR
		_StopService();
		#endif
	}

	public static void StartTracking()
	{
		#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		_StartTracking();
		#endif
	}

	public static void StopTracking()
	{
		#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		_StopTracking();
		#endif
	}

	public static void LaunchApplication(String appID)
	{
		#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		_launchApplication(appID);
		#endif
	}
	
	
	public static void updateRemoteStatus ()
	{
		
		#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
		remoteStatus = _remoteConnected();
		#endif
	}
	
	public static void setRemoteStatus (String newStatus)
	{
		remoteUUID = newStatus;
	}

	public static string printRemoteControls ()
	{
		return 
			"JH:" + GetAxis (JOY_HORIZONTAL) * MAX_JOY_HORIZONTAL +
				"JV:" + GetAxis (JOY_VERTICAL) * MAX_JOY_VERTICAL +
				"S:" + (GetButton (BUTTON_SELECT) ? "Y" : "N") +
				"B:" + (GetButton (BUTTON_BACK) ? "Y" : "N") +
				"O:" + (GetButton (BUTTON_OPTION) ? "Y" : "N") +
				"U:" + (GetButton (BUTTON_UP) ? "Y" : "N") +
				"D:" + (GetButton (BUTTON_DOWN) ? "Y" : "N") +
				"T:" + (GetButton (BUTTON_TRIGGER) ? "Y" : "N");
	}

}

