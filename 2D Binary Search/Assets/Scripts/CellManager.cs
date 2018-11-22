using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CellManager : MonoBehaviour
{
    [Header("Cell Settings")]
    [Space(5f)]

    [SerializeField]
    private int cellCount = 100;

    [SerializeField]
    private float spawnDelay = 0.005f;

    [Header("Turn")]
    [Space(5f)]

    [SerializeField]
    private float turnDelay = 0.1f;

    [Header("Aspect")]
    [Space(5f)]

    [SerializeField]
    private Sprite passedCellSprite;

    [SerializeField]
    private Sprite goalCellSprite;

    [SerializeField]
    private Sprite currentCellSprite;

    private GameObject cellInstance;
    private List<GameObject> cells = new List<GameObject>();

    private float lastCellSpawnTime;
    private bool cellsSpawned;

    private float lastTurnTime;

    private Image goalCell;
    private Image currentCell;

    private int left;
    private int right;

    private int bottom;
    private int top;

    private void Start()
    {
        //Get the cell object.
        cellInstance = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        //Spawn cells if needed.
        if (cellsSpawned == false)
        {
            SpawnCells();

            return;
        }

        //If not enough time has passed, don't start the new turn.
        if(Time.time - lastTurnTime > turnDelay)
            return;

        ExecuteTurn();
    }

    private void ExecuteTurn()
    {

    }

    private void SelectStartingCells()
    {
        //Get a random cell to be the bomb cell.
        int goalIndex = Random.Range(0, cellCount);
        //Get the goal cell.
        goalCell = cells[goalIndex].GetComponent<Image>();
        //Change the sprite.
        goalCell.sprite = goalCellSprite;

        //Get a random cell to be the starting cell.
        int startingIndex = Random.Range(0, cellCount);
        if (startingIndex == goalIndex)
            startingIndex = Random.Range(0, cellCount);

        //Get the goal cell.
        currentCell = cells[startingIndex].GetComponent<Image>();
        //Change the sprite.
        currentCell.sprite = currentCellSprite;
    }

    private void SpawnCells()
    {
        //Go back if we've spawned enough cells.
        if (cells.Count == cellCount)
        {
            //All cells have been spawned.
            cellsSpawned = true;

            SelectStartingCells();

            return;
        }

        //Don't try spawning if enough time hasn't passed yet.
        if (Time.time - lastCellSpawnTime < spawnDelay)
            return;

        //Create a new instance.
        GameObject newCell = Instantiate(cellInstance, transform);
        //Activate the new cells.
        newCell.SetActive(true);

        //Save the newly instantiated cell.
        cells.Add(newCell);

        //Save the time we just spawned the cell at.
        lastCellSpawnTime = Time.time;
    }
}