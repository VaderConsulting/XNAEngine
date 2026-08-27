
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports System.Drawing

Namespace XNA
    Public Class WriteText

        'The Sub that will create and define the necessary objects and variables
        Public Shared Sub WriteText(ByVal str As String, ByVal FontName As String, ByVal FontSize As Single, _
        ByVal FontStyle As FontStyle, ByVal StringColour As Brush, ByVal BitmapXPos As Single, _
        ByVal BitmapYPos As Single, ByVal Texture1 As Texture2D, ByVal Vector1 As Vector2)

            Dim NewFont As Font = New Font(FontName, FontSize, FontStyle) ' The font that the text will be written in
            Dim Graphics As Graphics = System.Drawing.Graphics.FromHwnd(XNAGame.Window.Handle) ' The Graphics object that will 
            'write the text onto the bitmap
            Dim StringSize As SizeF = Graphics.MeasureString(str, NewFont) 'The length of the text
            Dim Bitmap As Bitmap = New Bitmap(CInt(StringSize.Width), CInt(StringSize.Height)) ' the bitmap that will hold the text
            Graphics = System.Drawing.Graphics.FromImage(Bitmap) 'tell the graphics object that it will be using the bitmap
            Graphics.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAliasGridFit ' and how it will display the text
            Graphics.DrawString(str, NewFont, StringColour, 0, 0) ' Draw the string on the bitmap
            Dim MemStream As System.IO.MemoryStream = New System.IO.MemoryStream 'set aside a portion of memory to hold the bitmap
            Bitmap.Save(MemStream, System.Drawing.Imaging.ImageFormat.Png) ' save the bitmap to the portion of memory
            MemStream.Position = 0 ' dont know what this does, but it is necessary
            Texture1 = Texture2D.FromFile(XNAEngine.XNAGraphics.GraphicsDevice, MemStream) ' create a texture to be used in the spritebatch from the 
            'bitmap that is stored in memory
            Vector1 = New Vector2(BitmapXPos, BitmapYPos) ' set the position of the texture

        End Sub

    End Class

End Namespace









