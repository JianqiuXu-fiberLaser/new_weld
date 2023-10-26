///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
// Ver.3.0: Base class for enum Tracetype.
//
///////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace RobotWeld3.GetTrace
{
    /// <summary>
    /// Base Clase for enum of Tracetype.
    /// </summary>
    public class CTraceType
    {
        public CTraceType() { }

        /// <summary>
        /// Get List of enum Tracetype
        /// </summary>
        /// <returns> List<Tracetype> </Tracetype></returns>
        public static List<Tracetype> GetTracetypeList()
        {
            int tLength = Enum.GetNames(typeof(Tracetype)).Length;

            var tl = new List<Tracetype>();
            for (int i = 1; i < tLength; i++)
            {
                var tt = i switch
                {
                    1 => Tracetype.FREE_TRACE,
                    2 => Tracetype.INTERSECT,
                    3 => Tracetype.STAGE_TRACE,
                    4 => Tracetype.SPIRAL,
                    _ => Tracetype.NONE,
                };

                tl.Add(tt);
            }

            return tl;
        }

        /// <summary>
        /// Get the Xml name from tracetype
        /// </summary>
        /// <param name="tpy"></param>
        /// <returns> the string name used in Xml</returns>
        public static string GetName(Tracetype tpy)
        {
            string s = tpy switch
            {
                Tracetype.FREE_TRACE => "free",
                Tracetype.INTERSECT => "intersect",
                Tracetype.STAGE_TRACE => "stage",
                Tracetype.SPIRAL => "spiral",
                _ => "none",
            };

            return s;
        }

        public static Tracetype GetTrace(string str)
        {
            var tpy = str switch
            {
                "free" => Tracetype.FREE_TRACE,
                "intersect" => Tracetype.INTERSECT,
                "stage" => Tracetype.STAGE_TRACE,
                "spiral" => Tracetype.SPIRAL,
                _ => Tracetype.NONE,
            };

            return tpy;
        }
    }

    /// <summary>
    /// The type of trac to run
    /// </summary>
    public enum Tracetype
    {
        NONE = 0,
        FREE_TRACE = 1,
        INTERSECT = 2,
        STAGE_TRACE = 3,
        SPIRAL = 4
    }
}
