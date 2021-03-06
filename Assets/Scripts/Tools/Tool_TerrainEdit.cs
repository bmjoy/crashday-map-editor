﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Tool_TerrainEdit : ToolGeneral
{
	private List<float> _currentSelectedPoints;
	private List<IntVector2> _currentSelectedTiles;

	private ComputeBuffer _buf;

	public float _oldHeight;
	public float _startHeight;

	public override void Initialize()
	{
		ToolName = "Edit Terrain";
		_currentSelectedPoints = new List<float>();
		_currentSelectedTiles = new List<IntVector2>();
	}

	public override void OnMapSizeChange()
	{
		OnSelected();
	}

	public override void OnSelected()
	{
		_currentSelectedPoints.Clear();
		_currentSelectedTiles.Clear();
		UpdateTerrainShader();
		TerrainManager.Terrain.GetComponent<MeshRenderer>().enabled = true;
	}

	public override void OnDeselected()
	{
		TerrainManager.Terrain.GetComponent<MeshRenderer>().enabled = false;
		_buf.Dispose();
	}

	public override void Update()
	{
		if (Input.GetButtonDown("Select all"))
		{
			if (_currentSelectedPoints.Count > 0)
			{
				_currentSelectedPoints.Clear();
				_currentSelectedTiles.Clear();
			}
			else
			{
				for (int y = 0; y < TrackManager.CurrentTrack.Height * 4 + 1; y++)
				{
					for (int x = 0; x < TrackManager.CurrentTrack.Width * 4 + 1; x++)
					{
						_currentSelectedPoints.Add(GetPoint(new IntVector2(x, y)));
					}
				}

				for (int y = 0; y < TrackManager.CurrentTrack.Height; y++)
					for(int x = 0; x < TrackManager.CurrentTrack.Width; x++)
						_currentSelectedTiles.Add(new IntVector2(x,y));
			}

			UpdateTerrainShader();
		}
	}

	public override void OnLMBUp(Vector3 point)
	{
		TerrainManager.UpdateCollider();
	}

	public override void OnLMBDown(Vector3 point)
	{
		_startHeight = Input.mousePosition.y*5/Screen.height;
		_oldHeight = _startHeight;
	}

	public override void OnLMB(Vector3 point)
	{
		//calcualte how high we lifted the mouse
		float delta = Input.mousePosition.y*5/Screen.height - _oldHeight;

		//for every selected point, change Track's heightmap, 
		//update terrain in the current point and move the heightpoint object
		foreach (var hp in _currentSelectedPoints)
		{
			IntVector2 p = GetPoint(hp);
			TrackManager.CurrentTrack.Heightmap[p.y][p.x] += delta;
			TerrainManager.UpdateTerrain(p);
		}

		//update terrain's mesh to show the changes
		TerrainManager.PushTerrainChanges();

		//update wireframe shader
		UpdateTerrainShader();

		//also update every tile which is affected
		foreach (var st in _currentSelectedTiles)
		{
			TrackManager.UpdateTerrainAt(st);
		}

		_oldHeight = Input.mousePosition.y*5/Screen.height;
	}

	public override void OnRMB(Vector3 pos)
	{
		pos.x += TrackManager.TileSize / 2;
		pos.z -= TrackManager.TileSize / 2;

		IntVector2 gridPosition = new IntVector2(Mathf.Clamp(Mathf.RoundToInt(pos.x / 5), 0, TrackManager.CurrentTrack.Width*4), 
			Mathf.Clamp(-1*Mathf.RoundToInt(pos.z / 5), 0, TrackManager.CurrentTrack.Height*4));

		float p = GetPoint(gridPosition);

		//if we are in remove selection mode
		if (Input.GetButton("Control"))
		{
			//check if we are pressing on selected point and remove it if so
			int res = _currentSelectedPoints.FindIndex(x => Mathf.Abs(x-p) < 0.01);
			if (res >= 0)
			{
				_currentSelectedPoints.RemoveAt(res);
			}
		}
		else
		{
			//if current point is not selected
			//(the check is needed to avoid selecting one point multiple times)
			if (!_currentSelectedPoints.Exists(x => Mathf.Abs(x-p) < 0.01))
			{
				_currentSelectedPoints.Add(GetPoint(gridPosition));

				int ax = 0;
				int ay = 0;

				//also select the tile on the current height point and add it
				IntVector2 tilePos = new IntVector2(gridPosition.x, gridPosition.y);

				if (gridPosition.x % 4 == 0 && gridPosition.x > 0)
				{
					tilePos.x -= 1;
					if(gridPosition.x != TrackManager.CurrentTrack.Width*4)
						ax += 1;
				}

				if (gridPosition.y % 4 == 0 && gridPosition.y > 0)
				{
					tilePos.y -= 1;
					if(gridPosition.y != TrackManager.CurrentTrack.Height*4)
						ay += 1;
				}

				for(int y = 0; y <= ay; y++)
					for(int x = 0; x <= ax; x++)
						_currentSelectedTiles.Add(new IntVector2(tilePos.x/4 + x, tilePos.y/4 + y));
			}
		}

		UpdateTerrainShader();
	}

	public override void UpdateGUI(Rect guiRect)
	{

	}

	private IntVector2 GetPoint(float p)
	{
		return new IntVector2(((int)p)%(TrackManager.CurrentTrack.Width*4+1), ((int)p)/(TrackManager.CurrentTrack.Width*4+1));
	}

	private float GetPoint(IntVector2 p)
	{
		return p.x + p.y * (TrackManager.CurrentTrack.Width * 4 + 1);
	}

	private void UpdateTerrainShader()
	{
		if(_buf != null)
			_buf.Dispose();

		//prevent creation of the zero sized compute buffer
		if (_currentSelectedPoints.Count == 0)
		{
			_buf = new ComputeBuffer(1, sizeof(float), ComputeBufferType.Default);
			float[] ar = {-1.0f};
			_buf.SetData(ar);
		}
		else
		{
			_buf = new ComputeBuffer(_currentSelectedPoints.Count, sizeof(float), ComputeBufferType.Default);
			_buf.SetData(_currentSelectedPoints);
		}

		Shader.SetGlobalBuffer("_Points", _buf);
		Shader.SetGlobalInt("_Points_Length", _buf.count);
	}
}