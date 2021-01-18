using System;

namespace Calculator
{
  
    class Angles
    {
        public enum units
        {
            RADIANS, 
            DEGREES,
            GRADIANS
        }

        public class Converter
        {
           
            /// /// <param name="angle">
            /// <param name="angleUnit">
            /// <returns></returns>
            public static double degrees(double angle, units angleUnit)
            {
                if (angleUnit == units.RADIANS)
                    return angle * 180 / Math.PI;
                else if (angleUnit == units.GRADIANS)
                    return angle * 9 / 10;
                else if (angleUnit == units.DEGREES)
                    return angle;
                else
                {
                    Exception error = new Exception("Invalid parameters");
                    throw error;
                }
            }

            
            /// /// <param name="angle">
            /// <param name="angleUnit">
            /// <returns></returns>
            public static double radians(double angle, units angleUnit)
            {
                
                if (angleUnit == units.RADIANS)
                    return angle;

                return degrees(angle, angleUnit) * Math.PI / 180;
            }

           
            /// /// <param name="angle">
            /// <param name="angleUnit">
            /// <returns></returns>
            public static double gradians(double angle, units angleUnit)
            {
                
                if (angleUnit == units.GRADIANS)
                    return angle;

                return degrees(angle, angleUnit) * 10 / 9;
            }
        }
    }
}
