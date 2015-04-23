
#include "Cuboid.h"

CSGCuboid::CSGCuboid()
    : m_localToCompositeMatrix()
    , m_dimensions()
{
}

CSGCuboid::CSGCuboid(const Vector4& position, const Vector4& dimensions, const Quaternion& orientation)
    : m_localToCompositeMatrix(position, orientation)
    , m_dimensions()
{
    SetDimensions(dimensions);
}

CSGCuboid::CSGCuboid(const CSGCuboid& other)
    : m_localToCompositeMatrix(other.m_localToCompositeMatrix)
    , m_dimensions(other.m_dimensions)
{
}

CSGCuboid::CSGCuboid(CSGCuboid&& other)
    : m_localToCompositeMatrix(other.m_localToCompositeMatrix)
    , m_dimensions(other.m_dimensions)
{
}

bool CSGCuboid::Equals(const CSGCuboid& other) const
{
    Vector4 testPoint = Vector4(1.0, 1.0, 1.0, 1.0);

    Vector4 myPoint = (m_localToCompositeMatrix * testPoint) + m_dimensions;
    Vector4 otherPoint = (other.m_localToCompositeMatrix * testPoint) + other.m_dimensions;
    
    return myPoint.Equals(otherPoint); // TODO(jwerner) is this correct?
}

bool CSGCuboid::Contains(const Vector4& point) const
{
    Vector4 localPoint = m_localToCompositeMatrix.CalcInverseTransform() * point;
    
    Vector4 zero = Vector4();
    if ((localPoint.GreaterThan(zero) || localPoint.Equals(zero))
        && (localPoint.LessThan(m_dimensions) || localPoint.Equals(m_dimensions)))
    {
        return true;
    }
    return false;
}

double CSGCuboid::CalcVolume() const
{
    return (m_dimensions.x * m_dimensions.y * m_dimensions.z);
}

void CSGCuboid::operator=(const CSGCuboid& rhs)
{
    m_localToCompositeMatrix = rhs.m_localToCompositeMatrix;
    m_dimensions = rhs.m_dimensions;
}

void CSGCuboid::SetPosition(const Vector4& position)
{
    m_localToCompositeMatrix[3][0] = position.x;
    m_localToCompositeMatrix[3][1] = position.y;
    m_localToCompositeMatrix[3][2] = position.z;
    m_localToCompositeMatrix[3][3] = position.w;
}

void CSGCuboid::SetDimensions(const Vector4& dimensions)
{
    m_dimensions = dimensions;
}
