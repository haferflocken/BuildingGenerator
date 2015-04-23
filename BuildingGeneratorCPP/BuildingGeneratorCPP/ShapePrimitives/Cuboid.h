// A CSG cuboid.

#pragma once

#ifndef INCLUDED_CSG_CUBOID_H
#define INCLUDED_CSG_CUBOID_H

#include "../Matrix4x4.h"
#include "../Vector4.h"

class Quaternion;

class CSGCuboid
{
public:
    CSGCuboid();
    CSGCuboid(const Vector4& position, const Vector4& dimensions, const Quaternion& orientation);
    explicit CSGCuboid(const CSGCuboid& other);
    explicit CSGCuboid(CSGCuboid&& other);

    bool Equals(const CSGCuboid& other) const;
    bool Contains(const Vector4& point) const;

    double CalcVolume() const;

    void operator=(const CSGCuboid& rhs);

    void SetPosition(const Vector4& position);
    void SetDimensions(const Vector4& dimensions);

private:
    Matrix4x4 m_localToCompositeMatrix; // The position of one of the vertexes of the cuboid, called the anchor, in composite space.
    Vector4 m_dimensions; // The dimensions define the position of the other vertexes relative to the anchor in the local space. Treated as a 3D vector.
};

#endif // INCLUDED_CSG_CUBOID_H
