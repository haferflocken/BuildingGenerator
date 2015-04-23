
#include "Quaternion.h"

Quaternion::Quaternion()
    : a(1.0)
    , b(0.0)
    , c(0.0)
    , d(0.0)
{
}

Quaternion::Quaternion(double a0, double b0, double c0, double d0)
    : a(a0)
    , b(b0)
    , c(c0)
    , d(d0)
{
}

Quaternion::Quaternion(const Quaternion& other)
{
    *this = other;
}

Quaternion::Quaternion(Quaternion&& other)
{
    *this = other;
}

void Quaternion::operator=(const Quaternion& rhs)
{
    a = rhs.a;
    b = rhs.b;
    c = rhs.c;
    d = rhs.d;
}
