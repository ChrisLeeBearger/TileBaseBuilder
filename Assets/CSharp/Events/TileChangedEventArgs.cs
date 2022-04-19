using System;

namespace Assets.Events
{
    public class TileChangedEventArgs : EventArgs
    {
        public Tile Tile { get; set; }
    }
}
