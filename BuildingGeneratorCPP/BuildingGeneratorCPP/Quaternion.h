// A quaternion of doubles.

#pragma once

#ifndef INCLUDED_QUATERNION_H
#define INCLUDED_QUATERNION_H

class Quaternion
{
public:
    Quaternion(); // Identity quaternion.
    Quaternion(double a0, double b0, double c0, double d0);
    Quaternion(const Quaternion& other);
    Quaternion(Quaternion&& other);

    void operator=(const Quaternion& rhs);

    double a;
    double b;
    double c;
    double d;
};

#endif // INCLUDED_QUATERNION_H
