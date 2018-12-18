using UnityEngine;

namespace BinarySearch
{
    public class BinarySolver : MonoBehaviour
    {
        private Vector2 size;
        private Vector2 currentPosition;

        private Vector2 startPosition;
        private Vector4 checkGrid;

        public BinarySolver(Vector2 size, Vector2 startPosition)
        {
            //Save the size.
            this.size = size;

            //Save the start position.
            this.startPosition = startPosition;

            //Save the current position.
            this.currentPosition = startPosition;
        }

        public void Setup(string indication)
        {
            //Go back if there's no indication.
            if (indication == string.Empty)
                return;

            //Check the first letter in the indication.
            switch (indication[0])
            {
                case 'U':
                    checkGrid.z = 0;
                    checkGrid.w = startPosition.y;
                    break;
                case 'D':
                    checkGrid.z = startPosition.y;
                    checkGrid.w = size.y;
                    break;
            }

            //If there's two letters to the indication.
            if (indication.Length > 1)
            {
                //Check the second letter.
                switch (indication[1])
                {
                    case 'R':
                        checkGrid.x = startPosition.x;
                        checkGrid.y = size.x;
                        break;
                    case 'L':
                        checkGrid.x = 0;
                        checkGrid.y = startPosition.x;
                        break;
                }
            }
        }

        public Vector2 GetNewPositionByIndication(string indication)
        {
            if (indication.Contains("D"))
                checkGrid.z = currentPosition.y + 1;
            else if (indication.Contains("U"))
                checkGrid.w = currentPosition.y - 1;

            if (indication.Contains("R"))
                checkGrid.x = currentPosition.x + 1;
            else if (indication.Contains("L"))
                checkGrid.y = currentPosition.x - 1;

            //Calculate the new x position.
            currentPosition.x = (int)(checkGrid.x + checkGrid.y) / 2;
            //Calculate the new y position.
            currentPosition.y = (int)(checkGrid.z + checkGrid.w) / 2;

            return currentPosition;
        }
    }
}
