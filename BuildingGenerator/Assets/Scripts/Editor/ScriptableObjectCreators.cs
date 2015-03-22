using UnityEngine;
using UnityEditor;
using System.IO;

public static class ScriptableObjectCreators
{
	public static void CreateAsset<T>() where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T>();

		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		if (path == "")
		{
			path = "Assets";
		}
		else if (Path.GetExtension(path) != "")
		{
			path = path.Replace(Path.GetFileName(path), "");
		}

		string assetPathName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");
		AssetDatabase.CreateAsset(asset, assetPathName);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;
	}

	[MenuItem("Building Generator/Create Building Blueprint")]
	public static void CreateBuildingBlueprint()
	{
		CreateAsset<BuildingBlueprint>();
	}
}
