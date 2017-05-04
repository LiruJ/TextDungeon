using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDungeonGame.Entities
{
    public class Door : Entity
    {
        #region Private Properties
        /// <summary>The tick the door was last opened</summary>
        private int openedOnTick;

        /// <summary>How many ticks it takes for the door to close on its own</summary>
        private const int ticksForDoorToClose = 5;
        #endregion

        #region Public Properties
        /// <summary>If this door is open</summary>
        public bool DoorOpen;
        #endregion

        #region Constructors
        /// <summary>Makes a door at this position</summary>
        /// <param name="pos">The position to place the door</param>
        public Door(Position pos) : base(pos)
        {
            CloseDoor();
        }
        #endregion

        #region Public Functions
        /// <summary>Closes the door if it's been open long enough</summary>
        /// <param name="currentTick"></param>
        public override void Update(int currentTick)
        {
            if (currentTick - openedOnTick >= ticksForDoorToClose && DoorOpen)
                CloseDoor();
        }

        /// <summary>Opens the door and keeps track of the tick it was opened on</summary>
        /// <param name="currentTick">The current tick</param>
        public void OpenDoor(int currentTick)
        {
            openedOnTick = currentTick;
            Character = (char)Entities.OpenDoor;
            DoorOpen = true;
            IsTransparent = true;
        }

        /// <summary>Closes the door</summary>
        public void CloseDoor()
        {
            Character = (char)Entities.ClosedDoor;
            DoorOpen = false;
            IsTransparent = false;
        }
        #endregion
    }
}
