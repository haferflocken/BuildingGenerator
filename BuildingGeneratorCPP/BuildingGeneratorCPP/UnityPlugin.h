// A place to put things to call in Unity.

#pragma once

#ifndef INCLUDED_UNITYPLUGIN_H
#define INCLUDED_UNITYPLUGIN_H

namespace UnityPlugin
{
    static void(__stdcall* DebugOutput)(char* message);
    static void(__stdcall* DebugBreak)();
}

#endif // INCLUDED_UNITYPLUGIN_H
