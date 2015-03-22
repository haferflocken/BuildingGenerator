using UnityEngine;
using System.Collections.Generic;
using Tuples;

public static class BuildingMeshGen
{
	// Create levels from the floor plan.
	public static List<Mesh> CreateBuilding(BuildingBlueprint floorPlan)
	{
		List<Mesh> levels = new List<Mesh>();

		foreach (BuildingBlueprint.Level levelPlan in floorPlan._levels)
		{
			Mesh level = CreateLevelMesh(levelPlan);
			levels.Add(level);
		}

		return levels;
	}

	private struct ExtrudedVertex
	{
		public Vector2 position;
		public int parentVertex;
		public int parentAdjacentVertex0;
		public int parentAdjacentVertex1;
	}

	private static Mesh CreateLevelMesh(BuildingBlueprint.Level levelPlan)
	{
		// Set up.
		float wallHeight = levelPlan._wallHeight;
		float wallThickness = levelPlan._wallThickness;
		float floorThickness = levelPlan._floorThickness;

		// Eliminate duplicate vertexes, split edges with vertexes in them, etc.
		List<Vector2> vertexes = new List<Vector2>(); // Each is a point (x, z)
		List<IntTuple2> edges = new List<IntTuple2>(); // Each is an edge (index1, index2) that indexes into vertexes.

		foreach (BuildingBlueprint.Wall wallPlan in levelPlan._walls)
		{
			for (int i = 1; i < wallPlan._points.Count; ++i)
			{
				IntTuple2 w1 = wallPlan._points[i - 1];
				IntTuple2 w2 = wallPlan._points[i];
				
				// Add the vertexes if we don't already have them.
				Vector2 p1 = new Vector2(w1.e0, w1.e1);
				Vector2 p2 = new Vector2(w2.e0, w2.e1);

				int p1Index = vertexes.FindIndex(v => v.Approximately(p1));
				bool p1IsNew = false;
				if (p1Index == -1)
				{
					p1Index = vertexes.Count;
					vertexes.Add(p1);
					p1IsNew = true;
				}

				int p2Index = vertexes.FindIndex(v => v.Approximately(p2));
				bool p2IsNew = false;
				if (p2Index == -1)
				{
					p2Index = vertexes.Count;
					vertexes.Add(p2);
					p2IsNew = true;
				}

				// DEBUG: Detect invalid walls.
				if (p1Index != -1 && p2Index != -1 && p1Index == p2Index)
				{
					Debug.LogWarning("Invalid wall.");
				}

				// Split any edges p1 and p2 are on.
				if (p1IsNew)
				{
					SplitEdgesWithPoint(vertexes, edges, p1Index);
				}
				if (p2IsNew)
				{
					SplitEdgesWithPoint(vertexes, edges, p2Index);
				}

				// Don't add the edge if we already have it.
				IntTuple2 curEdge = new IntTuple2(p1Index, p2Index);
				if (edges.IndexOf(curEdge) == -1)
				{
					// Resolve intersections by splitting the two edges at their midpoint.
					SplitEdgesWithEdge(vertexes, edges, curEdge);
				}
			}
		}

		// DEBUG: Detect dupliacte vertexes.
		vertexes.DetectDuplicates("Detected duplicate vertex.");

		// DEBUG: Detect duplicate edges.
		edges.DetectDuplicates((left, right) => (left.e0 == right.e0 && left.e1 == right.e1) || (left.e1 == right.e0 && left.e0 == right.e1), "Detected duplicate edge.");

		// DEBUG: Detect invalid edges.
		foreach (IntTuple2 edge in edges)
		{
			if (edge.e0 == edge.e1)
			{
				Debug.LogWarning("Detected invalid edge.");
			}
		}

		// DEBUG: Log the edges.
		foreach (IntTuple2 edge in edges)
		{
			Vector2 a = vertexes[edge.e0];
			Vector2 b = vertexes[edge.e1];

			Debug.Log("[" + a + ", " + b + "]");
		}

		// Map the vertexes to the edges that contain them and the angle of each edge around the vertex.
		List<GenTuple2<int, float>>[] vertsToEdges = new List<GenTuple2<int, float>>[vertexes.Count];
		for (int i = 0; i < vertsToEdges.Length; ++i)
		{
			vertsToEdges[i] = new List<GenTuple2<int, float>>();
		}
		for (int i = 0; i < edges.Count; ++i)
		{
			IntTuple2 edge = edges[i];
			vertsToEdges[edge.e0].Add(new GenTuple2<int, float>(i, 0.0f));
			vertsToEdges[edge.e1].Add(new GenTuple2<int, float>(i, 0.0f));
		}

		// Sort the edges in vertsToEdges by angle around the each vertex.
		for (int i = 0; i < vertsToEdges.Length; ++i)
		{
			Vector2 vertex = vertexes[i];

			List<GenTuple2<int, float>> containingEdges = vertsToEdges[i];
			foreach (GenTuple2<int, float> edgeAndAngle in containingEdges)
			{
				IntTuple2 edge = edges[edgeAndAngle.e0];
				int otherVertIndex = (edge.e0 == i) ? edge.e1 : edge.e0;
				Vector2 realEdge = vertexes[otherVertIndex] - vertex;
				float angle = MathUtil.CounterClockwiseAngle(realEdge.y, realEdge.x);
				edgeAndAngle.e1 = angle;
			}

			containingEdges.Sort((left, right) => left.e1.CompareTo(right.e1));
		}

		// Extrude the vertexes into the wall cross-section.
		List<ExtrudedVertex> extrudedVertexes = new List<ExtrudedVertex>();
		for (int i = 0; i < vertexes.Count; ++i)
		{
			Vector2 vertex = vertexes[i];
			List<GenTuple2<int, float>> containingEdges = vertsToEdges[i];
			Debug.Log("Extruding vertex " + vertex + " between " + containingEdges.Count + " edges.");
			
			if (containingEdges.Count > 1)
			{
				for (int j = 0, k = containingEdges.Count - 1; j < containingEdges.Count; k = j, ++j)
				{
					GenTuple2<int, float> edgeAndAngleJ = containingEdges[j];
					IntTuple2 edgeJ = edges[edgeAndAngleJ.e0];
					float edgeJAngle = edgeAndAngleJ.e1;

					GenTuple2<int, float> edgeAndAngleK = containingEdges[k];
					IntTuple2 edgeK = edges[edgeAndAngleK.e0];
					float edgeKAngle = edgeAndAngleK.e1;

					if (edgeJAngle < edgeKAngle)
					{
						edgeJAngle += MathUtil.TWOPI;
					}

					float offsetAngle = (edgeJAngle + edgeKAngle) * 0.5f;
					if (offsetAngle > MathUtil.TWOPI)
					{
						offsetAngle -= MathUtil.TWOPI;
					}

					Vector2 offset;
					offset.x = Mathf.Cos(offsetAngle);
					offset.y = Mathf.Sin(offsetAngle);

					float interiorAngle = Mathf.Abs(edgeJAngle - edgeKAngle);
					float extrusionAmount = (wallThickness * 0.5f) / Mathf.Sin(interiorAngle * 0.5f);
					offset *= extrusionAmount;

					ExtrudedVertex newVert;
					newVert.position = vertex + offset;
					newVert.parentVertex = i;
					newVert.parentAdjacentVertex0 = (edgeJ.e1 == i) ? edgeJ.e0 : edgeJ.e1;
					newVert.parentAdjacentVertex1 = (edgeK.e1 == i) ? edgeK.e1 : edgeK.e0;

					extrudedVertexes.Add(newVert);
				}
			}
			else if (containingEdges.Count == 1)
			{
				GenTuple2<int, float> edgeAndAngle = containingEdges[0];
				IntTuple2 containingEdge = edges[edgeAndAngle.e0];
				Vector3 edgeDirection;
				edgeDirection.x = Mathf.Cos(edgeAndAngle.e1);
				edgeDirection.y = 0.0f;
				edgeDirection.z = Mathf.Sin(edgeAndAngle.e1);

				Vector2 offset = (Quaternion.Euler(0.0f, 90.0f, 0.0f) * edgeDirection).SwizzleXZ();
				offset *= wallThickness * 0.5f;

				ExtrudedVertex newVert0;
				newVert0.position = vertex + offset;
				newVert0.parentVertex = i;
				newVert0.parentAdjacentVertex0 = (containingEdge.e0 == i) ? containingEdge.e1 : containingEdge.e0;
				newVert0.parentAdjacentVertex1 = -1;

				ExtrudedVertex newVert1 = newVert0;
				newVert1.position = vertex - offset;
				newVert1.parentAdjacentVertex0 = i; // Cap off ends.
				newVert1.parentAdjacentVertex1 = newVert0.parentAdjacentVertex0;

				extrudedVertexes.Add(newVert0);
				extrudedVertexes.Add(newVert1);
			}
		}
		
		// Find the edges between the extruded vertexes.
		List<IntTuple2> extrudedEdges = new List<IntTuple2>();
		for (int i = 0; i < extrudedVertexes.Count; ++i)
		{
			ExtrudedVertex eVertex = extrudedVertexes[i];
			Vector2 parentVertex = vertexes[eVertex.parentVertex];

			if (eVertex.parentAdjacentVertex0 != -1)
			{
				Vector2 parentAdjacentVertex0 = vertexes[eVertex.parentAdjacentVertex0];
				Vector2 parentAdjacentEdgeDir = (parentAdjacentVertex0 - parentVertex).normalized;

				List<int> potentialEdgeMates = extrudedVertexes.FindAllIndexes(v => v.parentVertex == eVertex.parentAdjacentVertex0);
				potentialEdgeMates.Sort((left, right) => 
				{
					ExtrudedVertex leftVert = extrudedVertexes[left];
					ExtrudedVertex rightVert = extrudedVertexes[right];
					Vector2 leftEdgeDir = (leftVert.position - eVertex.position).normalized;
					Vector2 rightEdgeDir = (rightVert.position - eVertex.position).normalized;
					float leftDot = Vector2.Dot(leftEdgeDir, parentAdjacentEdgeDir);
					float rightDot = Vector2.Dot(rightEdgeDir, parentAdjacentEdgeDir);
					return leftDot.CompareTo(rightDot);
				});

				int edgeMateIndex = potentialEdgeMates[potentialEdgeMates.Count - 1];
				ExtrudedVertex edgeMate = extrudedVertexes[edgeMateIndex];
				if (edgeMate.parentAdjacentVertex0 == eVertex.parentVertex)
				{
					edgeMate.parentAdjacentVertex0 = -1;
				}
				else
				{
					edgeMate.parentAdjacentVertex1 = -1;
				}
				extrudedVertexes[edgeMateIndex] = edgeMate;

				IntTuple2 newEdge = new IntTuple2(i, edgeMateIndex);
				extrudedEdges.Add(newEdge);
			}
			// Each vertex is only responsible for one edge, hence the "else".
			else if (eVertex.parentAdjacentVertex1 != -1)
			{
				Vector2 parentAdjacentVertex1 = vertexes[eVertex.parentAdjacentVertex1];
				Vector2 parentAdjacentEdgeDir = (parentAdjacentVertex1 - parentVertex).normalized;

				List<int> potentialEdgeMates = extrudedVertexes.FindAllIndexes(v => v.parentVertex == eVertex.parentAdjacentVertex1);
				potentialEdgeMates.Sort((left, right) =>
				{
					ExtrudedVertex leftVert = extrudedVertexes[left];
					ExtrudedVertex rightVert = extrudedVertexes[right];
					Vector2 leftEdgeDir = (leftVert.position - eVertex.position).normalized;
					Vector2 rightEdgeDir = (rightVert.position - eVertex.position).normalized;
					float leftDot = Vector2.Dot(leftEdgeDir, parentAdjacentEdgeDir);
					float rightDot = Vector2.Dot(rightEdgeDir, parentAdjacentEdgeDir);
					return leftDot.CompareTo(rightDot);
				});

				int edgeMateIndex = potentialEdgeMates[potentialEdgeMates.Count - 1];
				ExtrudedVertex edgeMate = extrudedVertexes[edgeMateIndex];
				if (edgeMate.parentAdjacentVertex0 == eVertex.parentVertex)
				{
					edgeMate.parentAdjacentVertex0 = -1;
				}
				else
				{
					edgeMate.parentAdjacentVertex1 = -1;
				}
				extrudedVertexes[edgeMateIndex] = edgeMate;

				IntTuple2 newEdge = new IntTuple2(edgeMateIndex, i);
				extrudedEdges.Add(newEdge);
			}
		}

		// DEBUG: Detect duplicate extruded edges.
		extrudedEdges.DetectDuplicates((left, right) => (left.e0 == right.e0 && left.e1 == right.e1) || (left.e1 == right.e0 && left.e0 == right.e1), "Detected duplicate extruded edge.");

		// DEBUG: Log the extruded extrudedEdges.
		foreach (IntTuple2 edge in extrudedEdges)
		{
			Vector2 a = extrudedVertexes[edge.e0].position;
			Vector2 b = extrudedVertexes[edge.e1].position;

			Debug.Log("[" + a + ", " + b + "]");
		}

		// Use the extruded edges to make the wall mesh.
		List<Vector3> meshVerts = new List<Vector3>();
		List<Vector2> meshUVs = new List<Vector2>();
		List<int> meshTris = new List<int>();

		foreach (IntTuple2 edge in extrudedEdges)
		{
			Vector3 a = extrudedVertexes[edge.e0].position.SwizzleX0Y();
			Vector3 b = extrudedVertexes[edge.e1].position.SwizzleX0Y();

			Vector3 c = a;
			c.y = wallHeight;
			Vector3 d = b;
			d.y = wallHeight;

			int iA = meshVerts.Count;
			meshVerts.Add(a);
			meshVerts.Add(b);
			meshVerts.Add(c);
			meshVerts.Add(d);

			meshUVs.AddRange(new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) });

			meshTris.AddRange(new int[] { iA, iA + 2, iA + 1 }); // A C B
			meshTris.AddRange(new int[] { iA + 2, iA + 3, iA + 1 }); // C D B
		}

		// Create the floor mesh by computing a constrained Delaunay triangulation of each floor polygon.
		foreach (BuildingBlueprint.Floor floorSection in levelPlan._floors)
		{
			// TODO(jwerner)
		}

		Mesh levelMesh = new Mesh();
		levelMesh.vertices = meshVerts.ToArray();
		levelMesh.uv = meshUVs.ToArray();
		levelMesh.triangles = meshTris.ToArray();
		levelMesh.RecalculateNormals();

		return levelMesh;
	}

	// Split any edges that intersect with the given edge at their midpoint.
	private static void SplitEdgesWithEdge(List<Vector2> vertexes, List<IntTuple2> edges, IntTuple2 splitterEdge)
	{
		// Gather edge split data. These are tuples in the form (splitter vertex index, split edge index).
		List<IntTuple2> edgeSplitData = new List<IntTuple2>();
		HashSet<int> splitEdgeVertsSet = new HashSet<int>();
		splitEdgeVertsSet.Add(splitterEdge.e0);
		splitEdgeVertsSet.Add(splitterEdge.e1);
		for (int i = 0; i < edges.Count; ++i)
		{
			IntTuple2 curEdge = edges[i];
			int intersectionIndex = MakeIntersection(vertexes, splitterEdge, curEdge);

			// If the current edge had a vertex on the splitter edge, split the splitter edge but leave the current egde alone.
			if (curEdge.e0 == intersectionIndex || curEdge.e1 == intersectionIndex)
			{
				Debug.Log("Will split edge [" + vertexes[splitterEdge.e0] + ", " + vertexes[splitterEdge.e1] + "] at " + vertexes[intersectionIndex]);
				splitEdgeVertsSet.Add(intersectionIndex);
			}
			// If the splitter edge had a vertex on the current edge, split the current edge but leave the splitter edge alone.
			else if (splitterEdge.e0 == intersectionIndex || splitterEdge.e1 == intersectionIndex)
			{
				Debug.Log("Will split edge [" + vertexes[curEdge.e0] + ", " + vertexes[curEdge.e1] + "] at " + vertexes[intersectionIndex]);
				edgeSplitData.Add(new IntTuple2(intersectionIndex, i));
			}
			// If the edges met in the middle, split them both.
			else if (intersectionIndex != -1)
			{
				Debug.Log("Will split edge [" + vertexes[splitterEdge.e0] + ", " + vertexes[splitterEdge.e1] + "] at " + vertexes[intersectionIndex]);
				Debug.Log("Will split edge [" + vertexes[curEdge.e0] + ", " + vertexes[curEdge.e1] + "] at " + vertexes[intersectionIndex]);
				splitEdgeVertsSet.Add(intersectionIndex);
				edgeSplitData.Add(new IntTuple2(intersectionIndex, i));
			}
		}

		// DEBUG: Log stuff to figure out why splitting doesn't work as expected.
		/*if (edgeSplitData.Count > 0)
		{
			string debugText = "Edge split data:\n";
			foreach (IntTuple2 splitData in edgeSplitData)
			{
				int vertexIndex = splitData.e0;
				int edgeIndex = splitData.e1;

				Vector2 vertex = vertexes[vertexIndex];
				IntTuple2 edge = edges[edgeIndex];

				debugText += ("[" + vertexes[edge.e0] + ", " + vertexes[edge.e1] + "] was split at " + vertex + "\n");
			}
			Debug.Log(debugText);
		}

		if (splitEdgeVertsSet.Count > 0)
		{
			string debugText = "Split edge verts set:\n";
			foreach (int vert in splitEdgeVertsSet)
			{
				debugText += vertexes[vert];
			}
			Debug.Log(debugText);
		}*/

		// Split the edges.
		foreach (IntTuple2 splitData in edgeSplitData)
		{
			int intersectionIndex = splitData.e0;
			int edgeIndex = splitData.e1;

			IntTuple2 edge = edges[edgeIndex];
			
			// Split the old edge in two.
			int oldEnd = edge.e1;
			edge.e1 = intersectionIndex;

			IntTuple2 newEdge = new IntTuple2(intersectionIndex, oldEnd);
			if (!edges.Contains(newEdge))
			{
				edges.Add(newEdge);
			}

			Debug.Log("Edge split into " + vertexes[edge.e0] + ", " + vertexes[intersectionIndex] + ", " + vertexes[newEdge.e1]);
		}

		// Make the new, split up edge to replace the old splitter edge from the vertexes.
		int[] splitEdgeVerts = new int[splitEdgeVertsSet.Count];
		splitEdgeVertsSet.CopyTo(splitEdgeVerts);
		Vector2 mostNegative = vertexes[splitEdgeVertsSet.MinBy(v => vertexes[v].x + vertexes[v].y)];
		System.Array.Sort<int>(splitEdgeVerts, (a, b) =>
			{
				Vector2 vA = vertexes[a] - mostNegative;
				Vector2 vB = vertexes[b] - mostNegative;
				return vA.sqrMagnitude.CompareTo(vB.sqrMagnitude);
			}
		);

		// DEBUG: Log the split edge verts.
		string debugText = "Split edge verts: ";
		foreach (int vertexIndex in splitEdgeVerts)
		{
			debugText += vertexes[vertexIndex] + ", ";
		}
		Debug.Log(debugText);

		// Create the new edges from the splitter edge data.
		for (int i = 1; i < splitEdgeVerts.Length; ++i)
		{
			int vA = splitEdgeVerts[i - 1];
			int vB = splitEdgeVerts[i];
			IntTuple2 newEdge = new IntTuple2(vA, vB);
			if (!edges.Contains(newEdge))
			{
				edges.Add(newEdge);
			}
		}
	}

	// Split any edges the given point lies on.
	private static void SplitEdgesWithPoint(List<Vector2> vertexes, List<IntTuple2> edges, int pointIndex)
	{
		Vector2 point = vertexes[pointIndex];
		List<IntTuple2> newEdges = new List<IntTuple2>();
		foreach (IntTuple2 edge in edges)
		{
			if (IsOnLine(vertexes[edge.e0], vertexes[edge.e1], point))
			{
				int endIndex = edge.e1;
				edge.e1 = pointIndex;
				IntTuple2 newEdge = new IntTuple2(pointIndex, endIndex);
				newEdges.Add(newEdge);
			}
		}
		edges.AddRange(newEdges);
	}

	private static int MakeIntersection(List<Vector2> vertexes, IntTuple2 line1, IntTuple2 line2)
	{
		Vector2 p1 = vertexes[line1.e0];
		Vector2 q1 = vertexes[line1.e1];
		Vector2 p2 = vertexes[line2.e0];
		Vector2 q2 = vertexes[line2.e1];

		// Lines with shared endpoints do not intersect.
		if (p1.Approximately(ref p2) || p1.Approximately(ref q2) || q1.Approximately(ref p2) || q1.Approximately(ref q2))
		{
			return -1;
		}

		// Parallel lines do not intersect.
		float slope1 = GetSlope(p1, q1);
		float slope2 = GetSlope(p2, q2);
		if (Mathf.Approximately(slope1, slope2))
		{
			return -1;
		}

		// Check if the lines end on each other.
		/*if (IsOnLine(vertexes, line1, line2.e0))
		{
			return line2.e0;
		}
		if (IsOnLine(vertexes, line1, line2.e1))
		{
			return line2.e1;
		}
		if (IsOnLine(vertexes, line2, line1.e0))
		{
			return line1.e0;
		}
		if (IsOnLine(vertexes, line2, line1.e1))
		{
			return line1.e1;
		}*/

		// Calculate the intersection.
		float yInt1 = p1.y - slope1 * p1.x;
		float yInt2 = p2.y - slope2 * p2.x;

		float intersectionX = (yInt2 - yInt1) / (slope1 - slope2);
		float intersectionY = (slope1 * yInt2 - slope2 * yInt1) / (slope1 - slope2);

		// Check to see that the intersection point is in the bounding box.
		Vector2[] boundPoints = new Vector2[] { q1, p2, q2 };
		float top = p1.y;
		float bottom = p1.y;
		float left = p1.x;
		float right = p1.x;
		foreach (Vector2 point in boundPoints)
		{
			if (point.y < top)
			{
				top = point.y;
			}
			else if (point.y > bottom)
			{
				bottom = point.y;
			}
			if (point.x < left)
			{
				left = point.x;
			}
			else if (point.x > right)
			{
				right = point.x;
			}
		}

		if (intersectionX <= left || intersectionX >= right || intersectionY <= top || intersectionY >= bottom)
		{
			return -1;
		}

		// The intersection is valid. See if we already have the vertex, or add it if we don't.
		Vector2 intersection = new Vector2(intersectionX, intersectionY);
		int existingIndex = vertexes.FindIndex(v => v.Approximately(intersection));
		if (existingIndex != -1)
		{
			return existingIndex;
		}

		if (intersection.Approximately(ref p1) || intersection.Approximately(ref p2) || intersection.Approximately(ref q1) || intersection.Approximately(ref q2))
		{
			Debug.LogWarning("Created invalid vertex.");
		}
		vertexes.Add(intersection);
		return vertexes.Count - 1;
	}

	// Check if a point is on a line. Does not count endpoints.
	private static bool IsOnLine(Vector2 start, Vector2 end, Vector2 point)
	{
		// Don't count endpoints.
		if (start == point || end == point)
		{
			return false;
		}

		// Check the bounding box.
		float top = end.y;
		float bottom = start.y;
		float left = end.x;
		float right = start.x;
		if (start.x < end.x)
		{
			left = start.x;
			right = end.x;
		}
		if (start.y < end.y)
		{
			top = start.y;
			bottom = end.y;
		}

		if (point.x < left || point.x > right || point.y < top || point.y > bottom)
		{
			return false;
		}

		// Check that we are on the line.
		float slope = (start.y - end.y) / (start.x - end.x);
		float targetY = slope * (point.x  - start.x) + start.y;
		return Mathf.Approximately(targetY, point.y);
	}

	private static bool IsOnLine(List<Vector2> vertexes, IntTuple2 line, int point)
	{
		return IsOnLine(vertexes[line.e0], vertexes[line.e1], vertexes[point]);
	}

	private static float GetSlope(Vector2 start, Vector2 end)
	{
		if (Mathf.Approximately(start.x, end.x))
		{
			return float.MaxValue;
		}
		return (start.y - end.y) / (start.x - end.x);
	}
}