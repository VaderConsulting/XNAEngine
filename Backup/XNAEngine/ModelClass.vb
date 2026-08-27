
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input

Namespace XNA
    Public Class ModelClass

#Region "Objects and Variables"
        Private ModelFile As Model ' The model object
        Private Position As Vector3 = Vector3.Zero  ' The world space position of the model
        Private RotationY As Single = 0.0F ' The rotation angle in degrees for the y direction
        Private RotationX As Single = 0.0F ' The rotation angle in degrees for the x direction
        Private MyWorldRotation As Matrix = Matrix.Identity ' The rotation matrix, set as a unit matrix
        Private Transforms As Matrix() ' The matrix used to decide how the model moves
        Private AspectRatio As Single ' The aspectratio of the model

        'A Few Temporary Variables that are not disposed, so that the garbage collector does not run
        Private TempMesh As ModelMesh
        Private TempEffect As BasicEffect
#End Region

#Region "Subs and Functions"

        ''' <summary>
        ''' Initializes the model instance.
        ''' </summary>
        ''' <param name="ModelPath ">The full path to content pipeline model file with the xnb extension.</param>
        Public Sub Initialize(ByVal ModelPath As String, ByVal ModelPosition As Vector3)

            Position = ModelPosition

            'If the file path conatins the xnb extension then remove it
            If Microsoft.VisualBasic.Right(ModelPath, 4) = ".xnb" Then
                ModelPath = Mid(ModelPath, 0, Len(ModelPath) - 4)
            End If
            Try
                'load the model at ModelPath for this instance
                ModelFile = XNAEngine.XNAContentManager.Load(Of Model)(ModelPath)
            Catch ex As Exception

            End Try

            'initially configure the various model variables
            Transforms = New Matrix(ModelFile.Bones.Count) {}
            ModelFile.CopyAbsoluteBoneTransformsTo(Transforms)

            'Set the default lighting flag, doe snot have to be set in every draw loop , so it is set here
            For Each TempMesh In ModelFile.Meshes
                For Each TempEffect In TempMesh.Effects
                    TempEffect.EnableDefaultLighting()
                Next
            Next

        End Sub

        ''' <summary>
        ''' Updates the model instance.
        ''' </summary>
        ''' <param name="ModelPosition">The new position of the model.</param>
        Public Sub Update(ByVal ModelPosition As Vector3)
            ' Set the position of the model instance
            Position = ModelPosition
        End Sub

        ''' <summary>
        ''' Draws the model instance.
        ''' </summary>
        ''' <param name="view">How you view the model - Use Camera.ViewMatrix</param>
        ''' <param name="projection">The model projection - Use Camera.ProjectionMatrix</param>
        ''' <param name="EnableZBuffer">If True, then DepthBufferEnable=True and DepthBufferWriteEnable=False, Otherwise
        ''' both will be set to False.</param>
        Public Sub Draw(ByVal view As Matrix, ByVal projection As Matrix, ByVal EnableZBuffer As Boolean)

            'configure Z Buffer
            Select Case EnableZBuffer
                Case Is = False
                    XNAEngine.XNAGraphics.GraphicsDevice.RenderState.DepthBufferEnable = False
                    XNAEngine.XNAGraphics.GraphicsDevice.RenderState.DepthBufferWriteEnable = False
                Case Is = True
                    XNAEngine.XNAGraphics.GraphicsDevice.RenderState.DepthBufferEnable = True
                    XNAEngine.XNAGraphics.GraphicsDevice.RenderState.DepthBufferWriteEnable = True
            End Select

            'apply the effects and draw
            For Each TempMesh In ModelFile.Meshes
                For Each TempEffect In TempMesh.Effects
                    TempEffect.View = view
                    TempEffect.Projection = projection
                    TempEffect.World = MyWorldRotation * Transforms(TempMesh.ParentBone.Index) * Matrix.CreateTranslation(Position)
                Next
                TempMesh.Draw()
            Next

        End Sub

#End Region

    End Class
End Namespace
