using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

namespace BinarySearch
{
    public class BinarySearch : MonoBehaviour
    {
        [SerializeField]
        private KeyCode refreshKey = KeyCode.R;

        [Header("Setup")]
        [Space(5f)]

        [Tooltip("Determines the number of rows and columns of cells that are spawned. (Columns x rows)")]
        [SerializeField]
        private Vector2 size = new Vector2(11, 11);

        [Tooltip("Determines the time that passes between each cell spawned.")]
        [SerializeField]
        private float spawnDelay = 0.05f;

        [Tooltip("Determines the position that the first cell will have from the origin.")]
        [SerializeField]
        private Vector2 padding = new Vector2(0.50f, -0.50f);

        [Tooltip("Determines the distance(In pixels) between each cell's x and y position.")]
        [SerializeField]
        private Vector2 spacing = new Vector2(55, 55);

        [Header("Turns")]
        [Space(5f)]

        [SerializeField]
        private float turnDelay = 0.1f;

        [SerializeField]
        private TextMeshProUGUI moveText;

        [Header("Cells")]
        [Space(5f)]

        [SerializeField]
        private Sprite[] sprites;

        private Vector2 lastPosition;
        private float lastSpawnTime;

        private GameObject cellPrefab;
        private bool cellsHaveSpawned;

        private Transform cellHolder;
        private List<Cell> cells = new List<Cell>();

        private Cell goalCell;
        private Cell occupiedCell;

        private int currentMove = -1;
        private float lastMoveTime;

        private bool isDone;
        private BinarySolver bSolver;

        private void Awake()
        {
            //Load the cell instance prefab.
            cellPrefab = (GameObject)Resources.Load("Cell Instance 2D");

            //Create a new cell holder object.
            cellHolder = new GameObject("Cell Holder").transform;
            //Parent it to this object.
            cellHolder.SetParent(transform);
            //Reset the local position.
            cellHolder.localPosition = default;
        }

        private void Update()
        {
            //If we press the refresh key.
            if(Input.GetKeyDown(refreshKey))
            {
                //Refresh the game.
                RefreshGame();
            }
        }

        private void FixedUpdate()
        {
            //If we haven't finished spawning the cells.
            if (cellsHaveSpawned == false)
                SpawnCells();
            //Run the game if it's not over yet.
            else if (isDone == false)
                TickGame();
        }

        /// <summary>
        /// This function takes care of all that happens in a single turn.
        /// Things that happen: We find the new direction, we move the cell...
        /// </summary>
        private void TickGame()
        {
            //Go back if not enough time has passed in order to start the next turn.
            if (Time.time - lastMoveTime < turnDelay)
                return;

            //Get the indication to the goal for this turn.
            var indicationToGoal = GetIndicationToGoal();

            //If there was no indication to the goal.
            if (indicationToGoal == string.Empty)
            {
                //The search is finished.
                isDone = true;

                //Reset the moves.
                currentMove = 0;

                return;
            }
            //If there is an indication to the goal.
            else
            {
                //If this is the first move, setup the solver.
                if (currentMove == 0)
                    bSolver.Setup(indicationToGoal);
                //If it is not the first move.
                else
                {
                    //Get the next position to move towards.
                    Vector2 nextMove = bSolver.GetNewPositionByIndication(indicationToGoal);

                    //Update the occupied cell's position.
                    UpdateWithNewOccupied(nextMove);
                }
            }

            //Update the move text.
            moveText.text = "Moves => " + currentMove;

            //Start the next turn.
            currentMove++;

            //Save the turn time.
            lastMoveTime = Time.time;
        }

        /// <summary>
        /// This function takes care of resetting all the cells back to normal, and 
        /// then updating the occupied cell to the new position.
        /// It also keeps the goal cell at the same position.
        /// </summary>
        /// <param name="newOccupied"></param>
        private void UpdateWithNewOccupied(Vector2 newOccupied)
        {
            //Go trough the cells.
            for (var i = 0; i < cells.Count; i++)
            {
                //Get the current cell.
                Cell current = cells[i];

                //If this is the new occupied cell.
                if(current.CellPosition == newOccupied)
                {
                    //Save the cell.
                    occupiedCell = current;

                    //Set the cell to be occupied.
                    occupiedCell.SetType(CellType.Occupied);

                    continue;
                }

                //Skip the goal cell.
                if (current == goalCell)
                    continue;

                //Set them to normal.
                current.SetType(CellType.Normal);
            }
        }

        /// <summary>
        /// This function creates a string that contains the direction towards which
        /// the occupied cell would have to go in order to find the goal cell.
        /// </summary>
        /// <returns></returns>
        private string GetIndicationToGoal()
        {
            var indication = string.Empty;

            //Go back if there's no goal cell.
            if (goalCell == null)
                return indication;

            //Compute the direction from the occupied cell to the goal cell.
            var directionToGoal = goalCell.CellPosition - occupiedCell.CellPosition;
            //Normalize the result.
            directionToGoal.Normalize();

            //Add the corresponding first direction.
            if (directionToGoal.y > 0)
                indication += "D";
            else if (directionToGoal.y < 0)
                indication += "U";

            //Add the corresponding second direction.
            if (directionToGoal.x < 0)
                indication += "L";
            else if (directionToGoal.x > 0)
                indication += "R";

            return indication;
        }

        /// <summary>
        /// This function takes care of spawning the cells in the correct order and
        /// adding the correct time delay between each spawn. It also selects the 
        /// occupied cell afterwards.
        /// </summary>
        private void SpawnCells()
        {
            //If we have already finished spawning the cells.
            if(cells.Count == size.x * size.y)
            {
                //Set the bool.
                cellsHaveSpawned = true;

                //Use the first cell as the occupied one.
                occupiedCell = cells[0];
                //Set its type accordingly.
                occupiedCell.SetType(CellType.Occupied);

                //Initialize the binary solver.
                bSolver = new BinarySolver(size, occupiedCell.CellPosition);

                return;
            }

            //Go back if not enough time has passed.
            if (Time.time - lastSpawnTime < spawnDelay)
                return;

            //Take the last position and make the y positive.
            var newPosition = lastPosition;
            newPosition.y *= -1;

            //Add the padding.
            newPosition += padding;
            //Add the spacing.
            newPosition *= spacing;

            //Spawn the new cell.
            SpawnCell(newPosition);

            //Increment the x.
            lastPosition.x++;

            //If we've finished spawning all columns in a row.
            if(lastPosition.x >= size.x)
            {
                //Move to the next one.
                lastPosition.y++;

                //And start from the first column.
                lastPosition.x = 0;
            }

            //Clamp the x to the column count.
            lastPosition.x = Mathf.Clamp(lastPosition.x, 0, size.x);
            //Clamp the y to the row count.
            lastPosition.y = Mathf.Clamp(lastPosition.y, 0, size.y);

            //Save the new spawn time.
            lastSpawnTime = Time.time;
        }

        /// <summary>
        /// This function is called from the "SpawnCells" function and, takes care of spawning
        /// a singular cell based on the position passed to it.
        /// </summary>
        /// <param name="newPosition"></param>
        private void SpawnCell(Vector2 newPosition)
        {
            //Spawn the new cell and parent it to the cell holder.
            GameObject newCellInstance = Instantiate(cellPrefab, cellHolder);
            //Set the new cell's name.
            newCellInstance.name = "Cell At => " + " (" + lastPosition.x + ", " + lastPosition.y + ")";

            //Get the cell's transform.
            RectTransform cellRect = newCellInstance.GetComponent<RectTransform>();
            //Set the cell's position.
            cellRect.anchoredPosition = newPosition;

            //Add the cell component on the new cell.
            Cell newCell = newCellInstance.AddComponent<Cell>();
            //Call on spawn.
            newCell.OnSpawn(lastPosition, sprites);

            //Get the button component on the cell.
            Button cellButton = newCellInstance.GetComponent<Button>();
            //Add a on click event to the cell to set the goal cell.
            cellButton.onClick.AddListener(() => { SetGoalCell(cellButton.transform); });

            //Add it to the cells list.
            cells.Add(newCell);
        }

        /// <summary>
        /// This function is called when the game needs to be restarted.
        /// </summary>
        public void RefreshGame()
        {
            //Re-initialize the cell list.
            cells = new List<Cell>();

            //Reset the moves.
            currentMove = 0;

            //Reset the last move time.
            lastMoveTime = 0f;

            //Reset the move text too.
            moveText.text = "Moves => 0";

            //Destroy all the other objects.
            for (var c = 0; c < cellHolder.childCount; c++)
                Destroy(cellHolder.GetChild(c).gameObject);

            //Reset the position.
            lastPosition = Vector2.zero;

            //The cells haven't been spawned yet.
            cellsHaveSpawned = false;
               
            //Reset the last spawn time.
            lastSpawnTime = 0f;
        }

        /// <summary>
        /// This function is called when one of the cell buttons is clicked, and takes care
        /// of making the clicked cell, the new goal cell.
        /// </summary>
        /// <param name="newGoal"></param>
        public void SetGoalCell(Transform newGoal)
        {
            //If there was already a goal cell and it isn't occupied.
            if(goalCell != null && goalCell.CellType != CellType.Occupied)
            {
                //Set it to normal.
                goalCell.SetType(CellType.Normal);
            }

            //Find the new goal cell.
            goalCell = cells.First(x => x.transform == newGoal);

            //Set it to be a goal cell type.
            goalCell.SetType(CellType.Goal);

            //Allow the game to start now that we have a goal cell.
            isDone = false;
        }
    }
}