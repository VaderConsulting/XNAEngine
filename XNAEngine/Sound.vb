Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Audio
Imports System.Threading

Namespace XNA
Public Class Sound

#Region "Objects and Variables"

        Public Shared AudioEngine As AudioEngine
        Public Shared WaveBank As WaveBank
        Public Shared SoundBank As SoundBank
        ' Thread and Sound to be used for playing the background music
        ' Public Shared MusicThread As Thread
        ' Public Shared MusicTrack As New Sound

        Private Cue As Cue
        Public CueName As String
#End Region

#Region "Subs and Functions"

        Public Sub Play(ByVal Cue1 As Cue)

            Try
                Cue1 = SoundBank.GetCue(CueName)
                Cue1.Play()
            Catch ex As Exception
            End Try

        End Sub

        Public Sub StopAudio(ByVal Cue1 As Cue)

            Try
                Cue1 = SoundBank.GetCue(CueName)
                Cue1.Stop(AudioStopOptions.Immediate)
            Catch ex As Exception
            End Try

        End Sub

        Public Shared Sub InitializeEngine()

            'AudioEngine = New AudioEngine("..\..\Audio\gameaudio.xgs")
            'WaveBank = New WaveBank(AudioEngine, "..\..\Audio\Wave Bank.xwb")
            'SoundBank = New SoundBank(AudioEngine, "..\..\Audio\Sound Bank.xsb")

            'Initialize background Music
            '    MusicTrack.CueName = ""
            ' MusicThread = New Thread(New ParameterizedThreadStart(AddressOf MusicTrack.Play))

        End Sub

        Public Shared Sub DisposeEngine()

            SoundBank.Dispose()
            WaveBank.Dispose()
            AudioEngine.Dispose()

        End Sub

        Public Shared Sub UpdateEngine()
            'AudioEngine.Update()
        End Sub

#End Region


End Class

End Namespace
