
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports System.IO

Namespace XNA
    Public Class TerrainClass

#Region "Objects and Variables"

        Private ABasicEffect As BasicEffect
        Private CustomEffect As Effect
        Private Offset As Integer = 0
        Private XDistance As Integer = 0
        Private ZDistance As Integer = 0 ' Positive Z behind you
        Private VertPosNormColours() As VertexPositionNormalColor = New VertexPositionNormalColor() {}
        Private Indices() As Integer = New Integer() {}
        Private HeightData() As Integer = New Integer() {}
        Private ThisEffectType As Effecttype
        Private WhichVertexType As VertexType
        Private TextureClassArray() As TextureClass

        Private ib As IndexBuffer
        Private vb As VertexBuffer

        'A Few Temporary Variables that are not disposed, so that the garbage collector does not run
        Private TempInt As Integer = 0
        Private TempIntX As Integer = 0
        Private TempIntY As Integer = 0
        Private TempIntZ As Integer = 0
        Private TempPass As EffectPass

        'Type of effect to be used
        Public Enum Effecttype As Integer
            Basic = 0
            Custom = 1
        End Enum

        'Type of effect to be used
        Public Enum VertexType As Integer
            Colored = 0
            Textured = 1
        End Enum

        Private Structure VertexPositionNormalColor

            Public Position As Vector3
            Public Colour As Color
            Public Normal As Vector3

            Public Shared SizeInBytes As Integer = 7 * 4
            Public Shared VertexElements As VertexElement() = New VertexElement() _
            {New VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0), _
             New VertexElement(0, 4 * 3, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 0), _
             New VertexElement(0, 4 * 4, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0)}

        End Structure

#End Region

#Region "Subs and Functions"

        ''' <summary>
        ''' Initializes the terrain object. Taking the heightmap file name, with the file extension as a parameter.
        ''' </summary>
        ''' <param name="HeightMapFileName">The heightmap file name, with the file extension. Can be bmp, jpg, png.
        '''  Example: HeightMap.jpg, although not necessarry, should be square.</param>
        ''' <param name="DimensionMultiplier">Multplies the width an length vslues
        ''' of the heightmap to make it bigger</param>
        ''' <param name="WhichEffectType"> The Type of effect that will be used to draw the terrain.</param>
        ''' <param name="CustomEffectFileName">The optional name of the custom effect file. Required if using a custom effect. 
        ''' Try "Content\Effects\standardeffects" for a custom effect and "Nothing" for a basc effect.</param>
        ''' <param name="CustomEffectTechnique">The Technique with which to draw the terrain. Colored, Textured, etc. Required if using a custom effect. Type 
        ''' Nothing if you are using a basic effect</param>
        ''' <param name="WhichVertexType">The type of vertex used.</param>
        ''' <param name="TextureArray ">The array of textures to use. If using a colored effect
        ''' set this to Nothing</param>
        Public Sub Initialize(ByVal HeightMapFileName As String, ByVal WhichEffectType As Effecttype, _
        ByVal DimensionMultiplier As Single, ByVal WhichVertexType As VertexType, _
        ByVal CustomEffectFileName As String, ByVal CustomEffectTechnique As String, _
        ByVal TextureArray() As String)

            If DimensionMultiplier < 1.0 Then DimensionMultiplier = 1.0

            Try
                Dim Image1 As System.Drawing.Image ' An image
                Dim MemStream As System.IO.MemoryStream = New System.IO.MemoryStream ' A section of memory
                Dim Bitmap As Bitmap ' A bitmap object to hold the converted image

                Image1 = Image.FromFile(XNAGameProjectFolder & HeightMapFileName) ' Load the heightmap file

                'Convert the image to a bitmap
                Bitmap = New Bitmap(Image1)

                'Save the bitmap to the memory stream as a bitmap
                Bitmap.Save(MemStream, System.Drawing.Imaging.ImageFormat.Bmp)

                MemStream.Position = 0 'Reset the memstraem position to the start
                Dim BR As New BinaryReader(MemStream)

                'seek to the byte that indicates the offset to the actual pixeldata
                'The offset is  4 bytes.
                BR.BaseStream.Seek(10, SeekOrigin.Current)
                Offset = CInt(BR.ReadUInt32)

                'Next we seek another 4 bytes to byte 19, where we find the WIDTH and the Length of the image
                BR.BaseStream.Seek(4, SeekOrigin.Current)
                XDistance = CInt(BR.ReadUInt32)
                ZDistance = CInt(BR.ReadUInt32)

                'Now we can initialise our heightData array and seek further to the pixeldata
                BR.BaseStream.Seek(Offset - 26, SeekOrigin.Current)
                ReDim HeightData(XDistance * ZDistance)

                'Now we know the exact width and height, we are going to store the sum of the 3 colors as 
                'the height for a pixel. 
                Dim YDistance As Integer = 0
                For TempIntX = 0 To UBound(HeightData) - 1
                    YDistance = CInt(BR.ReadByte)
                    YDistance += CInt(BR.ReadByte)
                    YDistance += CInt(BR.ReadByte)
                    BR.ReadByte() ' alpha channel data
                    YDistance /= 8
                    HeightData(TempIntX) = CInt(YDistance)
                Next

                BR.Close()
                MemStream.Close()
                BR = Nothing
                Image1.Dispose()
                YDistance = Nothing
                MemStream.Dispose()
                Bitmap.Dispose()
            Catch ex As Exception
                ' MsgBox(ex.Message)
                'Application.Exit()
                Exit Sub
            End Try

            Select Case WhichVertexType
                Case VertexType.Colored   'Using Colours

                    'Configure the VertexPositionColours array, setting the position and colour
                    'of each vertex
                    ReDim VertPosNormColours(XDistance * ZDistance)
                    TempIntZ = -(ZDistance / 2)
                    TempIntX = -(XDistance / 2)
                    For TempInt = 0 To UBound(VertPosNormColours) - 1
                        VertPosNormColours(TempInt).Position = New Vector3(TempIntX, HeightData(TempInt), TempIntZ)
                        'Mutiply each vertex position by the dimension multiplier
                        VertPosNormColours(TempInt).Position *= DimensionMultiplier
                        'and determine Colours
                        Select Case VertPosNormColours(TempInt).Position.Y
                            Case 0 To 5
                                VertPosNormColours(TempInt).Colour = Color.Blue
                            Case 6 To 15
                                VertPosNormColours(TempInt).Colour = Color.Green
                            Case 16 To 128
                                VertPosNormColours(TempInt).Colour = Color.Gray
                            Case Is > 128
                                VertPosNormColours(TempInt).Colour = Color.WhiteSmoke
                        End Select
                        TempIntX += 1
                        If TempIntX >= (XDistance / 2) Then
                            TempIntX = -(XDistance / 2)
                            TempIntZ += 1
                        End If
                    Next

                    'Define Normal data. 
                    TempIntX = 0
                    TempIntY = 0
                    TempIntZ = 0
                    For TempIntY = 0 To ZDistance - 2
                        For TempIntZ = 0 To XDistance - 2
                            'Vector1 = first corner of the triangle minus the second corner of the triangle
                            'Vector2 = second corner of the triangle minus the third corner of the triangle
                            Dim Vector2 As Vector3 = Vector3.Subtract(VertPosNormColours((TempIntY _
                            * XDistance) + TempIntZ).Position, VertPosNormColours((TempIntY * _
                            XDistance) + TempIntZ + 1).Position)
                            Dim Vector1 As Vector3 = Vector3.Subtract(VertPosNormColours(((TempIntY _
                            + 1) * XDistance) + TempIntZ).Position, VertPosNormColours((TempIntY * _
                            XDistance) + TempIntZ).Position)
                            'The normal for that triangle is the cross product of the 2 vectirs
                            Dim Normal As Vector3 = Vector3.Cross(Vector1, Vector2)
                            'Normalizing the vector keeps it small.
                            Normal.Normalize()
                            'assign the Normals to the vertices
                            VertPosNormColours((TempIntY * XDistance) + TempIntZ).Normal += Normal
                            VertPosNormColours((TempIntY * XDistance) + TempIntZ + 1).Normal += Normal
                            VertPosNormColours(((TempIntY + 1) * XDistance) + TempIntZ).Normal += Normal
                        Next
                    Next

                    'Load the vertex buffer
                    vb = New VertexBuffer(XNAEngine.XNAGraphics.GraphicsDevice, _
                    UBound(VertPosNormColours) * 29, BufferUsage.WriteOnly)
                    vb.SetData(VertPosNormColours)

                    'work out indices, 3 indices for each triangle
                    TempInt = 0
                    For TempIntX = 0 To ZDistance - 2
                        For TempIntY = 0 To XDistance - 2
                            ReDim Preserve Indices(UBound(Indices) + 6)
                            Indices(TempInt) = (TempIntX * XDistance) + TempIntY
                            Indices(TempInt + 1) = (TempIntX * XDistance) + TempIntY + 1
                            Indices(TempInt + 2) = ((TempIntX + 1) * XDistance) + TempIntY
                            Indices(TempInt + 3) = (TempIntX * XDistance) + TempIntY + 1
                            Indices(TempInt + 4) = ((TempIntX + 1) * XDistance) + TempIntY + 1
                            Indices(TempInt + 5) = ((TempIntX + 1) * XDistance) + TempIntY
                            TempInt += 6
                        Next
                    Next

                    ib = New IndexBuffer(XNAEngine.XNAGraphics.GraphicsDevice, GetType(Integer), _
                    UBound(Indices) + 1, BufferUsage.WriteOnly)
                    ib.SetData(Indices)

                    Select Case WhichEffectType
                        Case Effecttype.Basic
                            'Configure basic effect
                            ABasicEffect = New BasicEffect(XNAEngine.XNAGraphics.GraphicsDevice, Nothing)
                            ABasicEffect.VertexColorEnabled = True  'The vertices can be colored
                            ABasicEffect.EnableDefaultLighting() 'initiallly set the defualt lighting
                            ABasicEffect.DirectionalLight0.Direction = Vector3.Up  'Set the lights position to (0,1,0)
                            ThisEffectType = Effecttype.Basic

                        Case Effecttype.Custom
                            'Load the texture and custom effect from the content pipeline resource
                            CustomEffect = XNAEngine.XNAContentManager.Load(Of Effect)(XNAGameProjectFolder & CustomEffectFileName)
                            ThisEffectType = Effecttype.Custom
                            'configure the effect parameters
                            CustomEffect.Parameters("xView").SetValue(Camera.ViewMatrix)
                            CustomEffect.Parameters("xProjection").SetValue(Camera.ProjectionMatrix)
                            CustomEffect.Parameters("xWorld").SetValue(Matrix.Identity)
                            CustomEffect.Parameters("xEnableLighting").SetValue(True)
                            CustomEffect.Parameters("xLightDirection").SetValue(Vector3.Up)
                            CustomEffect.CurrentTechnique = CustomEffect.Techniques(CustomEffectTechnique)

                    End Select

                Case VertexType.Textured 'Using Textures

                    '''''''UnFinished

                    'Load Textures
                    Try
                        For Each str As String In TextureArray
                            XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & str)
                        Next
                    Catch ex As Exception
                    End Try

                    'Configure Textures
                    ReDim TextureClassArray(XDistance * ZDistance)
                    For TempInt = 0 To UBound(TextureClassArray) - 1
                        TextureClassArray(TempInt) = New TextureClass
                    Next

                    For TempIntZ = 0 To ZDistance - 1
                        For TempIntX = 0 To XDistance - 1

                        Next
                    Next

            End Select

        End Sub

        ' Get the highest value in the heightmap
        Public Function GetHighestValueInHeightMap()
            TempInt = 0
            For TempIntX = 0 To UBound(HeightData) - 1
                If HeightData(TempIntX) > TempInt Then
                    TempInt = HeightData(TempIntX)
                End If
            Next
            Return TempInt
        End Function

        Public Sub Update()

        End Sub

        Public Sub Draw()

            Select Case WhichVertexType
                Case VertexType.Colored   'Using Colours
                    Select Case ThisEffectType
                        Case Effecttype.Basic
                            ABasicEffect.View = Camera.ViewMatrix
                            ABasicEffect.Projection = Camera.ProjectionMatrix

                            ABasicEffect.Begin()
                            'For each pass in the total number of passes made in the Textured technique
                            For Each TempPass In ABasicEffect.CurrentTechnique.Passes
                                'Begin this pass
                                TempPass.Begin()

                                'associate the vertexdeclaration with our graphics device
                                XNAEngine.XNAGraphics.GraphicsDevice.Vertices(0).SetSource(vb, 0, _
                                VertexPositionNormalColor.SizeInBytes)
                                XNAEngine.XNAGraphics.GraphicsDevice.Indices = ib
                                XNAEngine.XNAGraphics.GraphicsDevice.VertexDeclaration = New VertexDeclaration( _
                                XNAEngine.XNAGraphics.GraphicsDevice, _
                                VertexPositionNormalColor.VertexElements)
                                'and then draw the primitives in the TriangleList style.
                                XNAEngine.XNAGraphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, _
                                0, 0, UBound(VertPosNormColours), 0, (XDistance - 1) * (ZDistance - 1) * 2)

                                'End the pass
                                TempPass.End()
                            Next
                            'End the effect
                            ABasicEffect.End()
                        Case Effecttype.Custom

                            CustomEffect.Parameters("xView").SetValue(Camera.ViewMatrix)
                            CustomEffect.Parameters("xProjection").SetValue(Camera.ProjectionMatrix)

                            CustomEffect.Begin()
                            'For each pass in the total number of passes made in the Textured technique
                            For Each TempPass In CustomEffect.CurrentTechnique.Passes
                                'Begin this pass
                                TempPass.Begin()

                                'associate the vertexdeclaration with our graphics device
                                XNAEngine.XNAGraphics.GraphicsDevice.Vertices(0).SetSource(vb, 0, _
                                VertexPositionNormalColor.SizeInBytes)
                                XNAEngine.XNAGraphics.GraphicsDevice.Indices = ib
                                XNAEngine.XNAGraphics.GraphicsDevice.VertexDeclaration = New VertexDeclaration( _
                                XNAEngine.XNAGraphics.GraphicsDevice, _
                                VertexPositionNormalColor.VertexElements)
                                'and then draw the primitives in the TriangleList style.
                                XNAEngine.XNAGraphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, _
                        0, 0, UBound(VertPosNormColours), 0, (XDistance - 1) * (ZDistance - 1) * 2)

                                'End the pass
                                TempPass.End()
                            Next
                            'End the effect
                            CustomEffect.End()
                    End Select

                Case VertexType.Textured 'Using Textures

            End Select

        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' The Width of the Heightmap
        ''' </summary>
        Public ReadOnly Property Width()
            Get
                Return XDistance
            End Get
        End Property

        ''' <summary>
        ''' The Length of the Heightmap
        ''' </summary>
        Public ReadOnly Property Length()
            Get
                Return ZDistance
            End Get
        End Property

#End Region

    End Class
End Namespace
