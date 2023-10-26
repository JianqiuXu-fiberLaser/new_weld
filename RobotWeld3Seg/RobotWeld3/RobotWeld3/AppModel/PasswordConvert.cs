///////////////////////////////////////////////////////////////////////
//
// Sunflower Laser Welding System, Ver. 2.0 
// Designed by: Jiangsu Jacalt laser Tech Corp.
// Copyright (c) 2022, Jiangsu Jacalt laser Tech Corp.
// WebSite: https://www.jacalt-laser.com
// The Software used the GPL licenses. 
//
///////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace RobotWeld3.AppModel
{
    public class PasswordConvert : IValueConverter
    {
        private string? realWord = string.Empty;
        private char replaceChar = '*';

        public PasswordConvert() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null)
            {
                string? temp = parameter.ToString();
                if (!string.IsNullOrEmpty(temp))
                {
                    replaceChar = temp.First();
                }
            }

            if (value != null)
            {
                realWord = value.ToString();
            }


            string replaceWord = string.Empty;
            if (!string.IsNullOrEmpty(realWord))
            {
                for (int index = 0; index < realWord.Length; ++index)
                {
                    replaceWord += replaceChar;
                }
            }

            return replaceWord;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string backValue = string.Empty;
            if (value != null)
            {
                string? strValue = value.ToString();
                if (!string.IsNullOrEmpty(strValue) && realWord != null)
                {
                    for (int index = 0; index < strValue.Length; ++index)
                    {
                        if (strValue.ElementAt(index) == replaceChar)
                        {
                            backValue += realWord.ElementAt(index);
                        }
                        else
                        {
                            backValue += strValue.ElementAt(index);
                        }
                    }
                }

            }
            return backValue;
        }
    }
}
