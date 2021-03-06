﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TrackTileSavable _trackTileSavable;
	public string FieldName;

	public IntVector2 Size;
	private TerrainManager _terrainManager;
	private Vector3[] _originalVertices;
	private Vector3[] _currentVertices;

    public IntVector2 GridPosition;

    public void SetupTile(TrackTileSavable trackTileSavable, IntVector2 size, IntVector2 gridPosition, TerrainManager term, string fieldName)
    {
		_trackTileSavable = trackTileSavable;
	    Size = size;
	    GridPosition = gridPosition;
	    _terrainManager = term;
	    FieldName = fieldName;

		SetRotation(trackTileSavable.Rotation);
    }

	public void Rotate()
	{
		byte newRot = _trackTileSavable.Rotation;
		newRot += 1;
		if (newRot > 3) newRot = 0;

		SetRotation(newRot);
	}

	public Vector3 GetTransformPosition()
	{
		if (_trackTileSavable.Rotation%2 == 1)
		{
			return new Vector3((GridPosition.x + (Size.y/2)/2.0f)*TrackManager.TileSize, transform.localPosition.y, -1*(GridPosition.y + (Size.x/2)/2.0f)*TrackManager.TileSize);
		}
		return new Vector3((GridPosition.x + (Size.x/2)/2.0f)*TrackManager.TileSize, transform.localPosition.y, -1*(GridPosition.y + (Size.y/2)/2.0f)*TrackManager.TileSize);
	}

	public void UpdateTransform()
	{
		transform.localRotation = Quaternion.Euler(0, _trackTileSavable.Rotation * 90, 0);

		if(_trackTileSavable.IsMirrored != 0)
		{
			if(_trackTileSavable.Rotation%2 == 1)
				transform.localScale = new Vector3(1, 1, -1);
			else
				transform.localScale = new Vector3(-1, 1, 1);
		}
		else
		{
			transform.localScale = new Vector3(1,1,1);
		}

		transform.localPosition = GetTransformPosition();
	}

	public void Flip()
	{
		if (_trackTileSavable.IsMirrored != 0)
			_trackTileSavable.IsMirrored = 0;
		else
			_trackTileSavable.IsMirrored = 1;

		UpdateTransform();
	}

    public void SetRotation(byte rotation)
    {
        _trackTileSavable.Rotation = rotation;
        UpdateTransform();
    }

	public void SetOriginalVertices(Vector3[] original)
	{
		_originalVertices = original;
	}

	public void ApplyTerrain()
	{
		if (!GetComponent<MeshFilter>().sharedMesh)
		{
			return;
		}

		if(_currentVertices == null || _currentVertices.Length != _originalVertices.Length)
			_currentVertices = new Vector3[_originalVertices.Length];

		for (int i = 0; i < _originalVertices.Length; i++)
			_currentVertices[i] = _originalVertices[i];

		_terrainManager.ApplyTerrainToMesh(ref _currentVertices, GridPosition, _trackTileSavable.Rotation, Size, _trackTileSavable.IsMirrored > 0);

		GetComponent<MeshFilter>().mesh.vertices = _currentVertices;
		GetComponent<MeshFilter>().mesh.RecalculateBounds();
	}
}
