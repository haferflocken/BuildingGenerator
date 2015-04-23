// A four dimensional vector of doubles.

#pragma once

#ifndef INCLUDED_VECTOR4_H
#define INCLUDED_VECTOR4_H

class Vector4
{
public:
    Vector4();
    Vector4(double x, double y, double z, double w);
    Vector4(const Vector4& other);
    Vector4(Vector4&& other);

    bool Equals(const Vector4& other) const;
    bool GreaterThan(const Vector4& other) const;
    bool LessThan(const Vector4& other) const;
    double Dot(const Vector4& other) const;
    Vector4 Cross(const Vector4& other) const; // Worth noting that 4D vectors do not have a cross product- this is a 3D operation.

    void operator=(const Vector4& rhs);
    void operator+=(const Vector4& rhs);
    void operator-=(const Vector4& rhs);
    void operator*=(double rhs);
    void operator/=(double rhs);

    double x;
    double y;
    double z;
    double w;
};

inline Vector4 operator+(const Vector4& lhs, const Vector4& rhs)
{
    return Vector4(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.w + rhs.w);
}

inline Vector4 operator-(const Vector4& lhs, const Vector4& rhs)
{
    return Vector4(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.w - rhs.w);
}

inline Vector4 operator*(const Vector4& lhs, double rhs)
{
    return Vector4(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs, lhs.w * rhs);
}

inline Vector4 operator/(const Vector4& lhs, double rhs)
{
    return Vector4(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs, lhs.w * rhs);
}

#endif // INCLUDED_Vector4_H
