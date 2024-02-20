using System;

namespace GMToolKit.Inspector
{
    public class RangeAttribute : InspectAttribute
    {
        public float min, max;
        public RangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}