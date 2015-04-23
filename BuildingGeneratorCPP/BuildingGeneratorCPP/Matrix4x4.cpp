
#include "Matrix4x4.h"
#include "Vector4.h"
#include "Quaternion.h"

#include "DebugUtils.h"

#include <algorithm>

Matrix4x4::Matrix4x4()
{
    for (size_t i = 0; i < 4; ++i)
    {
        for (size_t j = 0; j < 4; ++j)
        {
            data[i][j] = 0.0;
        }
    }
}

Matrix4x4::Matrix4x4(const Vector4& position, const Quaternion& orientation)
{
    // Quaternion to rotation matrix from http://www.euclideanspace.com/maths/geometry/rotations/conversions/quaternionToMatrix/
    data[0][0] = 1.0 - (2.0 * orientation.c * orientation.c) - (2.0 * orientation.d * orientation.d);
    data[0][1] = (2.0 * orientation.b * orientation.c) + (2.0 * orientation.d * orientation.a);
    data[0][2] = (2.0 * orientation.b * orientation.d) - (2.0 * orientation.c * orientation.a);
    data[0][3] = 0.0;

    data[1][0] = (2.0 * orientation.b * orientation.c) - (2.0 * orientation.d * orientation.a);
    data[1][1] = 1.0 - (2.0 * orientation.b * orientation.b) - (2.0 * orientation.d * orientation.d);
    data[1][2] = (2.0 * orientation.c * orientation.d) + (2.0 * orientation.b * orientation.a);
    data[1][3] = 0.0;

    data[2][0] = (2.0 * orientation.b * orientation.d) + (2.0 * orientation.c * orientation.a);
    data[2][1] = (2.0 * orientation.c * orientation.d) - (2.0 * orientation.b * orientation.a);
    data[2][2] = 1.0 - (2.0 * orientation.b * orientation.b) - (2.0 * orientation.c * orientation.c);
    data[2][3] = 0.0;

    // Position just becomes the translation component.
    data[3][0] = position.x;
    data[3][1] = position.y;
    data[3][2] = position.z;
    data[3][3] = position.w;
}

Matrix4x4::Matrix4x4(const Matrix4x4& other)
{
    *this = other;
}

Matrix4x4::Matrix4x4(Matrix4x4&& other)
{
    *this = other;
}

bool Matrix4x4::Equals(const Matrix4x4& other) const
{
    for (size_t i = 0; i < 4; ++i)
    {
        for (size_t j = 0; j < 4; ++j)
        {
            if (data[i][j] != other.data[i][j])
            {
                return false;
            }
        }
    }
    return true;
}

Matrix4x4 Matrix4x4::CalcInverse() const
{
    // TODO(jwerner)
    return Matrix4x4();
}

// Algorithm from http://graphics.stanford.edu/courses/cs248-98-fall/Final/q4.html
Matrix4x4 Matrix4x4::CalcInverseTransform() const
{
    dbBreakf("test");

    dbAssertf(data[0][3] == 0.0, "Matrix is not a transformation matrix.");
    dbAssertf(data[1][3] == 0.0, "Matrix is not a transformation matrix.");
    dbAssertf(data[2][3] == 0.0, "Matrix is not a transformation matrix.");
    dbAssertf(data[3][3] == 1.0, "Matrix is not a transformation matrix.");

    // Transpose the rotations, and the translation is the dot of the rotations with itself.
    Matrix4x4 out;

    for (size_t i = 0; i < 3; ++i)
    {
        for (size_t j = 0; j < 3; ++j)
        {
            out[i][j] = data[j][i];
        }
    }

    const double* u = data[0];
    const double* v = data[1];
    const double* w = data[2];
    const double* t = data[3];

    out[3][0] = (u[0] * t[0]) + (u[1] * t[1]) + (u[2] * t[2]);
    out[3][1] = (v[0] * t[0]) + (v[1] * t[1]) + (v[2] * t[2]);
    out[3][2] = (w[0] * t[0]) + (w[1] * t[1]) + (w[2] * t[2]);
    out[3][3] = 1.0;

    return out;
}

void Matrix4x4::operator=(const Matrix4x4& rhs)
{
    for (size_t i = 0; i < 4; ++i)
    {
        for (size_t j = 0; j < 4; ++j)
        {
            data[i][j] = rhs.data[i][j];
        }
    }
}
