Imports System.Windows.Forms
Imports System.IO


Public Class Asset

#Region "Objects and Variables"
    'The Array to hold all assets that are to be converted to xnb files
    Public Shared AssetArray() As Asset = New Asset() {}

    'Global Asset Varables - Output folder for xnb files
    'If left as the default, all xnb files will be created in the same folder as its corresponding asset.
    'If this is changed the asset's folder structure will be created in the VB project folder.
    'So if an asset is located at Images\Folder\asset.jpg, then the corresponding xnb file will be located at
    'AssetOutputFolder\Images\Folder\asset.xnb
    Public Shared AssetOutputFolder As String = "Content\" 'default value is ".\"
    ' The static readonly path to the folder that contains the VB project file vbproj
    Public Shared ReadOnly vbprojFolder As String = Microsoft.VisualBasic.Left(Application.StartupPath, Len(Application.StartupPath) - 42)

    'Asset Specific variables, that differ for each asset
    Public AssetPath As String = "" ' An asset's Path name relative to the program executable
    Public AssetType As AssetEnum ' The type of asset
    Public AssetName As String = "" ' The name of the asset without the file extension.
    Public AssetFileName As String = "" ' The name of the asset with the file extension.
#End Region

    'The list of possible assets
    Public Enum AssetEnum As Integer
        Texture_Sprite32bpp = 1
        Texture_ModelDXTmipmapped = 2
        Texture_Mipmapped = 3
        Effect_fx = 4
        Model_x = 5
        Model_fbx = 6
        Sound_XACT = 7
    End Enum

    ''' <summary>
    ''' Any Asset to be added to the content pipeline should be listed in this sub. Example:
    ''' Asset.AddAssettoContentPipeline("Textures\Texture.png", Asset.AssetEnum.Texture_Sprite32bpp).
    ''' The filepath starts at the project folder.
    ''' </summary>
    Public Shared Sub AssetList()
        Asset.AddAssettoContentPipeline("Textures\skyfront.jpg", AssetEnum.Texture_ModelDXTmipmapped)
        Asset.AddAssettoContentPipeline("Textures\skyback.jpg", AssetEnum.Texture_ModelDXTmipmapped)
        Asset.AddAssettoContentPipeline("Textures\skyup.jpg", AssetEnum.Texture_ModelDXTmipmapped)
        Asset.AddAssettoContentPipeline("Textures\skydown.jpg", AssetEnum.Texture_ModelDXTmipmapped)
        Asset.AddAssettoContentPipeline("Textures\skyleft.jpg", AssetEnum.Texture_ModelDXTmipmapped)
        Asset.AddAssettoContentPipeline("Textures\skyright.jpg", AssetEnum.Texture_ModelDXTmipmapped)
        Asset.AddAssettoContentPipeline("Textures\HeightMaps\HeightMap128.jpg", AssetEnum.Texture_ModelDXTmipmapped)
        Asset.AddAssettoContentPipeline("Textures\texturenotfound.jpg", AssetEnum.Texture_ModelDXTmipmapped)
        Asset.AddAssettoContentPipeline("Textures\grass.jpg", AssetEnum.Texture_ModelDXTmipmapped)
        Asset.AddAssettoContentPipeline("Textures\dirt.jpg", AssetEnum.Texture_ModelDXTmipmapped)
        Asset.AddAssettoContentPipeline("Textures\snow.jpg", AssetEnum.Texture_ModelDXTmipmapped)
        Asset.AddAssettoContentPipeline("Textures\rock.jpg", AssetEnum.Texture_ModelDXTmipmapped)
        Asset.AddAssettoContentPipeline("Textures\cement.jpg", AssetEnum.Texture_ModelDXTmipmapped)
        Asset.AddAssettoContentPipeline("Effects\skyboxeffect.fx", AssetEnum.Effect_fx)
        Asset.AddAssettoContentPipeline("Effects\standardeffects.fx", AssetEnum.Effect_fx)
    End Sub

    ''' <summary>
    ''' Add an asset to the content pipeline.
    ''' </summary>
    ''' <param name="AssetPathName">The path to the asset file, relative to the VB project file.
    ''' A jpeg in the same folder as the project .vbproj file would be: "asset.jpg", 
    ''' the same file inside a folder called images, where this folder is at the same level as the VB project file would be
    ''' "images\asset.jpg"</param>
    ''' <param name="AssetType1">The kind of asset to be added:
    ''' Texture_Mipmapped - Low Compression, Texture_Sprite32bpp - Medium Compression, Texture_ModelDXTmipmapped - High Compression
    ''' Texture_Sprite32bpp is the default.
    ''' Important Note: Many graphics cards do not support mipmapped textures that are not sized as a power of 2.
    ''' Also, For a texture to use DXT compression it's size must be dividable by 4. 
    ''' There is only one option for an fx file.</param>
    ''' <returns>True if the asset was successfully added to the asset array, otherwise False. Will fail if an 
    ''' asset with the same name already exists in the array.</returns>
    Public Shared Function AddAssettoContentPipeline(ByVal AssetPathName As String, ByVal AssetType1 As AssetEnum)
        ' A couple of string to hold the file name with and without the file extension
        Dim FileNameWithExt As String = Mid(AssetPathName, InStrRev(AssetPathName, Chr(92)) + 1)
        Dim FileNameWithoutExt As String = Microsoft.VisualBasic.Left(FileNameWithExt, InStrRev(FileNameWithExt, Chr(46)) - 1)

        'Loop through the asset array to see if a asset of the same name already exists
        For Each Asset1 As Asset In AssetArray
            If Asset1.AssetName = FileNameWithoutExt Then
                ' If an asset of the same name already exists in the asset array, the next one
                ' will not be added and the sub will exit
                MsgBox("A duplicate entry for Asset name: " & Asset1.AssetName & " already exists in the asset array" & vbCrLf & _
                "Please rename one of the assets.")
                Return False
            End If
        Next

        'Create a temporary asset and assign the required variables
        Dim TempAsset As New Asset
        TempAsset.AssetPath = AssetPathName
        TempAsset.AssetName = FileNameWithoutExt
        TempAsset.AssetType = AssetType1
        TempAsset.AssetFileName = FileNameWithExt

        'Increase the size of the asset array and add the new asset to it
        ReDim Preserve AssetArray(UBound(AssetArray) + 1)
        AssetArray(UBound(AssetArray)) = TempAsset

        'release variables
        TempAsset = Nothing
        FileNameWithExt = Nothing
        FileNameWithoutExt = Nothing

        Return True
    End Function

    ''' <summary>
    ''' This is the start point for the program, which iin turn runs the AssetList Sub
    ''' </summary>
    Public Shared Sub Main()

        'Add assets to asset array
        Asset.AssetList()

        'Make sure that the asset array is valid
        If Asset.AssetArray IsNot Nothing Then
            'Make sure that it has at least one entry
            If UBound(Asset.AssetArray) > -1 Then
                Try
                    'Try to create the C# project file tat MSBuild needs to create the asset binaries
                    Using sw As StreamWriter = New StreamWriter(Asset.vbprojFolder & "\Content_Temp.csproj", False)
                        sw.WriteLine("<Project DefaultTargets=" & Chr(34) & "Build" & Chr(34) & " xmlns=" & Chr(34) & "http://schemas.microsoft.com/developer/msbuild/2003" & Chr(34) & ">")
                        sw.WriteLine("<PropertyGroup>")
                        sw.WriteLine("<Configuration Condition=" & Chr(34) & " '$(Configuration)' == '' " & Chr(34) & ">Debug</Configuration>")
                        sw.WriteLine("<Platform Condition=" & Chr(34) & " '$(Platform)' == '' " & Chr(34) & ">x86</Platform>")
                        sw.WriteLine("<XnaFrameworkVersion>v1.0</XnaFrameworkVersion>")
                        sw.WriteLine("<XnaPlatform>Windows</XnaPlatform>")
                        sw.WriteLine("<XNAGlobalContentPipelineAssemblies>Microsoft.Xna.Framework.Content.Pipeline.EffectImporter.dll;Microsoft.Xna.Framework.Content." & _
                        "Pipeline.FBXImporter.dll;Microsoft.Xna.Framework.Content.Pipeline.TextureImporter.dll;Microsoft.Xna.Framework.Content.Pipeline.XImporter.dll" & _
                        "</XNAGlobalContentPipelineAssemblies> </PropertyGroup>")
                        sw.WriteLine("<PropertyGroup Condition=" & Chr(34) & " '$(Configuration)|$(Platform)' == 'Debug|x86' " & Chr(34) & ">")
                        sw.WriteLine("<OutputPath>" & Asset.AssetOutputFolder & "</OutputPath>")
                        sw.WriteLine("</PropertyGroup>")
                        sw.WriteLine("<ItemGroup> ")

                        'Add data to the csproj files for each asset in the asset array
                        For Each TempAsset As Asset In Asset.AssetArray
                            'Sort assetpath so that it does not have a \ or / at the start
                            If Mid(TempAsset.AssetPath, 1, 1) = Chr(92) Or Mid(TempAsset.AssetPath, 1, 1) = Chr(47) Then
                                TempAsset.AssetPath = Mid(TempAsset.AssetPath, 2)
                            End If
                            sw.WriteLine("<Content Include=" & Chr(34) & TempAsset.AssetPath & Chr(34) & ">")
                            sw.WriteLine("<XNAUseContentPipeline>true</XNAUseContentPipeline>")
                            Select Case TempAsset.AssetType
                                Case Is = Asset.AssetEnum.Texture_Mipmapped
                                    sw.WriteLine("<Importer>TextureImporter</Importer>")
                                    sw.WriteLine("<Processor>TextureProcessor</Processor>")
                                Case Is = Asset.AssetEnum.Texture_ModelDXTmipmapped
                                    sw.WriteLine("<Importer>TextureImporter</Importer>")
                                    sw.WriteLine("<Processor>ModelTextureProcessor</Processor>")
                                Case Is = Asset.AssetEnum.Texture_Sprite32bpp
                                    sw.WriteLine("<Importer>TextureImporter</Importer>")
                                    sw.WriteLine("<Processor>SpriteTextureProcessor</Processor>")
                                Case Is = AssetEnum.Effect_fx
                                    sw.WriteLine("<Importer>EffectImporter</Importer>")
                                    sw.WriteLine("<Processor>EffectProcessor</Processor>")
                                Case Is = AssetEnum.Model_x
                                    sw.WriteLine("<Importer>XImporter</Importer>")
                                    sw.WriteLine("<Processor>ModelProcessor</Processor>")
                                Case Is = AssetEnum.Model_fbx
                                    sw.WriteLine("<Importer>FbxImporter</Importer>")
                                    sw.WriteLine("<Processor>ModelProcessor</Processor>")
                                Case Is = AssetEnum.Sound_XACT
                                    sw.WriteLine("<Importer>XactImporter</Importer>")
                                    sw.WriteLine("<Processor>XactProcessor</Processor>")
                            End Select
                            sw.WriteLine("<Name>" & TempAsset.AssetName & "</Name>")
                            sw.WriteLine("</Content>")
                        Next
                        sw.WriteLine("</ItemGroup>")
                        sw.WriteLine("<Import Project=" & Chr(34) & "$(MSBuildBinPath)\Microsoft.CSharp.targets" & Chr(34) & " />")
                        sw.WriteLine("<Import Project=" & Chr(34) & "$(MSBuildExtensionsPath)\Microsoft\XNA\Game Studio Express\v1.0\Microsoft.Xna.ContentPipeline.targets" & Chr(34) & " />")
                        sw.WriteLine("</Project>")
                        sw.Close()
                    End Using
                Catch ex As Exception
                    'If the file creation fails then show this message box and exit the sub
                    MsgBox("While creating Content_Temp.csproj, the following error was raised:" & vbCrLf & ex.Message)
                    Exit Sub
                End Try

                ' Run MSBuild and create the asset binaries
                System.Diagnostics.Process.Start(Environment.GetEnvironmentVariable("Windir") & _
                "\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe", "/nologo " & Chr(34) & Asset.vbprojFolder & _
                "\Content_Temp.csproj" & Chr(34) & " /l:FileLogger,Microsoft.Build.Engine;logfile=" & _
                Chr(34) & Asset.vbprojFolder & "\AssetBuild.log" & Chr(34))

            End If
        End If
    End Sub

End Class