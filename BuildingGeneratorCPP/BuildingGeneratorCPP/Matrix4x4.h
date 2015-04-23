// A 4x4 matrix for transformations. Column major so as to line up with Unity.

#pragma once

#ifndef INCLUDED_MATRIX4X4_H
#define INCLUDED_MATRIX4X4_H

#include "Vector4.h"

class Quaternion;

class Matrix4x4
{
public:
    Matrix4x4();
    Matrix4x4(const Vector4& position, const Quaternion& orientation);
    Matrix4x4(const Matrix4x4& other);
    Matrix4x4(Matrix4x4&& other);

    bool Equals(const Matrix4x4& other) const;

    const double* operator[](size_t index) const { return data[index]; }
    double* operator[](size_t index) { return data[index]; }

    Matrix4x4 CalcInverse() const;
    Matrix4x4 CalcInverseTransform() const; // Calculates the inverse quickly for transformation matricies.

    void operator=(const Matrix4x4& rhs);

private:
    double data[4][4];
};

inline Matrix4x4 operator*(const Matrix4x4& lhs, const Matrix4x4& rhs)
{
    Matrix4x4 out;

    for (size_t i = 0; i < 4; ++i)
    {
        for (size_t j = 0; j < 4; ++j)
        {
            out[i][j] = lhs[i][j] * rhs[j][i];
        }
    }

    return out;
}

inline Vector4 operator*(const Matrix4x4& lhs, const Vector4& rhs)
{
    double outX = (lhs[0][0] * rhs.x) + (lhs[0][1] * rhs.y) + (lhs[0][2] * rhs.z) + (lhs[0][3] * rhs.w);
    double outY = (lhs[1][0] * rhs.x) + (lhs[1][1] * rhs.y) + (lhs[1][2] * rhs.z) + (lhs[1][3] * rhs.w);
    double outZ = (lhs[2][0] * rhs.x) + (lhs[2][1] * rhs.y) + (lhs[2][2] * rhs.z) + (lhs[2][3] * rhs.w);
    double outW = (lhs[3][0] * rhs.x) + (lhs[3][1] * rhs.y) + (lhs[3][2] * rhs.z) + (lhs[3][3] * rhs.w);

    return Vector4(outX, outY, outZ, outW);
}

#endif // INCLUDED_MATRIX4X4_H
