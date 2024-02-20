using System;

namespace GMToolKit.Inspector.UndoSystem
{
    public interface IUndoCommand
    {
        public Action Undo { get; }
        public Action Do { get; }
    }
}