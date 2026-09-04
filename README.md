# XNAEngine

Visual Studio 2008 VB.NET XNA 3.1 engine with camera, skybox, heightmap terrain, models, textures, XACT sound, and bitmap text. `XNA.XNAEngine` inherits `Microsoft.Xna.Framework.Game`: Escape exits, F1 toggles fullscreen, the window title shows FPS via `Framerate.CalculateFrameRate()`, and `Camera` runs in `Freeview` (arrow keys move/strafe, A/D roll, mouse pitch/yaw, 45° FOV, near/far clip 1-2000). Skybox, terrain, and XACT `Sound.InitializeEngine` calls are commented out, so `Draw` currently clears to black; `Test.vb` (a textured quad that loads `Content\Textures\Grass`) is in the folder but not listed in `XNAEngine.vbproj`. Nested `ContentPipeline` is a VB.NET console helper that writes a temporary XNA Game Studio Express 1.0 `Content_Temp.csproj` and shells MSBuild v2.0 to compile listed sky/heightmap/.fx assets to `.xnb` - those source textures and effects are not in the tree.

**Source last updated:** 2009-07-12 · **Language:** VB.NET · **Target:** .NET Framework (VS 2008 ToolsVersion 3.5 / ProductVersion 9.0.30729, upgraded from ToolsVersion 2.0; no `TargetFrameworkVersion`) + Microsoft XNA Framework 3.1 (`Microsoft.Xna.Framework` / `Microsoft.Xna.Framework.Game` 3.1.0.0) · **Output:** WinExe (`XNAEngine`) hosting an XNA `Game`, plus console exe (`ContentPipeline`)

## Solution structure

| Project | Language | Type | Purpose |
|---------|----------|------|---------|
| `XNAEngine` (`XNAEngine/XNAEngine.vbproj`) | VB.NET | WinExe (`Sub Main` → `XNAEngine.Run()`) | XNA 3.1 game host in namespace `XNA`: `Camera`, `SkyBoxClass`, `TerrainClass` (heightmap), `ModelClass`, `TextureClass`, `Sound` (XACT), `WriteText` (GDI+ bitmap → `Texture2D`), `Framerate`. |
| `ContentPipeline` (`XNAEngine/ContentPipeline/ContentPipeline/ContentPipeline.vbproj`) | VB.NET | Console exe (`StartupObject` `ContentPipeline.Asset`) | Lists sky/heightmap/effect assets, emits `Content_Temp.csproj` (`XnaFrameworkVersion` v1.0), and runs `%Windir%\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe`. Nested `ContentPipeline.sln` is Visual Basic Express 2005 (format 9.00). |

`Backup/` is the VS 2008 conversion copy from 12 July 2009 (no `Test.vb`). Keep `_UpgradeReport_Files` as conversion provenance. `UpgradeLog.XML` is gitignored (Windows username); a redacted `UpgradeLog.XML.example` is committed.

## How to open

Open `XNAEngine.sln` in Visual Studio 2008 or later (solution format 10.00) with XNA Game Studio 3.1 installed. The engine project references `Microsoft.Xna.Framework` and `Microsoft.Xna.Framework.Game` version 3.1.0.0. `ContentPipeline` still targets the Game Studio Express 1.0 content pipeline (`Microsoft.Xna.ContentPipeline.targets`) and does not reference the XNA 3.1 assemblies.

Microsoft XNA Framework is third-party Microsoft software. It is referenced only; no XNA runtime, pipeline DLLs, or Microsoft sample assets are bundled in this tree.

## Requirements

- Visual Studio 2005 to 2008

## Attribution and provenance

Working copy from Dave Robinson's OneDrive Historical Dev folder `XNAEngine`. Both assemblies: `AssemblyCompany` Hirogen, `AssemblyCopyright` Copyright © Hirogen 2006, `AssemblyVersion` 1.0.0.0. Engine `AssemblyTitle` / `AssemblyProduct` XNAGame; pipeline `AssemblyTitle` / `AssemblyProduct` ContentPipeline. `RootNamespace` / `AssemblyName` XNAEngine. Startup object is `Sub Main` (`MyType` WindowsFormsWithCustomSubMain; `Application.myapp` still lists `MainForm` Form1). VS conversion log dated Sunday, 12 July 2009 11:40 AM (see `UpgradeLog.XML.example`).

## License

MIT © 2026 VaderConsulting for Dave Robinson's code. See `LICENSE`. Microsoft XNA Framework (XNA Game Studio / Game Studio Express) is third-party Microsoft software and is not included in this repository.
