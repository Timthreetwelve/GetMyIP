// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

/// <summary>
/// Initializes a new instance of the <see cref="IPInfo"/> class.
/// </summary>
/// <param name="parm">The parm.</param>
/// <param name="val">The value.</param>
public class IPInfo(string parm, string val) : IComparable<IPInfo>
{
    #region Properties
    /// <summary>
    /// Gets or sets the parameter.
    /// </summary>
    /// <value>
    /// The parameter.
    /// </value>
    public string Parameter { get; set; } = parm;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public string Value { get; set; } = val;
    #endregion Properties

    #region Lists
    /// <summary>
    /// The geo information list
    /// </summary>
    public static readonly ObservableCollection<IPInfo> GeoInfoList = [];
    /// <summary>
    /// The internal list
    /// </summary>
    public static readonly ObservableCollection<IPInfo> InternalList = [];
    #endregion Lists

    #region Comparer
    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether
    /// the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="other">An object to compare with this instance.</param>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
    /// </returns>
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
