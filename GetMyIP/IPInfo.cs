// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP
{
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
        public static readonly ObservableCollection<IPInfo> FakeInfoList = new();
        public static readonly ObservableCollection<IPInfo> GeoInfoList = new();
        public static readonly ObservableCollection<IPInfo> InternalList = new();
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
