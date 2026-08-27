

Namespace XNA
    Public Class Framerate

        Private Shared lastTick As Integer
        Private Shared lastFrameRate As Integer
        Private Shared frameRate As Integer

        Public Shared Function CalculateFrameRate() As Integer

            If System.Environment.TickCount - lastTick >= 1000 Then
                lastFrameRate = frameRate
                frameRate = 0
                lastTick = System.Environment.TickCount
            End If

            frameRate += 1

            Return lastFrameRate

        End Function 'CalculateFrameRate

    End Class
End Namespace
