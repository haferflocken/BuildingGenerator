using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class CSGLib
{
	[DllImport("BuildingGeneratorCPP")]
	public static extern void RegisterDebugOutput(IntPtr pHandler);

	[DllImport("BuildingGeneratorCPP")]
	public static extern void RegisterDebugBreak(IntPtr pHandler);

	[DllImport("BuildingGeneratorCPP")]
	public static extern int TestContains(double x, double y, double z);

}
