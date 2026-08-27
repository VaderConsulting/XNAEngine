
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace XNA
    Public Class TextureClass

#Region "Objects and Variables"

        Private CustomEffect As Effect 'In XNA, all primitives must have an effect applied to them
        Private ABasicEffect As BasicEffect  'If your not using an effects file then you can use a builtin basiceffect
        Private EffectType As String = "Basic" ' The type of effect applied to the texture
        Private Vertices As VertexPositionTexture() ' There are a number of different vertex formats, we will use 
        'VertexPositionTexture array because we want to apply a texture and not a solid colour. 
        Private TextureFile As Texture2D 'Used to hold the texture
        Private TextureName As String = "" ' The name of the texture

        'The following arrays can be used by any texture, they are not specific to any one texture instance
        Public VertexArray() As Vector3 = New Vector3() {} ' The vertex array that can be used as a parameter to the Initialize Sub
        Public VertexUCoordArray() As Integer = New Integer() {0, 0, 1, 1} ' The U Coordinates array that can be used as a parameter to the Initialize Sub
        Public VertexVCoordArray() As Integer = New Integer() {1, 0, 0, 1} ' The V Coordinates  array that can be used as a parameter to the Initialize Sub

        'A Few Temporary Variables that are not disposed, so that the garbage collector does not run
        Private Shared TempInt As Integer = 0
        Private Shared TempPass As EffectPass

#End Region

#Region "Subs and Functions"

        ''' <summary>
        ''' Defines and initializes a texture. Uses a preloaded texture2D object.
        ''' </summary>
        ''' <param name="ThisTexturesName">The name given to the texture. A String Value.</param>
        ''' <param name="CustomEffectTechnique ">The custom effect technique to be used to draw the texture. If you are 
        ''' using a basic effect then set this value to nothing.</param>
        ''' <param name="ThisEffectType">The type of effect used to draw the texture. Either "Basic" or "Custom".</param>
        ''' <param name="ThisTextureFile">The pre-initialized texture2D object.</param>
        ''' <param name="CustomEffectFileName">The name of the effect .fx file. If your are using a basic effect, set this value to Nothing. Not the full path.</param>
        Public Sub Initialize(ByVal ThisTextureFile As Texture2D, ByVal CustomEffectFileName As String, _
        ByVal CustomEffectTechnique As String, ByVal ThisTexturesName As String, _
        ByVal ThisEffectType As String)

            'make sure all names are lowercase
            If CustomEffectFileName IsNot Nothing Then
                CustomEffectFileName = CustomEffectFileName.ToLower
            End If

            'Check to see if the Customfilename has the .xnb file extension included
            If CustomEffectFileName IsNot Nothing Then
                If Microsoft.VisualBasic.Right(CustomEffectFileName, 4) = ".xnb" Then
                    CustomEffectFileName = Mid(CustomEffectFileName, 1, Len(CustomEffectFileName) - 4)
                End If
            End If

            'Uses a custom effect
            EffectType = ThisEffectType

            'define the name of the texture
            TextureName = ThisTexturesName

            'initialize the vertex array
            Vertices = New VertexPositionTexture(UBound(VertexArray)) {}

            'Every Vertex has an x and y coordinate, however, the part of the texture that you want to display 
            'on top of that vertex are sometimes  called the u and v coordinates. In XNA though, they are reffered
            'to as TextureCoordinate.X and TextureCoordinate.Y as shown below.

            For TempInt = 0 To UBound(VertexArray)
                Vertices(TempInt).Position = VertexArray(TempInt)
                Vertices(TempInt).TextureCoordinate.X = VertexUCoordArray(TempInt)
                Vertices(TempInt).TextureCoordinate.Y = VertexVCoordArray(TempInt)
            Next

            Select Case ThisEffectType
                Case Is = "Basic"
                    'Initialize the basiceffect object
                    ABasicEffect = New BasicEffect(XNAEngine.XNAGraphics.GraphicsDevice, Nothing)
                    'configure the effect parameters
                    ABasicEffect.View = Camera.ViewMatrix
                    ABasicEffect.Projection = Camera.ProjectionMatrix
                    ABasicEffect.World = Matrix.Identity
                    ABasicEffect.TextureEnabled = True
                    ABasicEffect.Texture = ThisTextureFile
                Case Is = "Custom"
                    'Load the texture and custom effect from the content pipeline resource
                    CustomEffect = XNAEngine.XNAContentManager.Load(Of Effect)(XNAGameProjectFolder & CustomEffectFileName)

                    'configure the effect parameters
                    CustomEffect.Parameters("xView").SetValue(Camera.ViewMatrix)
                    CustomEffect.Parameters("xProjection").SetValue(Camera.ProjectionMatrix)
                    CustomEffect.Parameters("xWorld").SetValue(Matrix.Identity)
                    CustomEffect.Parameters("xTexture").SetValue(TextureFile)
                    CustomEffect.CurrentTechnique = CustomEffect.Techniques(CustomEffectTechnique)
            End Select

        End Sub

        ''' <summary>
        ''' Defines and initializes a texture. Uses the textuer name to load the texture.
        ''' </summary>
        ''' <param name="ThisTexturesName">The name given to the texture. A String Value.</param>
        ''' <param name="CustomEffectTechnique ">The custom effect technique to be used to draw the texture. If you are 
        ''' using a basic effect then set this value to nothing.</param>
        ''' <param name="ThisEffectType">The type of effect used to draw the texture. Either "Basic" or "Custom".</param>
        ''' <param name="ThisTextureFile">The name of the texture file. Should not iclude the .xnb file extension.
        ''' Is not the full file path. The Texture will be loaded/initialized as part of the Sub.
        ''' Example: "SkyUp" refers to a texture called SkyUp.xnb in the content\textures folder
        ''' Example 2: "Skies\SkyUp" refers to a texture called SkyUp.xnb in the content\textures\skies folder.</param>
        ''' <param name="CustomEffectFileName">The name of the effect .fx file. If your are using a basic effect, set this value to Nothing.</param>
        Public Sub Initialize(ByVal ThisTextureFile As String, ByVal CustomEffectFileName As String, _
        ByVal CustomEffectTechnique As String, ByVal ThisTexturesName As String, ByVal ThisEffectType As String)

            'make sure all names are lowercase
            If CustomEffectFileName IsNot Nothing Then
                CustomEffectFileName = CustomEffectFileName.ToLower
            End If
            ThisTextureFile = ThisTextureFile.ToLower

            'Check to see if any of the texture names have the .xnb file extension included.
            If Microsoft.VisualBasic.Right(ThisTextureFile, 4) = ".xnb" Then
                ThisTextureFile = Mid(ThisTextureFile, 1, Len(ThisTextureFile) - 4)
            End If

            'Check to see if the Customfilename has the .xnb file extension included
            If CustomEffectFileName IsNot Nothing Then
                If Microsoft.VisualBasic.Right(CustomEffectFileName, 4) = ".xnb" Then
                    CustomEffectFileName = Mid(CustomEffectFileName, 1, Len(CustomEffectFileName) - 4)
                End If
            End If

            'Uses a custom effect
            EffectType = ThisEffectType

            'define the name of the texture
            TextureName = ThisTexturesName

            'initialize the vertex array
            Vertices = New VertexPositionTexture(UBound(VertexArray)) {}

            'Every Vertex has an x and y coordinate, however, the part of the texture that you want to display 
            'on top of that vertex are sometimes  called the u and v coordinates. In XNA though, they are reffered
            'to as TextureCoordinate.X and TextureCoordinate.Y as shown below.

            For TempInt = 0 To UBound(VertexArray)
                Vertices(TempInt).Position = VertexArray(TempInt)
                Vertices(TempInt).TextureCoordinate.X = VertexUCoordArray(TempInt)
                Vertices(TempInt).TextureCoordinate.Y = VertexVCoordArray(TempInt)
            Next

            Select Case ThisEffectType
                Case Is = "Basic"
                    'Initialize the basiceffect object
                    ABasicEffect = New BasicEffect(XNAEngine.XNAGraphics.GraphicsDevice, Nothing)

                    'configure the effect parameters
                    ABasicEffect.View = Camera.ViewMatrix
                    ABasicEffect.Projection = Camera.ProjectionMatrix
                    ABasicEffect.World = Matrix.Identity
                    ABasicEffect.TextureEnabled = True
                    ABasicEffect.Texture = XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & ThisTextureFile)
                Case Is = "Custom"
                    'Load the texture and custom effect from the content pipeline resource
                    CustomEffect = XNAEngine.XNAContentManager.Load(Of Effect)(XNAGameProjectFolder & CustomEffectFileName)

                    'configure the effect parameters
                    CustomEffect.Parameters("xView").SetValue(Camera.ViewMatrix)
                    CustomEffect.Parameters("xProjection").SetValue(Camera.ProjectionMatrix)
                    CustomEffect.Parameters("xWorld").SetValue(Matrix.Identity)
                    CustomEffect.Parameters("xTexture").SetValue(XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & ThisTextureFile))
                    CustomEffect.CurrentTechnique = CustomEffect.Techniques(CustomEffectTechnique)
            End Select

        End Sub

        ''' <summary>
        ''' Draws the texture using default values.
        ''' </summary>
        Public Sub Draw()

            Select Case EffectType
                Case Is = "Custom"
                    'Once all parameters have been set, we will begin teh actual drawing process. In XNA all primitives 
                    'must have an effect applied to them.

                    CustomEffect.Parameters("xView").SetValue(Camera.ViewMatrix)
                    CustomEffect.Parameters("xProjection").SetValue(Camera.ProjectionMatrix)

                    CustomEffect.Begin()
                    'For each pass in the total number of passes made in the Textured technique
                    For Each TempPass In CustomEffect.CurrentTechnique.Passes
                        'Begin this pass
                        TempPass.Begin()

                        'associate the vertexdeclaration with our graphics device
                        XNAEngine.XNAGraphics.GraphicsDevice.VertexDeclaration = New VertexDeclaration(XNAEngine.XNAGraphics.GraphicsDevice, _
                        VertexPositionTexture.VertexElements)
                        'and then draw the primitives in the TriangleList style.
                        XNAEngine.XNAGraphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleFan, Vertices, 0, 2)

                        'End the pass
                        TempPass.End()
                    Next
                    'End the effect
                    CustomEffect.End()
                    Exit Select
                Case Is = "Basic"
                    'Once all parameters have been set, we will begin teh actual drawing process. In XNA all primitives 
                    'must have an effect applied to them.

                    ABasicEffect.View = Camera.ViewMatrix
                    ABasicEffect.Projection = Camera.ProjectionMatrix

                    ABasicEffect.Begin()
                    'For each pass in the total number of passes made in the Textured technique
                    For Each TempPass In ABasicEffect.CurrentTechnique.Passes
                        'Begin this pass
                        TempPass.Begin()

                        'associate the vertexdeclaration with our graphics device
                        XNAEngine.XNAGraphics.GraphicsDevice.VertexDeclaration = New VertexDeclaration(XNAEngine.XNAGraphics.GraphicsDevice, _
                        VertexPositionTexture.VertexElements)
                        'and then draw the primitives in the TriangleList style.
                        XNAEngine.XNAGraphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleFan, Vertices, 0, 2)

                        'End the pass
                        TempPass.End()
                    Next
                    'End the effect
                    ABasicEffect.End()
                    Exit Select
            End Select

        End Sub

        ''' <summary>
        ''' Draws the texture using manually configured values.
        ''' </summary>
        ''' <param name="ThePrimitiveType">The way the vertices are drawn. The default is TriangleFan.</param>
        ''' <param name="VertexOffset">The offset used to display the vertices. The default is 0</param>
        ''' <param name="PrimitiveCount">The number of primitives used to diaply the texture. Usually 1 per triangle.</param>
        Public Sub Draw(ByVal ThePrimitiveType As PrimitiveType, ByVal VertexArray As VertexPositionTexture(), ByVal VertexOffset As Integer, _
        ByVal PrimitiveCount As Integer)

            Select Case EffectType
                Case Is = "Custom"
                    'Once all parameters have been set, we will begin teh actual drawing process. In XNA all primitives 
                    'must have an effect applied to them.

                    CustomEffect.Parameters("xView").SetValue(Camera.ViewMatrix)
                    CustomEffect.Parameters("xProjection").SetValue(Camera.ProjectionMatrix)

                    CustomEffect.Begin()
                    'For each pass in the total number of passes made in the Textured technique
                    For Each TempPass In CustomEffect.CurrentTechnique.Passes
                        'Begin this pass
                        TempPass.Begin()

                        'associate the vertexdeclaration with our graphics device
                        XNAEngine.XNAGraphics.GraphicsDevice.VertexDeclaration = New VertexDeclaration(XNAEngine.XNAGraphics.GraphicsDevice, _
                        VertexPositionTexture.VertexElements)
                        'and then draw the primitives in the TriangleList style.
                        XNAEngine.XNAGraphics.GraphicsDevice.DrawUserPrimitives(ThePrimitiveType, VertexArray, VertexOffset, PrimitiveCount)

                        'End the pass
                        TempPass.End()
                    Next
                    'End the effect
                    CustomEffect.End()
                    Exit Select
                Case Is = "Basic"
                    'Once all parameters have been set, we will begin teh actual drawing process. In XNA all primitives 
                    'must have an effect applied to them.

                    ABasicEffect.View = Camera.ViewMatrix
                    ABasicEffect.Projection = Camera.ProjectionMatrix

                    ABasicEffect.Begin()

                    'For each pass in the total number of passes made in the Textured technique
                    For Each TempPass In ABasicEffect.CurrentTechnique.Passes
                        'Begin this pass
                        TempPass.Begin()

                        'associate the vertexdeclaration with our graphics device
                        XNAEngine.XNAGraphics.GraphicsDevice.VertexDeclaration = New VertexDeclaration(XNAEngine.XNAGraphics.GraphicsDevice, _
                        VertexPositionTexture.VertexElements)
                        'and then draw the primitives in the TriangleList style.
                        XNAEngine.XNAGraphics.GraphicsDevice.DrawUserPrimitives(ThePrimitiveType, VertexArray, VertexOffset, PrimitiveCount)

                        'End the pass
                        TempPass.End()
                    Next
                    'End the effect
                    ABasicEffect.End()
                    Exit Select
            End Select
        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' The CustomEffect used by this texture.
        ''' </summary>
        Public Property TheCustomEffect() As Effect
            Get
                Return CustomEffect
            End Get
            Set(ByVal Value As Effect)
                CustomEffect = Value
            End Set
        End Property

        ''' <summary>
        ''' The basicEffect used by this texture.
        ''' </summary>
        Public Property TheBasicEffect() As BasicEffect
            Get
                Return ABasicEffect
            End Get
            Set(ByVal Value As BasicEffect)
                ABasicEffect = Value
            End Set
        End Property

        ''' <summary>
        ''' The type of effect used to draw this texture, either "Basic" or "Custom"
        ''' </summary>
        Public Property TheEffectType() As String
            Get
                Return EffectType
            End Get
            Set(ByVal Value As String)
                EffectType = Value
            End Set
        End Property

        ''' <summary>
        ''' The texture file that is associated with this textureclass instance.
        ''' </summary>
        Public Property TheTextureFile() As Texture2D
            Get
                Return TextureFile
            End Get
            Set(ByVal Value As Texture2D)
                TextureFile = Value
            End Set
        End Property

        ''' <summary>
        ''' The vertex array that holds the vertices used to draw the texture.
        ''' </summary>
        Public Property TheVertices() As VertexPositionTexture()
            Get
                Return Vertices
            End Get
            Set(ByVal Value As VertexPositionTexture())
                Vertices = Value
            End Set
        End Property

#End Region

    End Class
End Namespace
