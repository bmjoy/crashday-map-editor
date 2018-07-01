﻿using UnityEngine;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;

public class TrackManager : MonoBehaviour
{
    public GameObject Dummy;
    public Transform Map;
    public Transform[,] Tiles;
	public TrackSavable CurrentTrack;

    public static int TileSize = 20;
	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void LoadTrack(TrackSavable track)
    {
	    CurrentTrack = track;
        for (int i = 0; i < Map.childCount; i++)
        {
            Destroy(Map.GetChild(i).gameObject);
        }

		GetComponent<TerrainManager>().GenerateTerrain();
			
        for (int y = 0; y < track.Height; y++)
        {
            for (int x = 0; x < track.Width; x++)
            {
                if (track.TrackTiles[x, y].FieldId < track.FieldFilesNumber)
                {
					TileManager tileManager = GetComponent<TileManager> ();

					int index = tileManager.tileNames.IndexOf (track.FieldFiles[track.TrackTiles [x, y].FieldId]);

	                if (index < 0 || index >= tileManager.tileModels.Count) continue;

                    string pathToCfl = IO.GetCrashdayPath() + "/data/content/tiles/" + track.FieldFiles[track.TrackTiles[x, y].FieldId];
                    string[] cflFIle = System.IO.File.ReadAllLines(pathToCfl);
					
					//The tile will be moved by the SetTile function later. The best moment to calcualte height is now.
					GameObject newTile = (GameObject) Instantiate(Dummy, new Vector3(0, tileManager.tileModels[index].P3DMeshes[0].Height/2, 0), Quaternion.identity);

                    //get the size of the model in tiles
                    string sizeStr = cflFIle[3];
	                sizeStr = sizeStr.Remove(sizeStr.IndexOf("#")).Trim();
	                sizeStr = sizeStr.Replace(" ", string.Empty);
					IntVector2 size = new IntVector2(sizeStr[0]-'0', sizeStr[1]-'0');

					//get the name of the model
	                string name = cflFIle[2];
	                name = name.Remove(name.IndexOf(".p3d")).Trim();

                    newTile.name = x + ":" + y + " " + name;

					newTile.GetComponent<MeshFilter>().mesh = tileManager.tileModels[index].CreateMeshes()[0];
	                newTile.GetComponent<Renderer>().materials = tileManager.tileMaterials[index];
					
	                newTile.transform.SetParent(Map);

	                Tile tile = newTile.AddComponent<Tile>();
					tile.SetupTile(track.TrackTiles [x, y], size, new Vector2(x, y), this);
					tile.ApplyTerrain();
                }
            }
        }
    }
}
