using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TileGame
{
    class SafeCast
    {
        public static int? ToInt32(object value)
        {
            int? returnValue = null;

            int temp;
            if( int.TryParse(value.ToString(), out temp))
            {
                returnValue = temp;
            }

            return returnValue;
        }
    }
}
