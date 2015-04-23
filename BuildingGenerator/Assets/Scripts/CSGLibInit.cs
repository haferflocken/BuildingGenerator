using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class CSGLibInit : MonoBehaviour
{
	unsafe static void DebugOutput(char* characters)
	{
		string output = Marshal.PtrToStringAnsi(new IntPtr(characters));
		Debug.Log(output);
	}

	static void DebugBreak()
	{
		Debug.Break();
	}

	unsafe delegate void DebugOutputDelegate(char* characters);
	delegate void DebugBreakDelegate();

	private DebugOutputDelegate _outputDelegate;
	private DebugBreakDelegate _breakDelegate;

	private GCHandle _outputHandle;
	private GCHandle _breakHandle;

	unsafe void Start()
	{
		_outputDelegate = DebugOutput;
		_breakDelegate = DebugBreak;

		_outputHandle = GCHandle.Alloc(_outputDelegate);
		_breakHandle = GCHandle.Alloc(_breakDelegate);

		CSGLib.RegisterDebugOutput(Marshal.GetFunctionPointerForDelegate(_outputDelegate));
		CSGLib.RegisterDebugBreak(Marshal.GetFunctionPointerForDelegate(_breakDelegate));
	}

	unsafe void OnDestroy()
	{
		_outputHandle.Free();
		_breakHandle.Free();
	}
}
