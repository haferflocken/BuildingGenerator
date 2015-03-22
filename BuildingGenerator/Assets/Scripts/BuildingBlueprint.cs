using UnityEngine;
using System.Collections.Generic;
using Tuples;

public class BuildingBlueprint : ScriptableObject
{
	[System.Serializable]
	public class Wall
	{
		public List<IntTuple2> _points;
	}

	[System.Serializable]
	public class Floor
	{
		public List<IntTuple2> _points;
	}

	[System.Serializable]
	public class Level
	{
		public float _wallHeight;
		public float _wallThickness;
		public float _floorThickness;
		public List<Wall> _walls;
		public List<Floor> _floors;
	}

	public List<Level> _levels;
}
