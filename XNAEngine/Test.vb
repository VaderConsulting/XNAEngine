
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace XNA
    Public Class Test

#Region "Objects and Variables"

        Private ABasicEffect As BasicEffect
        Public CustomEffect As Effect 'In XNA, all primitives must have an effect applied to them
        Public VertPosTextures() As VertexPositionTexture ' There are a number of different vertex formats, we will use 
        'VertexPositionTexture array because we want to apply a texture and not a solid colour. 
        Public viewMatrix As Matrix ' Used to position the player's view
        Public projectionMatrix As Matrix ' 'Used to calulate the player's view
        Public vertexBuffer1 As VertexBuffer ' The vertices will be stored in the vertex buffer, before being 
        'streamed to the graphics device
        Public TextureFile As Texture2D 'Used to hold the texture

        Private ib As IndexBuffer
        Private vb As VertexBuffer
        Private Indices() As Integer = New Integer() {}
        Private TempPass As EffectPass

#End Region

#Region "Subs and Functions"

        'Configure the vertices for the texture
        Public Sub Initialize()

            VertPosTextures = New VertexPositionTexture(4) {}

            'Every Vertex has an x and y coordinate, however, the part of the texture that you want to display 
            'on top of that vertex are sometimes  called the u and v coordinates. In XNA though, they are reffered
            'to as TextureCoordinate.X and TextureCoordinate.Y as shown below.

            VertPosTextures(0).Position = New Vector3(-10.0F, 10.0F, 1.0F)
            VertPosTextures(0).TextureCoordinate.X = 0
            VertPosTextures(0).TextureCoordinate.Y = 0

            VertPosTextures(1).Position = New Vector3(10.0F, 10.0F, 1.0F)
            VertPosTextures(1).TextureCoordinate.X = 1
            VertPosTextures(1).TextureCoordinate.Y = 0

            VertPosTextures(2).Position = New Vector3(10.0F, -10.0F, 1.0F)
            VertPosTextures(2).TextureCoordinate.X = 1
            VertPosTextures(2).TextureCoordinate.Y = 1

            VertPosTextures(3).Position = New Vector3(-10.0F, -10.0F, 1.0F)
            VertPosTextures(3).TextureCoordinate.X = 0
            VertPosTextures(3).TextureCoordinate.Y = 1

            ReDim Indices(4)
            Indices(0) = 0
            Indices(1) = 1
            Indices(2) = 2
            Indices(3) = 3

            ib = New IndexBuffer(XNAEngine.XNAGraphics.GraphicsDevice, GetType(Integer), 5, ResourceUsage.WriteOnly, ResourceManagementMode.Automatic)
            ib.SetData(Indices)

            vb = New VertexBuffer(XNAEngine.XNAGraphics.GraphicsDevice, 100, ResourceUsage.WriteOnly, ResourceManagementMode.Automatic)
            vb.SetData(VertPosTextures)

            ABasicEffect = New BasicEffect(XNAEngine.XNAGraphics.GraphicsDevice, Nothing)
            ABasicEffect.TextureEnabled = True
            ABasicEffect.Texture = XNAEngine.XNAContentManager.Load(Of Texture2D)(XNAGameProjectFolder & "Content\Textures\Grass")

        End Sub

        Public Sub Update()

            ABasicEffect.Projection = Camera.ProjectionMatrix
            ABasicEffect.View = Camera.ViewMatrix
            ABasicEffect.World = Matrix.Identity

        End Sub

        Public Sub Draw()


            ABasicEffect.Begin()
            'For each pass in the total number of passes made in the Textured technique
            For Each TempPass In ABasicEffect.CurrentTechnique.Passes
                'Begin this pass
                TempPass.Begin()

                'associate the vertexdeclaration with our graphics device
                XNAEngine.XNAGraphics.GraphicsDevice.Vertices(0).SetSource(vb, 0, _
                VertexPositionTexture.SizeInBytes)
                XNAEngine.XNAGraphics.GraphicsDevice.Indices = ib
                XNAEngine.XNAGraphics.GraphicsDevice.VertexDeclaration = New VertexDeclaration( _
                XNAEngine.XNAGraphics.GraphicsDevice, _
                VertexPositionTexture.VertexElements)
                'and then draw the primitives in the TriangleList style.
                XNAEngine.XNAGraphics.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleFan, _
                VertPosTextures, 0, UBound(VertPosTextures), Indices, 0, 2)
                'XNAEngine.XNAGraphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleFan, _
                'VertPosTextures, 0, 2)

                'End the pass
                TempPass.End()
            Next
            'End the effect
            ABasicEffect.End()

        End Sub

#End Region

    End Class
End Namespace
