using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class GenerateBuildingMeshWizard : ScriptableWizard
{
	private static string OUTPUT_PATH = "Assets/BuildingBlueprints/GeneratedMeshes/";

	[MenuItem("Building Generator/Generate Building Mesh")]
	public static void ShowWizard()
	{
		ScriptableWizard.DisplayWizard<GenerateBuildingMeshWizard>("Generate Building Mesh", "Generate");
	}

	// Wizard fields.
	public BuildingBlueprint _blueprint;

	void OnWizardCreate()
	{
		if (_blueprint == null)
		{
			return;
		}

		string floorPlanName = AssetDatabase.GetAssetPath(_blueprint);
		if (floorPlanName == "")
		{
			floorPlanName = "unnamed_floor_plan";
		}
		else
		{
			floorPlanName = floorPlanName.Substring(floorPlanName.LastIndexOf('/') + 1);
			floorPlanName = Path.GetFileNameWithoutExtension(floorPlanName);
		}

		List<Mesh> buildingLevels = BuildingMeshGen.CreateBuilding(_blueprint);

		for (int i = 0; i < buildingLevels.Count; ++i)
		{
			string assetPathName = AssetDatabase.GenerateUniqueAssetPath(OUTPUT_PATH + floorPlanName + " " + i + ".asset");
			AssetDatabase.CreateAsset(buildingLevels[i], assetPathName);
		}
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		EditorUtility.FocusProjectWindow();
		Selection.activeObject = buildingLevels[0];
	}
}
