
#include "CompositeShapeManager.h"

CompositeShapeManager CompositeShapeManager::s_Instance = CompositeShapeManager();

bool CompositeShapeManager::CompositeContains(CompositeShapeID id, const Vector4& position) const
{
    return m_shapes[id].Contains(position);
}
