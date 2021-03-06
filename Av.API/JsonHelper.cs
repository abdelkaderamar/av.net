// Copyright (c) Abdelkader Amar. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


﻿using System;
using Newtonsoft.Json.Linq;
using System.Globalization;
using log4net;

namespace Av.API
{
    public static class JsonHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(JsonHelper));

        public static string GetValue(JObject json, string property)
        {
            if (json.ContainsKey(property)) return json.GetValue(property).ToString();
            return string.Empty;
        }

        public static double GetDoubleValue(JObject json, string property, double defaultValue = 0.0)
        {
            if (json.ContainsKey(property))
            {
                string str = json.GetValue(property).ToString();
                double value;
                if (double.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                    return value;
            }
            return defaultValue;
        }

        public static decimal GetDecimalValue(JObject json, string property, decimal defaultValue = 0.0m)
        {
            if (json.ContainsKey(property))
            {
                string str = json.GetValue(property).ToString();
                decimal value;
                if (Decimal.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                    return value;
            }
            return defaultValue;
        }

        public static long GetLongValue(JObject json, string property, long defaultValue = 0)
        {
            if (json.ContainsKey(property))
            {
                string str = json.GetValue(property).ToString();
                long value;
                if (long.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                    return value;
            }
            return defaultValue;
        }

        public static DateTime GetDateTimeValue(JObject json, string property, string dateFormat)
        {
            try
            {
                string str = GetValue(json, property);
                DateTime dateTime = DateTime.ParseExact(str, dateFormat, CultureInfo.InvariantCulture);
                return dateTime;
            }
            catch (Exception e)
            {
                log.ErrorFormat("Exception when parsing json data {0}", e.StackTrace);
                throw e;
            }
        }

        public static DateTime GetDateTimeValue(JObject json, string property)
        {
            var epoch = GetLongValue(json, property);
            return UnixTimeStampToDateTime(epoch);
        }


        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static bool GetBooleanValue(JObject json, string property, bool defaultValue = false)
        {
            var str = GetValue(json, property);
            bool value;
            if (bool.TryParse(str, out value)) return value;
            return defaultValue;
        }

    }
}
