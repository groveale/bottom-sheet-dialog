using System;
namespace BottomSheetDialog.EventHandlers
{
    public class OnStateChangeEventArgs
    {
        public OnStateChangeEventArgs(int newState)
        {
            NewState = newState;
        }

        public int NewState { get; }
    }
}
