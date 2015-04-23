
#include "CompositeShape.h"
#include "DebugUtils.h"

#include <stack>

CompositeShape::CompositeShape()
    : m_shapes()
    , m_nodes()
    , m_position()
{
}

bool CompositeShape::Contains(const Vector4& point) const
{
    if (m_nodes.empty())
    {
        return false;
    }

    // Turn the composition tree into an input vector that can be evaluated as RPN.
    std::stack<const CompositeNode*> nodeStack;
    nodeStack.push(&m_nodes.front());

    std::vector<const CompositeNode*> rpnInput;

    while (!nodeStack.empty())
    {
        const CompositeNode& node = *(nodeStack.top());
        nodeStack.pop();

        rpnInput.push_back(&node);

        switch (node.operation)
        {
        case ShapeOperations::Shape:
            break;

        case ShapeOperations::Union:
        case ShapeOperations::Difference:
        case ShapeOperations::Intersection:
        {
            nodeStack.push(node.left);
            nodeStack.push(node.right);
            break;
        }
        default:
            dbLogf("Invalid shape operation %d", node.operation);
            return false;
        }
    }

    // Evaluate the RPN input backwards, as it was built backwards.
    std::stack<bool> evalStack;

    for (auto i = rpnInput.rbegin(), end = rpnInput.rend(); i < end; ++i)
    {
        const CompositeNode& node = **i;

        switch (node.operation)
        {
        case ShapeOperations::Shape:
        {
            const ShapeUnion& shape = m_shapes[node.shape];

            switch (shape.shapeType)
            {
            case CSGShapes::Cuboid:
            {
                evalStack.push(shape.cuboid.Contains(point));
                break;
            }
            default:
                dbLogf("Invalid shape type %d", shape.shapeType);
                return false;
            }

            break;
        }
        case ShapeOperations::Union:
        {
            bool lhs = evalStack.top();
            evalStack.pop();

            bool rhs = evalStack.top();
            evalStack.pop();

            evalStack.push(lhs || rhs);
            break;
        }
        case ShapeOperations::Difference:
        {
            bool lhs = evalStack.top();
            evalStack.pop();

            bool rhs = evalStack.top();
            evalStack.pop();

            evalStack.push(lhs && !rhs); // TODO(jwerner) this could very well be wrong if lhs and rhs are mixed up.
            break;
        }
        case ShapeOperations::Intersection:
        {
            bool lhs = evalStack.top();
            evalStack.pop();

            bool rhs = evalStack.top();
            evalStack.pop();

            evalStack.push(lhs && rhs);
            break;
        }
        default:
            dbLogf("Invalid shape operation %d. This should have been caught much earlier.", node.operation);
            return false;
        }
    }

    // The last bool on the evaluation stack is the result.
    if (evalStack.size() != 1)
    {
        dbLogf("Invalid number of results on the stack: %d", evalStack.size());
        return false;
    }
    bool result = evalStack.top();
    evalStack.pop();
    return result;
}

double CompositeShape::CalcVolume() const
{
    // TODO(jwerner) This will require numerical integration. The result should probably be cached.
    return 0.0;
}
