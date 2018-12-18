using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BinarySearch.BinarySearch))]
public class CellSpawnSorter2DEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Get a reference to the sorter.
        var sorter = (BinarySearch.BinarySearch)target;

        //If we modified any value.
        if (GUI.changed && Application.isPlaying)
        {
            //Respawn the cells.
            sorter.RefreshGame();
        }
    }
}