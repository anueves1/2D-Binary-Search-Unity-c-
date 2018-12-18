using UnityEngine;
using UnityEngine.UI;

namespace BinarySearch
{
    public enum CellType
    {
        Normal, Occupied, Goal
    }

    public class Cell : MonoBehaviour
    {
        public CellType CellType => cellType;
        public Vector2 CellPosition => cellPosition;

        [SerializeField]
        private CellType cellType;

        [SerializeField]
        private Vector2 cellPosition;

        private Image imageComponent;
        private Sprite[] sprites;

        /// <summary>
        /// Function called when this cell gets spawned, takes care
        /// of setting up some values.
        /// </summary>
        /// <param name="cellPosition"></param>
        /// <param name="sprites"></param>
        public void OnSpawn(Vector2 cellPosition, Sprite[] sprites)
        {
            //Get a reference to the image component on the cell.
            imageComponent = GetComponent<Image>();

            //Save the sprites.
            this.sprites = sprites;

            //Save the cell's position.
            this.cellPosition = cellPosition;
        }

        /// <summary>
        /// This function changes the type of cell this cell is.
        /// </summary>
        /// <param name="cellType"></param>
        public void SetType(CellType cellType)
        {
            //Set the new cell type.
            this.cellType = cellType;

            //Get the type index.
            var appearenceIndex = (int)cellType;

            //Assign the correct sprite.
            imageComponent.sprite = sprites[appearenceIndex];
        }
    }
}