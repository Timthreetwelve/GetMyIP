# Copilot Instructions

## Project Guidelines
- Display last refresh time in local user time, not UTC.

## Build, test, and lint

- **SDK/runtime target:** .NET SDK `10.0.400` (`global.json`), WPF on Windows (`net10.0-windows`).
- **Restore:** `dotnet restore GetMyIP.slnx`
- **Build (solution):** `dotnet build GetMyIP.slnx -c Debug`
- **Build (single project):** `dotnet build GetMyIP\GetMyIP.csproj -c Debug`
- **Run locally:** `dotnet run --project GetMyIP\GetMyIP.csproj`
- **Run hidden/history mode (command-line path):** `dotnet run --project GetMyIP\GetMyIP.csproj -- --hide`
- **Tests:** there is currently no automated test project in this repository.
- **Linting/analyzers:** there is no separate lint script; code style/analyzer enforcement happens during `dotnet build` (`AnalysisLevel=latest-recommended`, `EnforceCodeStyleInBuild=true` in `GetMyIP.csproj`).

## Commit Message Format

- The commit message should have a short description (50 characters or less) followed by a blank line and then a longer description.
- The short description should be in the imperative mood (e.g., "Fix bug" or "Add feature").
- The longer description should provide additional context about the change, including any relevant background information, the motivation for the change, and any potential impact on the project.
- The longer description should use bullet points to organize information and make it easier to read.
- Reference any related issues or pull requests at the end of the long description. If no related issues or pull requests exist, omit this section.
- If the commit fixes an issue or task, include Fixes #<issue-number> or Closes #<issue-number> at the end of the long description.

## Key repository conventions

- **Localization source of truth:** `GetMyIP/Languages/Strings.en-US.xaml` is the authoritative key set. Other `Strings.<culture>.xaml` files must stay key-compatible with it.
- **Localization change log is required:** when changing `Strings.en-US.xaml`, add/update the timestamped change-log comments at the bottom using existing `A |`, `U |`, `D |` format.
- **Preserve localization structure exactly:** if a string uses `xml:space="preserve"` and/or encoded line breaks (`&#x0a;`), keep those semantics in translated files.
- **MVVM Toolkit pattern:** ViewModels and settings classes use CommunityToolkit attributes (`[ObservableProperty]`, `[RelayCommand]`) with partial classes; follow the existing generated-property style.
- **Global usings are intentional:** many files rely on `GlobalUsings.cs`, including `using static GetMyIP.Helpers.NLogHelpers` and `using static GetMyIP.Helpers.ResourceHelpers`; avoid adding redundant per-file imports unless required (for example, `Octokit` is intentionally local in `GitHubHelpers.cs`).
- **User-facing text should come from resources:** prefer `GetStringResource(...)` keys over inline UI strings so language files remain the single localization surface.

## High-level architecture

- This is a **single-project WPF desktop app** (`GetMyIP/GetMyIP.csproj`) with MVVM.
- `App.xaml` defines shared Material Design theme resources and the DataTemplate mapping from ViewModels to Views. `MainWindow` sets `DataContext = new NavigationViewModel()`.
- Startup flow is concentrated in `App.OnStartup` and `MainWindowHelpers.GetMyIPStartUp()`:
  - initialize settings (`ConfigHelpers.InitializeSettings`)
  - configure logging (`NLogHelpers.NLogConfig`)
  - select/load localization resources (`Languages/Strings.*.xaml`)
  - apply UI settings (theme, accent color, scaling, window position)
- Runtime state is centralized in static settings objects:
  - persisted settings: `ConfigManager<UserSettings>.Setting` (`usersettings.json` in app directory)
  - temporary state: `ConfigManager<TempSettings>.Setting`
  - `SettingChange.UserSettingChanged` is the side-effect hub for setting updates (theme switch, log level, restart on language change, etc.).
- Logging is centralized through NLog and a global static logger (`_log`) from `NLogHelpers`; log files are written to `%TEMP%\T_K\...`.

## Coding conventions

- **C# 12 features:** use `file-scoped namespaces`, `global using` where appropriate.
- Prefer explicit type declarations over `var` for public properties, method parameters, and return types.

## About the author

- I'm a solo developer and the sole maintainer of this project.
- I'm a hobbiest programmer and not a professional software engineer.
- My skillset is limited but I'm willing to learn and improve. 
- Please explain things to me in simple terms if I ask for help.
- I desire to publish a simple, useful, and reliable desktop application that is easy to use and maintain.