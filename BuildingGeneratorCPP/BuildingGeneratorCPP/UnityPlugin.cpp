// This file surfaces functions to Unity so it may use them.

#if _MSC_VER // this is defined when compiling with Visual Studio
#define EXPORT_API __declspec(dllexport) // Visual Studio needs annotating exported functions with this
#else
#define EXPORT_API // XCode does not need annotating exported functions, so define is empty
#endif

#include "UnityPlugin.h"
#include "CompositeShapeManager.h"

// ------------------------------------------------------------------------

extern "C"
{
    void EXPORT_API RegisterDebugOutput(void (__stdcall* pHandler)(char* message))
    {
        UnityPlugin::DebugOutput = pHandler;
        UnityPlugin::DebugOutput("Debug output handler registered.");
    }

    void EXPORT_API RegisterDebugBreak(void(__stdcall* pHandler)())
    {
        UnityPlugin::DebugBreak = pHandler;
        UnityPlugin::DebugOutput("Debug break handler registered.");
    }

    int EXPORT_API TestContains(double x, double y, double z)
    {
        if (CompositeShapeManager::s_Instance.CompositeContains(0, Vector4(x, y, z, 1)))
        {
            return 1;
        }
        return 0;
    }
}
