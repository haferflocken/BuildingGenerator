// A composite of various shape primitives using boolean operators.

#pragma once

#ifndef INCLUDED_COMPOSITESHAPE_H
#define INCLUDED_COMPOSITESHAPE_H

#include "ShapePrimitives/Cuboid.h"

#include <vector>

/////////////////////////////////////////////////////////////////////////

enum class CSGShapes
{
    Invalid = -1,
    Cuboid = 0,
};

enum class ShapeOperations
{
    Invalid = -1,
    Shape = 0,
    Union = 1,
    Difference = 2,
    Intersection = 3,
};

/////////////////////////////////////////////////////////////////////////

class CompositeShape
{
public:
    CompositeShape();

    bool Contains(const Vector4& point) const; // The point is treated as a 3D vector.

    double CalcVolume() const;

    inline void Union(const CSGCuboid& cuboid)
    {
        // TODO(jwerner) remove. this is a quick add for testing
        ShapeUnion shapeUnion;
        shapeUnion.shapeType = CSGShapes::Cuboid;
        shapeUnion.cuboid = cuboid;
        m_shapes.push_back(shapeUnion);

        CompositeNode node;
        node.operation = ShapeOperations::Shape;
        node.shape = m_shapes.size() - 1;
        m_nodes.push_back(node);
    }
    
private:
    struct ShapeUnion
    {
        ShapeUnion()
            : shapeType(CSGShapes::Invalid)
        {
        }

        CSGShapes shapeType;
        union
        {
            struct { CSGCuboid cuboid; }; // Wrapped in a struct to let the union work.
        };
    };

    struct CompositeNode
    {
        CompositeNode()
            : operation(ShapeOperations::Invalid)
            , left(nullptr)
            , right(nullptr)
            , shape(-1)
        {
        }

        ShapeOperations operation;
        union // This union is here to make it apparent that a node either has left & right or a shapeIndex, but is it actually any clearer?
        {
            struct // Accessible when operation != Shape
            {
                CompositeNode* left;
                CompositeNode* right;
            };
            size_t shape; // Accessible when operation == Shape
        };
    };

    std::vector<ShapeUnion> m_shapes;
    std::vector<CompositeNode> m_nodes;
    Vector4 m_position; // Treated as a 3D vector.
};

#endif // INCLUDED_COMPOSITESHAPE_H
