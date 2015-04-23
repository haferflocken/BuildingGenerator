// Handles creating and manipulating composite shapes.

#pragma once

#ifndef INCLUDED_COMPOSITE_SHAPE_MANAGER_H
#define INCLUDED_COMPOSITE_SHAPE_MANAGER_H

#include "Vector4.h"
#include "Quaternion.h"
#include "CompositeShape.h"

typedef int CompositeShapeID;

class CompositeShapeManager
{
public:
    static CompositeShapeManager s_Instance;

    CompositeShapeManager()
        : m_shapes()
    {
        // Quick dumb setup TODO(jwerner) remove
        CSGCuboid cuboid(Vector4(), Vector4(1.0, 1.0, 1.0, 1.0), Quaternion());
        CompositeShape compositeTest;
        compositeTest.Union(cuboid);
        m_shapes.push_back(compositeTest);
    }

    bool CompositeContains(CompositeShapeID id, const Vector4& position) const;

private:
    std::vector<CompositeShape> m_shapes;
};

#endif // INCLUDED_COMPOSITE_SHAPE_MANAGER_H
