using System;

namespace Assets.Events
{
    public class CharacterCreatedEventArgs : EventArgs
    {
        public Character Character { get; set; }
    }
}
