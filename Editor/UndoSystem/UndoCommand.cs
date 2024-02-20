using System;

namespace GMToolKit.Inspector.UndoSystem
{
    public interface IUndoCommand
    {
        public Action Undo { get; }
        public Action Do { get; }
    }

    public class UndoCommand : IUndoCommand
    {
        public Action Undo { get; set; }
        public Action Do { get; set; }
    }
}