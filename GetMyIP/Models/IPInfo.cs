// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

/// <summary>
/// Initializes a new instance of the <see cref="IPInfo"/> class.
/// </summary>
/// <param name="parm">The parm.</param>
/// <param name="val">The value.</param>
public class IPInfo(string parm, string val)
{
    #region Properties
    /// <summary>
    /// Gets or sets the parameter.
    /// </summary>
    /// <value>The parameter.</value>
    public string Parameter { get; set; } = parm;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>The value.</value>
    public string Value { get; set; } = val;
    #endregion Properties

    #region Lists
    /// <summary>
    /// The Geo information list
    /// </summary>
    public static readonly ObservableCollection<IPInfo> GeoInfoList = [];
    /// <summary>
    /// The internal list
    /// </summary>
    public static readonly ObservableCollection<IPInfo> InternalList = [];
    #endregion Lists
}
