﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolGeneral
{
    public string ToolName;
    public string ToolDescription;

	public TrackManager TrackManager;
	public TileManager TileManager;
	public TerrainManager TerrainManager;
	public Transform SomePrefab;

	public GameObject ModelPrefab;

	public virtual void Initialize()
	{
	}

	public virtual void OnSelected()
	{
	}

	public virtual void OnDeselected()
	{
	}

	public virtual void OnLMBUp(Vector3 point)
	{

	}

	public virtual void OnLMBDown(Vector3 point)
	{
	}

	public virtual void OnLMB(Vector3 point)
	{
	}

	public virtual void OnRMBDown(Vector3 point)
	{
	}

	public virtual void OnRMB(Vector3 point)
	{
	}

	public virtual void OnMapSizeChange()
	{

	}

	public virtual void OnMouseOverTile(IntVector2 point)
	{
	}

	public virtual void UpdateGUI(Rect guiRect)
	{
	}

	public virtual void Update()
	{
	}
}
