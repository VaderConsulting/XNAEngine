
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input

Namespace XNA
    Public Class Camera

#Region "Objects and Variables"
        Private Shared MatrixView As Matrix ' Used in the draw sub
        Private Shared MatrixProjection As Matrix ' Used in the draw sub
        Private Shared CameraPos As Vector3 = Vector3.Zero  'The world space coordinates of the camera
        Private Shared LastCameraPos As Vector3 = CameraPos 'The position of the camera in the last update loop.
        Private Shared CameraViewVector As Vector3 = New Vector3(0.0F, 0.0F, -50.0F) ' The world space direction that the camera faces
        Private Shared CameraPitchSng As Single = 0.0F ' Moves the camera direction round the x axis
        Private Shared CameraRollSng As Single = 0.0F ' Moves the camera direction round the z axis
        Private Shared RightVector As Vector3 = Vector3.Right ' The camera space vector that points right, initially set to WorldSpace right
        Private Shared UpVector As Vector3 = Vector3.Up ' The camera space vector that points up, initially set to WorldSpace up
        Private Shared ForwardVector As Vector3 = Vector3.Forward  ' The camera space vector that points forward, initially set to WorldSpace forward
        Private Shared DirectionQuaternion As Quaternion ' The quaternion that represents the Camera Direction
        Private Shared ResultQuaternion As Quaternion ' The resultant Quaternion created by the multiplication of the Direction and Rotation Quaternions
        Private Shared CameraYawSng As Single = 0.0F ' Moves the camera direction round the y axis
        Private Shared ViewAngleSng As Single = MathHelper.ToRadians(45.0F)
        Private Shared AspectRatioSng As Single = XNAEngine.XNAGraphics.GraphicsDevice.Viewport.Width / XNAEngine.XNAGraphics.GraphicsDevice.Viewport.Height
        Private Shared ClipNear As Single = 1.0F ' Objects closer than 1.0F to the camera will not be drawn
        Private Shared ClipFar As Single = 2000.0F ' Objects further away than 2000.0F will not be drawn
        Private Shared RotationQuaternion As Quaternion ' The quaternion used that represents the rotation of the camera direction
        Private Shared MatrixRotation As Matrix ' the rotation matrix created from quatRotation and used to calculate the new camera direction
        Private Shared LastMousePosX As Single = 0.0F ' The X position of the mouse in the last draw loop
        Private Shared LastMousePosY As Single = 0.0F ' The Y position of the mouse in the last draw loop
        Private Shared CurCameraType As New CameraTypeEnum ' The type of camera currently in Use
        Private Shared Frustum As BoundingFrustum ' Used to decide what obejects are drawn

        'Used to determine if skybox should be repositioned
        Private Shared SkyboxUpdate As Boolean = False

        'used in the update sub to determine camera movement
        Public Enum CameraTypeEnum As Integer
            Freeview = 0
            FirstPerson = 1
            ThirdPerson = 2
        End Enum

#End Region

#Region "Subs and Functions"

        ''' <summary>
        ''' Initializes the camera using the Projection and View Matrices
        ''' </summary>
        Public Shared Sub Initialize()

            'Setup the camera initial camera position and direction
            MatrixProjection = Matrix.CreatePerspectiveFieldOfView(ViewAngleSng, AspectRatioSng, ClipNear, ClipFar)
            MatrixView = Matrix.CreateLookAt(CameraPos, CameraViewVector, UpVector)
            Mouse.SetPosition(XNAGame.Window.ClientBounds.Width / 2, XNAGame.Window.ClientBounds.Height / 2)

        End Sub

        ''' <summary>
        ''' Updates the camera's view and position vectors using quaternion geometry.
        ''' </summary>
        ''' <param name="GetKeys  ">The current state of the keyboard, showing which keys are pressed.</param>
        ''' <param name="GetMouse  ">The current state of the mouse, showing if it has moved.</param>
        Public Shared Sub Update(ByVal GetKeys As KeyboardState, ByVal GetMouse As MouseState, ByVal TheCameraType As CameraTypeEnum)

            'Keep Mouse in centre of screen so that it does not stop camera when mouse reaches screen edge.
            If GetMouse.X <= 5 Then Mouse.SetPosition(XNAGame.Window.ClientBounds.Width / 2, GetMouse.Y)
            If GetMouse.X >= XNAGame.Window.ClientBounds.Width - 5 Then Mouse.SetPosition(XNAGame.Window.ClientBounds.Width / 2, GetMouse.Y)
            If GetMouse.Y <= 5 Then Mouse.SetPosition(GetMouse.X, XNAGame.Window.ClientBounds.Height / 2)
            If GetMouse.Y >= XNAGame.Window.ClientBounds.Height - 5 Then Mouse.SetPosition(GetMouse.X, XNAGame.Window.ClientBounds.Height / 2)

            Select Case TheCameraType ' The various types of camera
                Case Is = CameraTypeEnum.Freeview ' The camera is not limited in movement

                    'Set the current Camera type
                    CurCameraType = CameraTypeEnum.Freeview

                    'Normalize the vectors, so they dont end up with enormous values
                    UpVector.Normalize()
                    RightVector.Normalize()
                    ForwardVector.Normalize()

                    'Reset Roll so that it does not effect this loop unless explicitly pressed
                    CameraRollSng = 0.0F

                    'When the Up arrow is pressed
                    If GetKeys.IsKeyDown(Keys.Up) Then
                        ' Move the camera position and direction forward
                        CameraPos = CameraPos + ForwardVector
                        CameraViewVector = CameraViewVector + ForwardVector
                        SkyboxUpdateRequired = True
                        'When the Down arrow is pressed
                    ElseIf GetKeys.IsKeyDown(Keys.Down) Then
                        ' Move the camera position and direction backward
                        CameraPos = CameraPos - ForwardVector
                        CameraViewVector = CameraViewVector + ForwardVector
                        SkyboxUpdateRequired = True
                    End If

                    'When the Left arrow is pressed
                    If GetKeys.IsKeyDown(Keys.Left) Then
                        ' Move the camera position and direction Left
                        CameraPos = CameraPos - RightVector
                        CameraViewVector = CameraViewVector - RightVector
                        SkyboxUpdateRequired = True
                        'When the Right arrow is pressed
                    ElseIf GetKeys.IsKeyDown(Keys.Right) Then
                        ' Move the camera position and direction Right
                        CameraPos = CameraPos + RightVector
                        CameraViewVector = CameraViewVector + RightVector
                        SkyboxUpdateRequired = True
                    End If

                    'When the A key is pressed
                    If GetKeys.IsKeyDown(Keys.A) Then
                        ' Increase the camera roll to the left
                        CameraRollSng -= MathHelper.ToRadians(1.0F)
                        'When the D key is pressed
                    ElseIf GetKeys.IsKeyDown(Keys.D) Then
                        ' Increase the camera roll to the left
                        CameraRollSng += MathHelper.ToRadians(1.0F)
                    End If
                    'Keep the max speed of rotation between 0.05 and -0.05
                    If CameraRollSng > 0.05F Then
                        CameraRollSng = 0.05F
                    ElseIf CameraRollSng < -0.05F Then
                        CameraRollSng = -0.05F
                    End If

                    'If the mouse has moved up
                    If GetMouse.Y < LastMousePosY Then
                        'Rotate Up
                        CameraPitchSng += MathHelper.ToRadians(0.05F)
                    ElseIf GetMouse.Y > LastMousePosY Then
                        'Rotate Down
                        CameraPitchSng -= MathHelper.ToRadians(0.05F)
                    Else
                        'If mouse is not moving, slow the camera speed until it reaches 0
                        Select Case CameraPitchSng
                            Case Is < -0.002F
                                CameraPitchSng += 0.002F
                                Exit Select
                            Case -0.002F To 0.002F
                                CameraPitchSng = 0.0F
                                Exit Select
                            Case Is > 0.002F
                                CameraPitchSng -= 0.002F
                                Exit Select
                        End Select
                    End If
                    'Keep the max speed of rotation between 0.05 and -0.05
                    If CameraPitchSng > 0.05F Then
                        CameraPitchSng = 0.05F
                    ElseIf CameraPitchSng < -0.05F Then
                        CameraPitchSng = -0.05F
                    End If


                    'If the mouse has moved Left
                    If GetMouse.X < LastMousePosX Then
                        'rotate Left
                        CameraYawSng += MathHelper.ToRadians(0.05F)
                    ElseIf GetMouse.X > LastMousePosX Then
                        'rotate Right
                        CameraYawSng -= MathHelper.ToRadians(0.05F)
                    Else
                        'If mouse is not moving, slow the camera speed until it reaches 0
                        Select Case CameraYawSng
                            Case Is < -0.002F
                                CameraYawSng += 0.002F
                                Exit Select
                            Case -0.002F To 0.002F
                                CameraYawSng = 0.0F
                                Exit Select
                            Case Is > 0.002F
                                CameraYawSng -= 0.002F
                                Exit Select
                        End Select
                    End If
                    'Keep the max speed of rotation between 0.05 and -0.05
                    If CameraYawSng > 0.05F Then
                        CameraYawSng = 0.05F
                    ElseIf CameraYawSng < -0.05F Then
                        CameraYawSng = -0.05F
                    End If


                    'Only if a rotation has occured.
                    If CameraYawSng <> 0.0F Or CameraPitchSng <> 0.0F Or CameraRollSng <> 0.0F Then
                        'Work out the direction quaternion
                        DirectionQuaternion = Quaternion.CreateFromAxisAngle(CameraViewVector, 0.0F)
                        'Apply the cameraPitch and CameraYaw
                        RotationQuaternion = Quaternion.Multiply(Quaternion.CreateFromAxisAngle(RightVector, CameraPitchSng), _
                        Quaternion.CreateFromAxisAngle(UpVector, CameraYawSng))
                        'Apply the cameraRoll
                        RotationQuaternion = Quaternion.Multiply(Quaternion.CreateFromAxisAngle(ForwardVector, _
                        CameraRollSng), RotationQuaternion)
                        'Get the resultant quaternion
                        ResultQuaternion = RotationQuaternion * DirectionQuaternion
                        'Create teh roatation matrix
                        MatrixRotation = Matrix.CreateFromQuaternion(ResultQuaternion)

                        'Apply the rotation matrix to the vectors
                        CameraViewVector = Vector3.Transform(CameraViewVector, MatrixRotation)
                        RightVector = Vector3.Transform(RightVector, MatrixRotation)
                        UpVector = Vector3.Transform(UpVector, MatrixRotation)
                        ForwardVector = Vector3.Transform(ForwardVector, MatrixRotation)

                    End If

                    Exit Select

                Case Is = CameraTypeEnum.FirstPerson
                    'Set the current Camera type
                    CurCameraType = CameraTypeEnum.FirstPerson

                    Exit Select

                Case Is = CameraTypeEnum.ThirdPerson
                    'Set the current Camera type
                    CurCameraType = CameraTypeEnum.ThirdPerson

                    Exit Select

            End Select

        End Sub

        ''' <summary>
        ''' Sets the Matrices used by the games draw sub.
        ''' </summary>
        Public Shared Sub Draw()
            MatrixProjection = Matrix.CreatePerspectiveFieldOfView(ViewAngleSng, AspectRatioSng, ClipNear, ClipFar)
            MatrixView = Matrix.CreateLookAt(CameraPos, CameraViewVector, UpVector)

            'Create a Bounding Frustum to be used in culling
            Frustum = New BoundingFrustum(Matrix.Multiply(ViewMatrix, ProjectionMatrix))

        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' The matrix representation of the camera's view.
        ''' </summary>
        Public Shared ReadOnly Property ViewMatrix() As Matrix
            Get
                Return MatrixView
            End Get
        End Property

        ''' <summary>
        ''' The matrix representation of the camera's pprojection.
        ''' </summary>
        Public Shared ReadOnly Property ProjectionMatrix() As Matrix
            Get
                Return MatrixProjection
            End Get
        End Property

        ''' <summary>
        ''' The boudingFrustum that represents the camera's cuurent view
        ''' </summary>
        Public Shared ReadOnly Property CameraFrustum() As BoundingFrustum
            Get
                Return Frustum
            End Get
        End Property

        ''' <summary>
        ''' The camera position as a vector3.
        ''' </summary>
        Public Shared ReadOnly Property CameraPosition() As Vector3
            Get
                Return CameraPos
            End Get
        End Property

        ''' <summary>
        ''' The camera's  position  in the last draw loop as a vector3.
        ''' </summary>
        Public Shared Property LastCameraPosition() As Vector3
            Get
                Return LastCameraPos
            End Get
            Set(ByVal value As Vector3)
                LastCameraPos = value
            End Set
        End Property

        ''' <summary>
        ''' The camera's view as a vector3.
        ''' </summary>
        Public Shared ReadOnly Property CameraView() As Vector3
            Get
                Return CameraViewVector
            End Get
        End Property

        ''' <summary>
        ''' The camera's current pitch angle in Radians as a Single.
        ''' </summary>
        Public Shared ReadOnly Property CameraPitch() As Single
            Get
                Return CameraPitchSng
            End Get
        End Property

        ''' <summary>
        ''' The camera's current roll angle in Radians as a Single.
        ''' </summary>
        Public Shared ReadOnly Property CameraRoll() As Single
            Get
                Return CameraRollSng
            End Get
        End Property

        ''' <summary>
        ''' The camera's vector right.
        ''' </summary>
        Public Shared ReadOnly Property VectorRight() As Vector3
            Get
                Return RightVector
            End Get
        End Property

        ''' <summary>
        ''' The camera's vector up.
        ''' </summary>
        Public Shared ReadOnly Property VectorUp() As Vector3
            Get
                Return UpVector
            End Get
        End Property

        ''' <summary>
        ''' The camera's vector Forward.
        ''' </summary>
        Public Shared ReadOnly Property VectorForward() As Vector3
            Get
                Return ForwardVector
            End Get
        End Property

        ''' <summary>
        ''' The camera's farclip as a Single.
        ''' </summary>
        Public Shared Property FarClip() As Single
            Get
                Return ClipFar
            End Get
            Set(ByVal value As Single)
                ClipFar = value
            End Set
        End Property

        ''' <summary>
        ''' The X component of the camera's Previous mouse position.
        ''' </summary>
        Public Shared Property PreviousMousePosX() As Single
            Get
                Return LastMousePosX
            End Get
            Set(ByVal value As Single)
                LastMousePosX = value
            End Set
        End Property

        ''' <summary>
        ''' The Y component of the camera's Previous mouse position.
        ''' </summary>
        Public Shared Property PreviousMousePosY() As Single
            Get
                Return LastMousePosY
            End Get
            Set(ByVal value As Single)
                LastMousePosY = value
            End Set
        End Property

        ''' <summary>
        ''' The camera type currently in use.
        ''' </summary>
        Public Shared ReadOnly Property CurrentCameraType() As Integer
            Get
                Return CurCameraType
            End Get
        End Property

        ''' <summary>
        ''' The Boolean that determines whether the skybvox needs repositioned.
        ''' </summary>
        Public Shared Property SkyboxUpdateRequired() As Boolean
            Get
                Return SkyboxUpdate
            End Get
            Set(ByVal value As Boolean)
                SkyboxUpdate = value
            End Set
        End Property

#End Region

    End Class
End Namespace
