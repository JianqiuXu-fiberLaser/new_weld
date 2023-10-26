///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 3.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// The Software used the GPL licenses. 
//
// Ver 2.2: (1) check module for each key input. 
//
///////////////////////////////////////////////////////////////////////

using System.Windows.Input;

namespace RobotWeld3.AlgorithmsBase
{
    class KeyInput
    {
        /// <summary>
        /// If the input characters in TextBox are numbers, return true.
        /// </summary>
        /// <param name="key"> the key enumerate </param>
        /// <returns> True: int number </returns>
        internal static bool IsIntNumber(Key key)
        {
            if (key == Key.Enter || key == Key.Back ||
                (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// If the input characters in TextBox are double numbers
        /// </summary>
        /// <param name="key"> the key enumerate </param>
        /// <returns> true for double number, include period </returns>
        internal static bool IsNumber(Key key)
        {
            if (key == Key.Enter || key == Key.Back || key == Key.OemPeriod || key == Key.Decimal ||
                (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
