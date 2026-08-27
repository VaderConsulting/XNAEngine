
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Content
Imports Microsoft.Xna.Framework.Content.Pipeline
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input

'All game components will be held inside the XNA NameSpace
Namespace XNA
    'The main game component  will be called XNA GAme
    Public Class XNAEngine
        'Which includes the Microsoft.Xna.Framework.game Template
        Inherits Game

#Region "objects and Variables"
        Public Shared XNAGraphics As GraphicsDeviceManager ' An instance of the Graphics Device manager
        Public Shared XNAContentManager As ContentManager ' An Instance of the Content manager

        'Private SkyBox As New SkyBoxClass ' The Skybox
        'Private Terrain As New TerrainClass  ' The Terrain

#End Region

#Region "Subs and Functions"

        ' When an instance of the XNAGame class is created using the New keyword, this Sub runs.
        Public Sub New()
            XNAGraphics = New GraphicsDeviceManager(Me)
            XNAGraphics.IsFullScreen = False    'Set the window to fullscreen
            XNAContentManager = New ContentManager(Services)

            'Add an event handler that runs when the window is resized
            AddHandler MyBase.Window.ClientSizeChanged, New EventHandler(AddressOf Me.OnGameWindowResized)
            'Add an event handler that runs when the device is reset
            AddHandler XNAGraphics.DeviceReset, New EventHandler(AddressOf Me.OnDeviceReset)

        End Sub

        'the following code runs when the window is resized
        Private Sub OnGameWindowResized(ByVal sender As Object, ByVal e As EventArgs)

        End Sub

        'the following code runs when the window is resized
        Private Sub OnDeviceReset(ByVal sender As Object, ByVal e As EventArgs)
            'Re-initialize the skybox
            'SkyBox.OnDeviceReset()
        End Sub

        'Initializes the XNAGame class instance, runs once when game starts
        Protected Overrides Sub Initialize()

            MyBase.Initialize() 'comes first, because this loads LoadGraphicsContent below
            'Initialize Camera First
            Camera.Initialize()
            'Then Initialize Skybox
            'SkyBox.Initialize("SkyUp", "SkyDown", "SkyRight", "SkyLeft", "SkyFront", "SkyBack", 6.0F, "Basic", True)
            'Then the rest

         '   Terrain.Initialize("Textures\HeightMaps\HeightMap128.jpg", _
          'TerrainClass.Effecttype.Basic, 6.0, TerrainClass.VertexType.Colored, _
         '"Content\Effects\standardeffects", "Textured", New String() {"Content\Textures\dirt", _
         '"Content\Textures\grass", "Content\Textures\rock", "Content\Textures\snow"})

        End Sub

        'Load All Graphics Content XNB files
        Protected Overrides Sub LoadGraphicsContent(ByVal LoadAllContent As Boolean)
            If LoadAllContent = True Then

            End If
        End Sub

        'Unload All Graphics Content
        Protected Overrides Sub UnloadGraphicsContent(ByVal UnloadAllContent As Boolean)
            If UnloadAllContent = True Then
                XNAContentManager.Unload()
            End If
        End Sub

        'Updates all game components, runs before every Draw Loop
        Protected Overrides Sub Update(ByVal gameTime As GameTime)

            'Find what keys are presed and save it in GetKeys
            Dim GetKeys As KeyboardState = Keyboard.GetState
            Dim GetMouse As MouseState = Mouse.GetState

            'Exit the game with Escape
            If GetKeys.IsKeyDown(Keys.Escape) Then
                MyBase.Exit()
            End If

            'Change from Windowed to Fullscreen
            If GetKeys.IsKeyDown(Keys.F1) Then
                XNAGraphics.ToggleFullScreen()
            End If

            'Update the Camera
            Camera.Update(GetKeys, GetMouse, Camera.CameraTypeEnum.Freeview) ' Update the camera position, view, etc
            'assign the previous mouse position
            Camera.PreviousMousePosX = GetMouse.X
            Camera.PreviousMousePosY = GetMouse.Y
            'Update the skybox
            'If Camera.SkyboxUpdateRequired = True Then
             '   SkyBox.Update(True)
              '  Camera.SkyboxUpdateRequired = False ' Reset SkyboxUpdate value
            'End If

            'Add you game code here

            'Show the game framerate in the title bar
            Me.Window.Title = String.Format("The framerate is {0}", Framerate.CalculateFrameRate())
            
            '  Sound.UpdateEngine()
            MyBase.Update(gameTime)
            Application.DoEvents() ' Lets windows do its stuff
        End Sub

        'Renders any Backbuffer draw data to the screen
        Protected Overrides Sub Draw(ByVal gameTime As GameTime)
            XNAGraphics.GraphicsDevice.Clear(Color.Black)

            XNAGraphics.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace
            'XNAGraphics.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame
           
            'Draw Camera First
            Camera.Draw()
            'Then Draw Skybox
            'SkyBox.Draw()
            'Then draw the rest
            'Terrain.Draw()

            MyBase.Draw(gameTime)
        End Sub

#End Region

#Region "Properties"

#End Region

    End Class

#Region "Entry Point for XNAGame"
    'The starting point for the game
    Module modMain
        'An instance of the XNAGame Class
        Public XNAGame As XNAEngine

        'The location of the projhect folder, which contains the .vbproj file
        Public ReadOnly XNAGameProjectFolder As String = Mid(Mid(Application.StartupPath, 1, _
        InStrRev(Application.StartupPath, "\") - 1), 1, InStrRev(Mid(Application.StartupPath, 1, _
        InStrRev(Application.StartupPath, "\") - 1), "\"))

        'The starting point of the game is Main() because it is ot a windows form
        Public Sub Main(ByVal args As String())
            XNAGame = New XNAEngine
            XNAGame.Run()
        End Sub

    End Module
#End Region

End Namespace

