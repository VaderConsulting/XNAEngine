
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace XNA
    Public Class SkyBoxClass

#Region "Objects and Variables"

        'The 6 textureclass objects that form the skybox
        Private SkyUp As New TextureClass
        Private SkyDown As New TextureClass
        Private SkyRight As New TextureClass
        Private SkyLeft As New TextureClass
        Private SkyFront As New TextureClass
        Private SkyBack As New TextureClass

        'An Array to hold the textureclasses
        Private TextureClassArray() As TextureClass

        'There are 8 vertices that will be used to make the skybox. The Vertices are created as if you were 
        'standing in the centre of the cude at vector3 = 0,0,0 looking in the positive z direction, which is out 
        'of the screen towards you.
        Private SkyUpVertexBackLeft As Vector3
        Private SkyUpVertexFrontLeft As Vector3
        Private SkyUpVertexFrontRight As Vector3
        Private SkyUpVertexBackRight As Vector3
        Private SkyDownVertexBackLeft As Vector3
        Private SkyDownVertexFrontLeft As Vector3
        Private SkyDownVertexFrontRight As Vector3
        Private SkyDownVertexBackRight As Vector3

        'Used to determine whether the skybox will use a custom or basic effect
        Private EffectType As String = "Basic"

        'Does the skybox move synchronously with the camaera position
        Private LockSkyboxIntoCameraPosition As Boolean = False
        Private SkyBoxOrigin As New Vector3(0.0F, 0.0F, 0.0F) ' used only if LockSkyboxIntoCameraposition = True

        'A Few Temporary Variables that are not disposed, so that the garbage collector does not run
        Private TempInt As Integer = 0
        Private TempTextureClass As TextureClass

#End Region

#Region "Subs and Functions"

        ''' <summary>
        ''' Initializes an instance of the skybox. Skybox dimensions are automatically created.
        ''' Each texture name is a string value and points to the location of the texture. The Texture will be 
        ''' loaded/initialized as part of the Sub. Example: "SkyUp" refers to a texture called SkyUp.xnb in the 
        ''' content\textures folder Example 2: "Skies\SkyUp" refers to a texture called SkyUp.xnb in the 
        ''' content\textures\skies folder. The dimensions of the skybox will be determined by the dimension of the 
        ''' SkyUp and SkyFront textures multiplied by the value given for DimensionMultiplier.
        '''</summary>
        ''' <param name="strSkyUp">The name of the SkyUp texture.</param>
        ''' <param name="strSkyDown">The name of the SkyDown texture.</param>
        ''' <param name="strSkyRight">The name of the SkyRight texture.</param>
        ''' <param name="strSkyLeft">The name of the SkyLeft texture.</param>
        ''' <param name="strSkyFront">The name of the SkyFront texture.</param>
        ''' <param name="strSkyBack">The name of the SkyBack texture.</param>
        ''' <param name="DimensionMultiplier">The floating point value that the texture dimensions will be multiplied by to
        ''' set the skybox size. Example, a value of 2.0 and a texture length of 1024 will make a skybox length of 2048. 
        ''' If the value entered is less than 0 then the value will be set to 1.</param>
        ''' <param name="thisEffectType">The type of effect used to display the skybox, choices are "Custom" or "Basic".</param>
        ''' <param name="LockSkyBox">If True the skybox will move in synch with the camera position.</param>
        Public Sub Initialize(ByVal strSkyUp As String, ByVal strSkyDown As String, ByVal strSkyRight As String, _
        ByVal strSkyLeft As String, ByVal strSkyBack As String, ByVal strSkyFront As String, ByVal DimensionMultiplier As Single, _
        ByVal thisEffectType As String, ByVal LockSkyBox As Boolean)

            'make sure that DimensionMultiplier is a single value 
            DimensionMultiplier = CSng(DimensionMultiplier)

            'Make sure Dimensionmultiplier is bigger than 0
            If DimensionMultiplier <= 0 Then DimensionMultiplier = 1

            'Check that thisEffectType has the proper value
            If Not thisEffectType = "Basic" AndAlso Not thisEffectType = "Custom" Then
                Throw New Exception
                Exit Sub
            End If

            'Set the effecttype
            EffectType = thisEffectType

            'Check to see if any of the texture names have the .xnb file extension included.
            If Microsoft.VisualBasic.Right(strSkyUp, 4) = ".xnb" Then
                strSkyUp = Mid(strSkyUp, 1, Len(strSkyUp) - 4)
            End If
            If Microsoft.VisualBasic.Right(strSkyDown, 4) = ".xnb" Then
                strSkyDown = Mid(strSkyDown, 1, Len(strSkyDown) - 4)
            End If
            If Microsoft.VisualBasic.Right(strSkyRight, 4) = ".xnb" Then
                strSkyRight = Mid(strSkyRight, 1, Len(strSkyRight) - 4)
            End If
            If Microsoft.VisualBasic.Right(strSkyLeft, 4) = ".xnb" Then
                strSkyLeft = Mid(strSkyLeft, 1, Len(strSkyLeft) - 4)
            End If
            If Microsoft.VisualBasic.Right(strSkyFront, 4) = ".xnb" Then
                strSkyFront = Mid(strSkyFront, 1, Len(strSkyFront) - 4)
            End If
            If Microsoft.VisualBasic.Right(strSkyBack, 4) = ".xnb" Then
                strSkyBack = Mid(strSkyBack, 1, Len(strSkyBack) - 4)
            End If

            Try
                'assign the textures
                SkyUp.TheTextureFile = XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & "Content\Textures\" & strSkyUp)
                SkyDown.TheTextureFile = XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & "Content\Textures\" & strSkyDown)
                SkyRight.TheTextureFile = XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & "Content\Textures\" & strSkyRight)
                SkyLeft.TheTextureFile = XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & "Content\Textures\" & strSkyLeft)
                SkyFront.TheTextureFile = XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & "Content\Textures\" & strSkyFront)
                SkyBack.TheTextureFile = XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & "Content\Textures\" & strSkyBack)

                'Initialize teh array
                TextureClassArray = New TextureClass() {SkyUp, SkyDown, SkyRight, SkyLeft, SkyFront, SkyBack}

                For TempInt = 0 To UBound(TextureClassArray)
                    If TextureClassArray(TempInt).TheTextureFile Is Nothing Then
                        TextureClassArray(TempInt).TheTextureFile = XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & "Content\Textures\texturenotfound")
                    End If
                Next
            Catch ex As Exception
                'MsgBox(ex.Message)
            End Try

            'Determine and assign the postions of the vertices.
            SkyUpVertexBackLeft = New Vector3((-(SkyUp.TheTextureFile.Width / 2) * DimensionMultiplier), ((SkyFront.TheTextureFile.Width / 2) * DimensionMultiplier), (-(SkyUp.TheTextureFile.Height / 2) * DimensionMultiplier))
            SkyUpVertexFrontLeft = New Vector3((-(SkyUp.TheTextureFile.Width / 2) * DimensionMultiplier), ((SkyFront.TheTextureFile.Width / 2) * DimensionMultiplier), ((SkyUp.TheTextureFile.Height / 2) * DimensionMultiplier))
            SkyUpVertexFrontRight = New Vector3(((SkyUp.TheTextureFile.Width / 2) * DimensionMultiplier), ((SkyFront.TheTextureFile.Width / 2) * DimensionMultiplier), ((SkyUp.TheTextureFile.Height / 2) * DimensionMultiplier))
            SkyUpVertexBackRight = New Vector3(((SkyUp.TheTextureFile.Width / 2) * DimensionMultiplier), ((SkyFront.TheTextureFile.Width / 2) * DimensionMultiplier), (-(SkyUp.TheTextureFile.Height / 2) * DimensionMultiplier))
            SkyDownVertexBackLeft = New Vector3((-(SkyUp.TheTextureFile.Width / 2) * DimensionMultiplier), (-(SkyFront.TheTextureFile.Width / 2) * DimensionMultiplier), (-(SkyUp.TheTextureFile.Height / 2) * DimensionMultiplier))
            SkyDownVertexFrontLeft = New Vector3((-(SkyUp.TheTextureFile.Width / 2) * DimensionMultiplier), (-(SkyFront.TheTextureFile.Width / 2) * DimensionMultiplier), ((SkyUp.TheTextureFile.Height / 2) * DimensionMultiplier))
            SkyDownVertexFrontRight = New Vector3(((SkyUp.TheTextureFile.Width / 2) * DimensionMultiplier), (-(SkyFront.TheTextureFile.Width / 2) * DimensionMultiplier), ((SkyUp.TheTextureFile.Height / 2) * DimensionMultiplier))
            SkyDownVertexBackRight = New Vector3(((SkyUp.TheTextureFile.Width / 2) * DimensionMultiplier), (-(SkyFront.TheTextureFile.Width / 2) * DimensionMultiplier), (-(SkyUp.TheTextureFile.Height / 2) * DimensionMultiplier))

            'Should the skybox move in synch with the camera position
            LockSkyboxIntoCameraPosition = LockSkyBox
            SkyBoxOrigin = Camera.CameraPosition 'The initial position of the skybox origin

            'Set the cameraview to the diameter of the skybox
            Camera.FarClip = Vector3.Distance(SkyUpVertexBackLeft, SkyDownVertexFrontRight)

            Select Case EffectType
                Case Is = "Basic"
                    'Initialize the textures
                    'Setup the Temporary Vertex array 
                    SkyUp.VertexArray = New Vector3() {SkyUpVertexBackLeft, SkyUpVertexFrontLeft, SkyUpVertexFrontRight, SkyUpVertexBackRight}
                    SkyUp.Initialize(SkyUp.TheTextureFile, Nothing, Nothing, "SkyUp", thisEffectType)
                    'Redo the vertex array for the next texture, no need to change the U and V corrs because they are the
                    'same for all textures
                    SkyDown.VertexArray = New Vector3() {SkyDownVertexFrontLeft, SkyDownVertexBackLeft, SkyDownVertexBackRight, SkyDownVertexFrontRight}
                    SkyDown.Initialize(SkyDown.TheTextureFile, Nothing, Nothing, "SkyDown", thisEffectType)
                    SkyRight.VertexArray = New Vector3() {SkyDownVertexBackRight, SkyUpVertexBackRight, SkyUpVertexFrontRight, SkyDownVertexFrontRight}
                    SkyRight.Initialize(SkyRight.TheTextureFile, Nothing, Nothing, "SkyRight", thisEffectType)
                    SkyLeft.VertexArray = New Vector3() {SkyDownVertexFrontLeft, SkyUpVertexFrontLeft, SkyUpVertexBackLeft, SkyDownVertexBackLeft}
                    SkyLeft.Initialize(SkyLeft.TheTextureFile, Nothing, Nothing, "SkyLeft", thisEffectType)
                    SkyBack.VertexArray = New Vector3() {SkyDownVertexBackLeft, SkyUpVertexBackLeft, SkyUpVertexBackRight, SkyDownVertexBackRight}
                    SkyBack.Initialize(SkyBack.TheTextureFile, Nothing, Nothing, "SkyBack", thisEffectType)
                    SkyFront.VertexArray = New Vector3() {SkyDownVertexFrontRight, SkyUpVertexFrontRight, SkyUpVertexFrontLeft, SkyDownVertexFrontLeft}
                    SkyFront.Initialize(SkyFront.TheTextureFile, Nothing, Nothing, "SkyFront", thisEffectType)

                    'Setup the basic effect for each skybox texture
                    For Each TempTextureClass In TextureClassArray
                        TempTextureClass.TheBasicEffect.GraphicsDevice.SamplerStates(0).AddressU = TextureAddressMode.Clamp
                        TempTextureClass.TheBasicEffect.GraphicsDevice.SamplerStates(0).AddressV = TextureAddressMode.Clamp
                        TempTextureClass.TheBasicEffect.GraphicsDevice.RenderState.DepthBufferEnable = False
                        TempTextureClass.TheBasicEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace
                    Next

                Case Is = "Custom"
                    'Initialize the textures
                    'Setup the Temporary Vertex array 
                    SkyUp.VertexArray = New Vector3() {SkyUpVertexBackLeft, SkyUpVertexFrontLeft, SkyUpVertexFrontRight, SkyUpVertexBackRight}
                    SkyUp.Initialize(SkyUp.TheTextureFile, "skyboxeffect", "SkyBox", "SkyUp", thisEffectType)
                    'Redo the vertex array for the next texture, no need to change the U and V corrs because they are the
                    'same for all textures
                    SkyDown.VertexArray = New Vector3() {SkyDownVertexFrontLeft, SkyDownVertexBackLeft, SkyDownVertexBackRight, SkyDownVertexFrontRight}
                    SkyDown.Initialize(SkyDown.TheTextureFile, "skyboxeffect", "SkyBox", "SkyDown", thisEffectType)
                    SkyRight.VertexArray = New Vector3() {SkyDownVertexBackRight, SkyUpVertexBackRight, SkyUpVertexFrontRight, SkyDownVertexFrontRight}
                    SkyRight.Initialize(SkyRight.TheTextureFile, "skyboxeffect", "SkyBox", "SkyRight", thisEffectType)
                    SkyLeft.VertexArray = New Vector3() {SkyDownVertexFrontLeft, SkyUpVertexFrontLeft, SkyUpVertexBackLeft, SkyDownVertexBackLeft}
                    SkyLeft.Initialize(SkyLeft.TheTextureFile, "skyboxeffect", "SkyBox", "SkyLeft", thisEffectType)
                    SkyBack.VertexArray = New Vector3() {SkyDownVertexBackLeft, SkyUpVertexBackLeft, SkyUpVertexBackRight, SkyDownVertexBackRight}
                    SkyBack.Initialize(SkyBack.TheTextureFile, "skyboxeffect", "SkyBox", "SkyBack", thisEffectType)
                    SkyFront.VertexArray = New Vector3() {SkyDownVertexFrontRight, SkyUpVertexFrontRight, SkyUpVertexFrontLeft, SkyDownVertexFrontLeft}
                    SkyFront.Initialize(SkyFront.TheTextureFile, "skyboxeffect", "SkyBox", "SkyFront", thisEffectType)

            End Select

        End Sub

        ''' <summary>
        ''' Initializes an instance of the skybox. Skybox dimensions are manually entered.
        ''' Each texture name is a string value and points to the location 
        ''' of the texture. The Texture will be loaded/initialized as part of the Sub.
        ''' Example: "SkyUp" refers to a texture called SkyUp.xnb in the content\textures folder
        ''' Example 2: "Skies\SkyUp" refers to a texture called SkyUp.xnb in the content\textures\skies folder.
        ''' The dimensions of the skybox will be determined by manually entering a vector3 for each of the 8 vertices.
        '''</summary>
        ''' <param name="strSkyUp">The name of the SkyUp texture.</param>
        ''' <param name="strSkyDown">The name of the SkyDown texture.</param>
        ''' <param name="strSkyRight">The name of the SkyRight texture.</param>
        ''' <param name="strSkyLeft">The name of the SkyLeft texture.</param>
        ''' <param name="strSkyFront">The name of the SkyFront texture.</param>
        ''' <param name="strSkyBack">The name of the SkyBack texture.</param>
        ''' <param name="SkyDownBackLeft">The Vector3 coord of the SkyBox Vertex at (-X,-Y,-Z), 
        ''' using the XYZ system where +X = Right, +Y = Up, +Z = towards you. Origin = New Vector3(0,0,0)</param>
        ''' <param name="SkyDownBackRight">The Vector3 coord of the SkyBox Vertex at (+X,-Y,-Z), 
        ''' using the XYZ system where +X = Right, +Y = Up, +Z = towards you. Origin = New Vector3(0,0,0)</param>
        ''' <param name="SkyDownFrontLeft">The Vector3 coord of the SkyBox Vertex at (-X,-Y,+Z), 
        ''' using the XYZ system where +X = Right, +Y = Up, +Z = towards you. Origin = New Vector3(0,0,0)</param>
        ''' <param name="SkyDownFrontRight">The Vector3 coord of the SkyBox Vertex at (+X,-Y,+Z), 
        ''' using the XYZ system where +X = Right, +Y = Up, +Z = towards you. Origin = New Vector3(0,0,0)</param>
        ''' <param name="SkyUpBackLeft">The Vector3 coord of the SkyBox Vertex at (-X,+Y,-Z), 
        ''' using the XYZ system where +X = Right, +Y = Up, +Z = towards you. Origin = New Vector3(0,0,0)</param>
        ''' <param name="SkyUpBackRight">The Vector3 coord of the SkyBox Vertex at (+X,+Y,-Z), 
        ''' using the XYZ system where +X = Right, +Y = Up, +Z = towards you. Origin = New Vector3(0,0,0)</param>
        ''' <param name="SkyUpFrontLeft">The Vector3 coord of the SkyBox Vertex at (-X,+Y,+Z), 
        ''' using the XYZ system where +X = Right, +Y = Up, +Z = towards you. Origin = New Vector3(0,0,0)</param>
        ''' <param name="SkyUpFrontRight">The Vector3 coord of the SkyBox Vertex at (+X,+Y,+Z), 
        ''' using the XYZ system where +X = Right, +Y = Up, +Z = towards you. Origin = New Vector3(0,0,0)</param>
        ''' <param name="thisEffectType">The type of effect used to display the skybox, choices are "Custom" or "Basic".</param>
        ''' <param name="LockSkyBox">If True the skybox will move in synch with the camera position.</param>
        Public Sub Initialize(ByVal strSkyUp As String, ByVal strSkyDown As String, ByVal strSkyRight As String, _
        ByVal strSkyLeft As String, ByVal strSkyFront As String, ByVal strSkyBack As String, ByVal SkyUpBackLeft As Vector3, _
        ByVal SkyUpFrontLeft As Vector3, ByVal SkyUpFrontRight As Vector3, ByVal SkyUpBackRight As Vector3, _
        ByVal SkyDownBackLeft As Vector3, ByVal SkyDownFrontLeft As Vector3, ByVal SkyDownFrontRight As Vector3, _
        ByVal SkyDownBackRight As Vector3, ByVal thisEffectType As String, ByVal LockSkyBox As Boolean)

            'Check that thisEffectType has the proper value
            If Not thisEffectType = "Basic" AndAlso Not thisEffectType = "Custom" Then
                Throw New Exception
                Exit Sub
            End If

            'Set the effect type
            EffectType = thisEffectType

            'Check to see if any of the texture names have the .xnb file extension included.
            If Microsoft.VisualBasic.Right(strSkyUp, 4) = ".xnb" Then
                strSkyUp = Mid(strSkyUp, 1, Len(strSkyUp) - 4)
            End If
            If Microsoft.VisualBasic.Right(strSkyDown, 4) = ".xnb" Then
                strSkyDown = Mid(strSkyDown, 1, Len(strSkyDown) - 4)
            End If
            If Microsoft.VisualBasic.Right(strSkyRight, 4) = ".xnb" Then
                strSkyRight = Mid(strSkyRight, 1, Len(strSkyRight) - 4)
            End If
            If Microsoft.VisualBasic.Right(strSkyLeft, 4) = ".xnb" Then
                strSkyLeft = Mid(strSkyLeft, 1, Len(strSkyLeft) - 4)
            End If
            If Microsoft.VisualBasic.Right(strSkyFront, 4) = ".xnb" Then
                strSkyFront = Mid(strSkyFront, 1, Len(strSkyFront) - 4)
            End If
            If Microsoft.VisualBasic.Right(strSkyBack, 4) = ".xnb" Then
                strSkyBack = Mid(strSkyBack, 1, Len(strSkyBack) - 4)
            End If

            Try
                'assign the textures
                SkyUp.TheTextureFile = XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & "Content\Textures\" & strSkyUp)
                SkyDown.TheTextureFile = XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & "Content\Textures\" & strSkyDown)
                SkyRight.TheTextureFile = XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & "Content\Textures\" & strSkyRight)
                SkyLeft.TheTextureFile = XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & "Content\Textures\" & strSkyLeft)
                SkyBack.TheTextureFile = XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & "Content\Textures\" & strSkyBack)
                SkyFront.TheTextureFile = XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & "Content\Textures\" & strSkyFront)

                'Initialize the array
                TextureClassArray = New TextureClass() {SkyUp, SkyDown, SkyRight, SkyLeft, SkyFront, SkyBack}

                For TempInt = 0 To UBound(TextureClassArray)
                    If TextureClassArray(TempInt).TheTextureFile Is Nothing Then
                        TextureClassArray(TempInt).TheTextureFile = XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & "Content\Textures\texturenotfound")
                    End If
                Next
            Catch ex As Exception
                ' MsgBox(ex.Message)
            End Try

            'Determine and assign the postions of the vertices.
            SkyUpVertexBackLeft = SkyUpBackLeft
            SkyUpVertexFrontLeft = SkyUpFrontLeft
            SkyUpVertexFrontRight = SkyUpFrontRight
            SkyUpVertexBackRight = SkyUpBackRight
            SkyDownVertexBackLeft = SkyDownBackLeft
            SkyDownVertexFrontLeft = SkyDownFrontLeft
            SkyDownVertexFrontRight = SkyDownFrontRight
            SkyDownVertexBackRight = SkyDownBackRight

            'Should the skybox move in synch with the camera position
            LockSkyboxIntoCameraPosition = LockSkyBox
            'Because the skybox origin might not be (0,0,0) when the vertices are initialized manually, we must now calculate it.
            SkyBoxOrigin = New Vector3(SkyUpVertexFrontRight.X - (Vector3.Distance(SkyUpVertexFrontRight, SkyUpVertexFrontLeft) / 2), _
            SkyUpVertexFrontRight.Y - (Vector3.Distance(SkyUpVertexFrontRight, SkyDownVertexFrontRight) / 2), _
            SkyUpVertexFrontRight.Z - (Vector3.Distance(SkyUpVertexFrontRight, SkyUpVertexBackRight) / 2))

            'Set the cameraview to the diameter of the skybox
            Camera.FarClip = Vector3.Distance(SkyUpVertexBackLeft, SkyDownVertexFrontRight)

            Select Case EffectType
                Case Is = "Basic"
                    'Initialize the textures
                    'Setup the Temporary Vertex array 
                    SkyUp.VertexArray = New Vector3() {SkyUpVertexBackLeft, SkyUpVertexFrontLeft, SkyUpVertexFrontRight, SkyUpVertexBackRight}
                    SkyUp.Initialize(SkyUp.TheTextureFile, Nothing, Nothing, "SkyUp", thisEffectType)
                    'Redo the vertex array for the next texture, no need to change the U and V corrs because they are the
                    'same for all textures
                    SkyDown.VertexArray = New Vector3() {SkyDownVertexFrontLeft, SkyDownVertexBackLeft, SkyDownVertexBackRight, SkyDownVertexFrontRight}
                    SkyDown.Initialize(SkyDown.TheTextureFile, Nothing, Nothing, "SkyDown", thisEffectType)
                    SkyRight.VertexArray = New Vector3() {SkyDownVertexBackRight, SkyUpVertexBackRight, SkyUpVertexFrontRight, SkyDownVertexFrontRight}
                    SkyRight.Initialize(SkyRight.TheTextureFile, Nothing, Nothing, "SkyRight", thisEffectType)
                    SkyLeft.VertexArray = New Vector3() {SkyDownVertexFrontLeft, SkyUpVertexFrontLeft, SkyUpVertexBackLeft, SkyDownVertexBackLeft}
                    SkyLeft.Initialize(SkyLeft.TheTextureFile, Nothing, Nothing, "SkyLeft", thisEffectType)
                    SkyBack.VertexArray = New Vector3() {SkyDownVertexBackLeft, SkyUpVertexBackLeft, SkyUpVertexBackRight, SkyDownVertexBackRight}
                    SkyBack.Initialize(SkyBack.TheTextureFile, Nothing, Nothing, "SkyBack", thisEffectType)
                    SkyFront.VertexArray = New Vector3() {SkyDownVertexFrontRight, SkyUpVertexFrontRight, SkyUpVertexFrontLeft, SkyDownVertexFrontLeft}
                    SkyFront.Initialize(SkyFront.TheTextureFile, Nothing, Nothing, "SkyFront", thisEffectType)

                    'Setup the basic effect for each skybox texture
                    For Each TempTextureClass As TextureClass In TextureClassArray
                        TempTextureClass.TheBasicEffect.GraphicsDevice.SamplerStates(0).AddressU = TextureAddressMode.Clamp
                        TempTextureClass.TheBasicEffect.GraphicsDevice.SamplerStates(0).AddressV = TextureAddressMode.Clamp
                        TempTextureClass.TheBasicEffect.GraphicsDevice.RenderState.DepthBufferEnable = False
                        TempTextureClass.TheBasicEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace
                    Next

                Case Is = "Custom"
                    'Initialize the textures
                    'Setup the Temporary Vertex array 
                    SkyUp.VertexArray = New Vector3() {SkyUpVertexBackLeft, SkyUpVertexFrontLeft, SkyUpVertexFrontRight, SkyUpVertexBackRight}
                    SkyUp.Initialize(SkyUp.TheTextureFile, "skyboxeffect", "SkyBox", "SkyUp", thisEffectType)
                    'Redo the vertex array for the next texture, no need to change the U and V corrs because they are the
                    'same for all textures
                    SkyDown.VertexArray = New Vector3() {SkyDownVertexFrontLeft, SkyDownVertexBackLeft, SkyDownVertexBackRight, SkyDownVertexFrontRight}
                    SkyDown.Initialize(SkyDown.TheTextureFile, "skyboxeffect", "SkyBox", "SkyDown", thisEffectType)
                    SkyRight.VertexArray = New Vector3() {SkyDownVertexBackRight, SkyUpVertexBackRight, SkyUpVertexFrontRight, SkyDownVertexFrontRight}
                    SkyRight.Initialize(SkyRight.TheTextureFile, "skyboxeffect", "SkyBox", "SkyRight", thisEffectType)
                    SkyLeft.VertexArray = New Vector3() {SkyDownVertexFrontLeft, SkyUpVertexFrontLeft, SkyUpVertexBackLeft, SkyDownVertexBackLeft}
                    SkyLeft.Initialize(SkyLeft.TheTextureFile, "skyboxeffect", "SkyBox", "SkyLeft", thisEffectType)
                    SkyBack.VertexArray = New Vector3() {SkyDownVertexBackLeft, SkyUpVertexBackLeft, SkyUpVertexBackRight, SkyDownVertexBackRight}
                    SkyBack.Initialize(SkyBack.TheTextureFile, "skyboxeffect", "SkyBox", "SkyBack", thisEffectType)
                    SkyFront.VertexArray = New Vector3() {SkyDownVertexFrontRight, SkyUpVertexFrontRight, SkyUpVertexFrontLeft, SkyDownVertexFrontLeft}
                    SkyFront.Initialize(SkyFront.TheTextureFile, "skyboxeffect", "SkyBox", "SkyFront", thisEffectType)
            End Select

        End Sub

        ''' <summary>
        '''When the graphics device is lost, usually when another process takes control of it, the skybox must be re-initialized.
        '''</summary>
        Public Sub OnDeviceReset()
            'When the XNAGraphics.GarphicsDevice is lost, the skybox must be reinitialized
            Select Case EffectType
                Case Is = "Basic"
                    'Initialize the textures
                    'Setup the Temporary Vertex array 
                    SkyUp.VertexArray = New Vector3() {SkyUpVertexBackLeft, SkyUpVertexFrontLeft, SkyUpVertexFrontRight, SkyUpVertexBackRight}
                    SkyUp.Initialize(SkyUp.TheTextureFile, Nothing, Nothing, "SkyUp", EffectType)
                    'Redo the vertex array for the next texture, no need to change the U and V corrs because they are the
                    'same for all textures
                    SkyDown.VertexArray = New Vector3() {SkyDownVertexFrontLeft, SkyDownVertexBackLeft, SkyDownVertexBackRight, SkyDownVertexFrontRight}
                    SkyDown.Initialize(SkyDown.TheTextureFile, Nothing, Nothing, "SkyDown", EffectType)
                    SkyRight.VertexArray = New Vector3() {SkyDownVertexBackRight, SkyUpVertexBackRight, SkyUpVertexFrontRight, SkyDownVertexFrontRight}
                    SkyRight.Initialize(SkyRight.TheTextureFile, Nothing, Nothing, "SkyRight", EffectType)
                    SkyLeft.VertexArray = New Vector3() {SkyDownVertexFrontLeft, SkyUpVertexFrontLeft, SkyUpVertexBackLeft, SkyDownVertexBackLeft}
                    SkyLeft.Initialize(SkyLeft.TheTextureFile, Nothing, Nothing, "SkyLeft", EffectType)
                    SkyBack.VertexArray = New Vector3() {SkyDownVertexBackLeft, SkyUpVertexBackLeft, SkyUpVertexBackRight, SkyDownVertexBackRight}
                    SkyBack.Initialize(SkyBack.TheTextureFile, Nothing, Nothing, "SkyBack", EffectType)
                    SkyFront.VertexArray = New Vector3() {SkyDownVertexFrontRight, SkyUpVertexFrontRight, SkyUpVertexFrontLeft, SkyDownVertexFrontLeft}
                    SkyFront.Initialize(SkyFront.TheTextureFile, Nothing, Nothing, "SkyFront", EffectType)

                    'Setup the basic effect for each skybox texture
                    For Each TempTextureClass In TextureClassArray
                        TempTextureClass.TheBasicEffect.GraphicsDevice.SamplerStates(0).AddressU = TextureAddressMode.Clamp
                        TempTextureClass.TheBasicEffect.GraphicsDevice.SamplerStates(0).AddressV = TextureAddressMode.Clamp
                        TempTextureClass.TheBasicEffect.GraphicsDevice.RenderState.DepthBufferEnable = False
                        TempTextureClass.TheBasicEffect.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace
                    Next

                Case Is = "Custom"
                    'Initialize the textures
                    'Setup the Temporary Vertex array 
                    SkyUp.VertexArray = New Vector3() {SkyUpVertexBackLeft, SkyUpVertexFrontLeft, SkyUpVertexFrontRight, SkyUpVertexBackRight}
                    SkyUp.Initialize(SkyUp.TheTextureFile, "skyboxeffect", "SkyBox", "SkyUp", EffectType)
                    'Redo the vertex array for the next texture, no need to change the U and V corrs because they are the
                    'same for all textures
                    SkyDown.VertexArray = New Vector3() {SkyDownVertexFrontLeft, SkyDownVertexBackLeft, SkyDownVertexBackRight, SkyDownVertexFrontRight}
                    SkyDown.Initialize(SkyDown.TheTextureFile, "skyboxeffect", "SkyBox", "SkyDown", EffectType)
                    SkyRight.VertexArray = New Vector3() {SkyDownVertexBackRight, SkyUpVertexBackRight, SkyUpVertexFrontRight, SkyDownVertexFrontRight}
                    SkyRight.Initialize(SkyRight.TheTextureFile, "skyboxeffect", "SkyBox", "SkyRight", EffectType)
                    SkyLeft.VertexArray = New Vector3() {SkyDownVertexFrontLeft, SkyUpVertexFrontLeft, SkyUpVertexBackLeft, SkyDownVertexBackLeft}
                    SkyLeft.Initialize(SkyLeft.TheTextureFile, "skyboxeffect", "SkyBox", "SkyLeft", EffectType)
                    SkyBack.VertexArray = New Vector3() {SkyDownVertexBackLeft, SkyUpVertexBackLeft, SkyUpVertexBackRight, SkyDownVertexBackRight}
                    SkyBack.Initialize(SkyBack.TheTextureFile, "skyboxeffect", "SkyBox", "SkyBack", EffectType)
                    SkyFront.VertexArray = New Vector3() {SkyDownVertexFrontRight, SkyUpVertexFrontRight, SkyUpVertexFrontLeft, SkyDownVertexFrontLeft}
                    SkyFront.Initialize(SkyFront.TheTextureFile, "skyboxeffect", "SkyBox", "SkyFront", EffectType)

            End Select
        End Sub

        ''' <summary>
        ''' If LockSkyBox is True then this sub will update the position of the skybox, such that it is always the 
        ''' same distance from the camera, giving the impression that the sky is always on the horizon
        ''' </summary>
        ''' <param name="LockSkybox">The Boolean that decides if this sub should run - Use SkyBoxInstance.LockSkyBox</param>
        ''' <remarks></remarks>
        Public Sub Update(ByVal LockSkybox As Boolean)

            'If LockSkybox = False then the skybox position does not need to updated.
            If LockSkybox = False Then Exit Sub

            'If the camera position has moved.
            If Camera.LastCameraPosition <> Camera.CameraPosition Then
                ' calculate the new Skybox vertex positions
                SkyUpVertexBackLeft = Vector3.Add(SkyUpVertexBackLeft, _
                    Vector3.Subtract(Camera.CameraPosition, Camera.LastCameraPosition))
                SkyUpVertexFrontLeft = Vector3.Add(SkyUpVertexFrontLeft, _
                     Vector3.Subtract(Camera.CameraPosition, Camera.LastCameraPosition))
                SkyUpVertexFrontRight = Vector3.Add(SkyUpVertexFrontRight, _
                     Vector3.Subtract(Camera.CameraPosition, Camera.LastCameraPosition))
                SkyUpVertexBackRight = Vector3.Add(SkyUpVertexBackRight, _
                     Vector3.Subtract(Camera.CameraPosition, Camera.LastCameraPosition))
                SkyDownVertexBackLeft = Vector3.Add(SkyDownVertexBackLeft, _
                     Vector3.Subtract(Camera.CameraPosition, Camera.LastCameraPosition))
                SkyDownVertexFrontLeft = Vector3.Add(SkyDownVertexFrontLeft, _
                     Vector3.Subtract(Camera.CameraPosition, Camera.LastCameraPosition))
                SkyDownVertexFrontRight = Vector3.Add(SkyDownVertexFrontRight, _
                     Vector3.Subtract(Camera.CameraPosition, Camera.LastCameraPosition))
                SkyDownVertexBackRight = Vector3.Add(SkyDownVertexBackRight, _
                      Vector3.Subtract(Camera.CameraPosition, Camera.LastCameraPosition))

                'Update the SkyBoxOrigin too
                SkyBoxOrigin = Vector3.Add(SkyBoxOrigin, _
                    Vector3.Subtract(Camera.CameraPosition, Camera.LastCameraPosition))

                'Update the Vertices array of each texture with the new skybox vertices
                SkyUp.VertexArray = New Vector3() {SkyUpVertexBackLeft, SkyUpVertexFrontLeft, SkyUpVertexFrontRight, SkyUpVertexBackRight}
                For TempInt = 0 To 3
                    SkyUp.TheVertices(TempInt).Position = SkyUp.VertexArray(TempInt)
                Next
                SkyDown.VertexArray = New Vector3() {SkyDownVertexFrontLeft, SkyDownVertexBackLeft, SkyDownVertexBackRight, SkyDownVertexFrontRight}
                For TempInt = 0 To 3
                    SkyDown.TheVertices(TempInt).Position = SkyDown.VertexArray(TempInt)
                Next
                SkyRight.VertexArray = New Vector3() {SkyDownVertexBackRight, SkyUpVertexBackRight, SkyUpVertexFrontRight, SkyDownVertexFrontRight}
                For TempInt = 0 To 3
                    SkyRight.TheVertices(TempInt).Position = SkyRight.VertexArray(TempInt)
                Next
                SkyLeft.VertexArray = New Vector3() {SkyDownVertexFrontLeft, SkyUpVertexFrontLeft, SkyUpVertexBackLeft, SkyDownVertexBackLeft}
                For TempInt = 0 To 3
                    SkyLeft.TheVertices(TempInt).Position = SkyLeft.VertexArray(TempInt)
                Next
                SkyBack.VertexArray = New Vector3() {SkyDownVertexBackLeft, SkyUpVertexBackLeft, SkyUpVertexBackRight, SkyDownVertexBackRight}
                For TempInt = 0 To 3
                    SkyBack.TheVertices(TempInt).Position = SkyBack.VertexArray(TempInt)
                Next
                SkyFront.VertexArray = New Vector3() {SkyDownVertexFrontRight, SkyUpVertexFrontRight, SkyUpVertexFrontLeft, SkyDownVertexFrontLeft}
                For TempInt = 0 To 3
                    SkyFront.TheVertices(TempInt).Position = SkyFront.VertexArray(TempInt)
                Next

                'assign the last camera position
                Camera.LastCameraPosition = Camera.CameraPosition
            End If

        End Sub

        ''' <summary>
        ''' Updates the views of the skybox
        ''' </summary>
         Public Sub Draw()

            For Each TempTextureClass In TextureClassArray
                Select Case TempTextureClass.TheEffectType
                    Case Is = "Custom"
                        TempTextureClass.TheCustomEffect.Parameters("xView").SetValue(Camera.ViewMatrix)
                        TempTextureClass.TheCustomEffect.Parameters("xProjection").SetValue(Camera.ProjectionMatrix)
                        TempTextureClass.Draw()
                    Case Is = "Basic"
                        TempTextureClass.TheBasicEffect.View = Camera.ViewMatrix
                        TempTextureClass.TheBasicEffect.Projection = Camera.ProjectionMatrix
                        TempTextureClass.Draw()
                End Select
            Next
        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' The Boolean that determines whether teh skybox will move in sycnh with the camera position.
        ''' </summary>
        Public Property LockSkybox() As Boolean
            Get
                Return LockSkyboxIntoCameraPosition
            End Get
            Set(ByVal Value As Boolean)
                LockSkyboxIntoCameraPosition = Value
            End Set
        End Property

        ''' <summary>
        ''' The Vector3 that represents the skybox's central point, the skybox origin.
        ''' </summary>
        Public ReadOnly Property Origin() As Vector3
            Get
                Return SkyBoxOrigin
            End Get
        End Property

#End Region

    End Class
End Namespace
