using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PowerUI;

public class UIManager : MonoBehaviour 
{
	private string CrashdayPath = "";

	void Start()
	{
		var document=UI.document;
		
		var ui_loadMapButton = document.getElementById("loadMapButton");
		var ui_saveMapButton = document.getElementById("saveMapButton");

		ui_loadMapButton.onmousedown = UI_LoadMap;
		ui_saveMapButton.onmousedown = UI_SaveMap;
	}

	public void UI_LoadMap(MouseEvent mouseEvent)
	{
		string path = EditorUtility.OpenFilePanel("Open trk file", CrashdayPath + "/user/", "trk");
		if (path.Length != 0)
		{
			PlayerPrefs.SetString("lastmappath", path);
			MapParser mapParser = new MapParser();
			TrackSavable Track = mapParser.ReadMap(path);
			GetComponent<TrackManager>().LoadTrack(Track);
		}
	}

	public void UI_SaveMap(MouseEvent mouseEvent)
	{
		string path = EditorUtility.SaveFilePanel("Save trk file", CrashdayPath + "/user/", "my_awesome_track", "trk");
		MapParser mapParser = new MapParser();
		mapParser.SaveMap(GetComponent<TrackManager>().CurrentTrack, path);
	}
}
