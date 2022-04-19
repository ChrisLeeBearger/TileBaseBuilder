using System;

namespace Assets.Events
{
    public class FurnitureCreatedEventArgs : EventArgs
    {
        public Furniture Furniture { get; set; }
    }
}
