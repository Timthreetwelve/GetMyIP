// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace GetMyIP
{
    // Must use IComparable to be able to sort the lists
    public class IPInfo : IComparable<IPInfo>
    {
        #region Properties
        public string Parameter { get; set; }

        public string Value { get; set; }
        #endregion Properties

        #region Constructor
        public IPInfo(string parm, string val)
        {
            Parameter = parm;
            Value = val;
        }
        #endregion Constructor

        #region Lists
        public static List<IPInfo> GeoInfoList = new List<IPInfo>();
        public static List<IPInfo> InternalList = new List<IPInfo>();
        #endregion Lists

        #region Comparer
        public int CompareTo(IPInfo other)
        {
            // A null value means that this object is greater.
            if (other == null)
            {
                return 1;
            }
            else
            {
                return Parameter.CompareTo(other.Parameter);
            }
        }
        #endregion Comparer
    }
}
