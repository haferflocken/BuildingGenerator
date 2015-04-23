// Utility functions for debugging.

#pragma once

#ifndef INCLUDED_DEBUG_UTILS_H
#define INCLUDED_DEBUG_UTILS_H

#include <string>

#ifdef RELEASE
#include "UnityPlugin.h"

#define dbLogf(formatString, ...) \
    do \
    { \
        char out[256]; \
        sprintf_s(out, formatString, __VA_ARGS__); \
        UnityPlugin::DebugOutput(out); \
    } \
    while (false)

#define dbBreakf(formatString, ...) \
    do \
    { \
        dbLogf(formatString, __VA_ARGS__); \
        UnityPlugin::DebugBreak(); \
        __debugbreak(); \
    } \
    while (false)
    

#define dbAssertf(expr, formatString, ...) \
    do \
    { \
        if (expr) \
        { \
            dbBreakf(formatString, __VA_ARGS__); \
        } \
    } \
    while (false)

#else
#define dbLogf(formatString, ...)
#define dbBreakf(formatString, ...)
#define dbAssertf(expr, formatString, ...)
#endif

#endif // INCLUDED_DEBUG_UTILS_H
