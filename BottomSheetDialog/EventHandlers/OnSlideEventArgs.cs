using System;
namespace BottomSheetDialog.EventHandlers
{
    public class OnSlideEventArgs
    {
        public OnSlideEventArgs(float slideOffset)
        {
            SlideOffset = slideOffset;
        }

        public float SlideOffset { get; }
    }
}
