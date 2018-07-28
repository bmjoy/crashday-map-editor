﻿using System.IO;
using SFB;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public TrackSavable Track;
    public P3DModel Model;
    public string CrashdayPath = "";

	public GUISkin Skin;

	void Start ()
	{
	    CrashdayPath = IO.GetCrashdayPath();
		GetComponent<TileManager>().LoadTiles();
		GetComponent<TrackManager>().LoadTrack();
	}

    void OnGUI()
    {
	    //GUI.skin = Skin;

        if (GUI.Button(new Rect(5, 5, 160, 35), "Load map"))
        {
            string path = StandaloneFileBrowser.OpenFilePanel("Open trk file", CrashdayPath + "/user/", "trk", false)[0];
            if (path.Length != 0)
            {
				PlayerPrefs.SetString("lastmappath", path);
				MapParser mapParser = new MapParser();
                Track = mapParser.ReadMap(path);
                GetComponent<TrackManager>().LoadTrack(Track);
            }
        }

	    /*if (PlayerPrefs.HasKey("lastmappath"))
	    {
		    string LastMapPath = PlayerPrefs.GetString("lastmappath");
		    if (File.Exists(LastMapPath))
		    {
			    if (GUI.Button(new Rect(115, 5, 100, 30), "Load last map"))
			    {
				    MapParser mapParser = new MapParser();
				    Track = mapParser.ReadMap(LastMapPath);
				    GetComponent<TrackManager>().LoadTrack(Track);
			    }
		    }
	    }*/

	    if (GUI.Button(new Rect(175, 5, 160, 35), "Save map"))
	    {
		    string path = StandaloneFileBrowser.SaveFilePanel("Save trk file", CrashdayPath + "/user/", "my_awesome_track", "trk");
			MapParser mapParser = new MapParser();
			mapParser.SaveMap(GetComponent<TrackManager>().CurrentTrack, path);
	    }
    }

}
