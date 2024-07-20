using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

[Serializable]
public class GridVO
{
    [EnumToggleButtons] public Enums.BlockType SelectedTileType;

    [OnValueChanged("CreateGrid")]
    [Range(3, 8)] public int GridWidth = 5;
    [OnValueChanged("CreateGrid")]
    [Range(3, 12)] public int GridHeight = 5;

    [SerializeField][HideInInspector] public List<TileVO> Grid;

#if UNITY_EDITOR
    [ShowInInspector]
    [TableMatrix(SquareCells = true, DrawElementMethod = "DrawElement")]
    private TileVO[,] _editorGrid;
    [SerializeField] private TileVO _insertationTileData;

    private TileVO DrawElement(Rect rect, TileVO tile)
    {
        switch (tile.Type)
        {
            case Enums.BlockType.Regular:

                EditorGUI.DrawRect(rect.Padding(1), Color.white);

                break;
            case Enums.BlockType.Special:

                EditorGUI.DrawRect(rect.Padding(1), Color.white);

                break;
            case Enums.BlockType.Obstacle:

                EditorGUI.DrawRect(rect.Padding(1), Color.white);

                break;
        }

        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            if (Event.current.button == 0)
            {
                tile.Type = SelectedTileType;
            }

            Serialize();
            _insertationTileData = tile;
            GUI.changed = true;
            Event.current.Use();
        }

        var labelStyle = EditorStyles.label;
        labelStyle.alignment = TextAnchor.UpperCenter;
        labelStyle.fontSize = 18;
        labelStyle.fontStyle = FontStyle.Bold;
        EditorGUI.LabelField(rect, $"{tile.Type}");

        if (tile.Type == Enums.BlockType.Regular)
        {
            labelStyle.alignment = TextAnchor.LowerCenter;
            EditorGUI.LabelField(rect, $"{tile.Color}");
        }

        return tile;
    }

    private void CreateGrid()
    {
        Grid = new List<TileVO>();
        _editorGrid = new TileVO[GridWidth, GridHeight];

        for (int i = 0; i < GridHeight * GridWidth; i++)
        {
            Grid.Add(new TileVO
            {
                Type = Enums.BlockType.Regular,
            });
        }

        Deserialize();
    }

    [OnInspectorInit]
    private void OnInit()
    {
        _editorGrid = new TileVO[GridWidth, GridHeight];
        Deserialize();
    }

    private void Serialize()
    {
        for (int i = 0; i < GridWidth; i++)
        {
            for (int j = 0; j < GridHeight; j++)
            {
                Grid[i * GridHeight + j] = _editorGrid[i, j];
            }
        }
    }
    private void Deserialize()
    {
        for (int i = 0; i < GridWidth; i++)
        {
            for (int j = 0; j < GridHeight; j++)
            {
                _editorGrid[i, j] = Grid[i * GridHeight + j];
            }
        }
    }
#endif
}
