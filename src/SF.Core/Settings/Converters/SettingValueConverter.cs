﻿using SF.Core.Entitys;
using System;
using System.Globalization;


namespace SF.Core.Settings.Converters
{
    public static class SettingValueConverter
    {
        public static SettingValueEntity ToEntity(this string value, SettingValueType valueType)
        {
            var result = new SettingValueEntity { ValueType = valueType.ToString() };

            if (valueType == SettingValueType.Boolean)
            {
                result.BooleanValue = Convert.ToBoolean(value);
            }
            else if (valueType == SettingValueType.DateTime)
            {
                result.DateTimeValue = Convert.ToDateTime(value, CultureInfo.InvariantCulture);
            }
            else if (valueType == SettingValueType.Decimal)
            {
                result.DecimalValue = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            }
            else if (valueType == SettingValueType.Integer)
            {
                result.IntegerValue = Convert.ToInt32(value, CultureInfo.InvariantCulture);
            }
            else if (valueType == SettingValueType.LongText)
            {
                result.LongTextValue = value;
            }
            else if (valueType == SettingValueType.SecureString)
            {
                result.ShortTextValue = value;
            }
            else
            {
                result.ShortTextValue = value;
            }

            return result;
        }
    }
}
