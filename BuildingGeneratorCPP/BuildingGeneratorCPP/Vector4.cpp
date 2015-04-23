
#include "Vector4.h"

Vector4::Vector4()
    : x(0.0)
    , y(0.0)
    , z(0.0)
    , w(0.0)
{
}

Vector4::Vector4(double x0, double y0, double z0, double w0)
    : x(x0)
    , y(y0)
    , z(z0)
    , w(w0)
{
}

Vector4::Vector4(const Vector4& other)
    : x(other.x)
    , y(other.y)
    , z(other.z)
    , w(other.w)
{
}

Vector4::Vector4(Vector4&& other)
    : x(other.x)
    , y(other.y)
    , z(other.z)
    , w(other.w)
{
}

bool Vector4::Equals(const Vector4& other) const
{
    return (x == other.x && y == other.y && z == other.z && w == other.w);
}

bool Vector4::GreaterThan(const Vector4& other) const
{
    return (x > other.x) && (y > other.y) && (z > other.z) && (w > other.w);
}

bool Vector4::LessThan(const Vector4& other) const
{
    return (x < other.x) && (y < other.y) && (z < other.z) && (w < other.w);
}

double Vector4::Dot(const Vector4& other) const
{
    return (x * other.x) + (y * other.y) + (z * other.z) + (w * other.w);
}

Vector4 Vector4::Cross(const Vector4& other) const
{
    double cX = y * other.z - z * other.y;
    double cY = z * other.x - x * other.z;
    double cZ = x * other.y - y * other.x;
    return Vector4(cX, cY, cZ, 1.0);
}

void Vector4::operator=(const Vector4& rhs)
{
    x = rhs.x;
    y = rhs.y;
    z = rhs.z;
    w = rhs.w;
}

void Vector4::operator+=(const Vector4& rhs)
{
    x += rhs.x;
    y += rhs.y;
    z += rhs.z;
    w += rhs.w;
}

void Vector4::operator-=(const Vector4& rhs)
{
    x -= rhs.x;
    y -= rhs.y;
    z -= rhs.z;
    w -= rhs.w;
}

void Vector4::operator*=(double rhs)
{
    x *= rhs;
    y *= rhs;
    z *= rhs;
    w *= rhs;
}

void Vector4::operator/=(double rhs)
{
    x /= rhs;
    y /= rhs;
    z /= rhs;
    w /= rhs;
}
