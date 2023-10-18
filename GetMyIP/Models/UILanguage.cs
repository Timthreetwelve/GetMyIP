// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace GetMyIP.Models;

/// <summary>
/// Class for language properties.
/// </summary>
/// <seealso cref="CommunityToolkit.Mvvm.ComponentModel.ObservableObject" />
internal partial class UILanguage : ObservableObject
{
    #region Properties
    [ObservableProperty]
    private string _contributor;

    [ObservableProperty]
    private int? _currentLanguageStringCount = App.LanguageStrings;

    [ObservableProperty]
    private int? _defaultStringCount = App.DefaultLanguageStrings;

    [ObservableProperty]
    private string _language;

    [ObservableProperty]
    private string _languageCode;

    [ObservableProperty]
    private string _languageNative;

    [ObservableProperty]
    private string _note = string.Empty;
    #endregion Properties

    #region Override ToString
    /// <summary>
    /// Overrides the ToString method.
    /// </summary>
    /// <remarks>
    /// Used to write language code to user settings file.
    /// </remarks>
    /// <returns>
    /// The language code as a string.
    /// </returns>
    public override string ToString()
    {
        return LanguageCode;
    }
    #endregion Override ToString

    #region List of languages
    /// <summary>
    /// List of languages with language code
    /// </summary>
    private static List<UILanguage> LanguageList { get; } = new()
    {
        new UILanguage {Language = "English", LanguageCode = "en-US", LanguageNative = "English",    Contributor = "Timthreetwelve", Note="Default"},
        new UILanguage {Language = "English", LanguageCode = "en-GB", LanguageNative = "English",    Contributor = "Timthreetwelve"},
    };

    /// <summary>
    /// List of defined languages ordered by LanguageNative.
    /// </summary>
    public static List<UILanguage> DefinedLanguages => LanguageList.OrderBy(x => x.LanguageNative).ToList();
    #endregion List of languages
}
