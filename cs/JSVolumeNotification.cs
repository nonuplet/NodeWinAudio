using Microsoft.JavaScript.NodeApi;
using NAudio.CoreAudioApi;

namespace NodeWinAudio.cs;

[JSExport]
public class JSVolumeNotification
{
  public bool Muted { get; private set; }
  public float MasterVolume { get; private set; }

  public JSVolumeNotification(AudioVolumeNotificationData data)
  {
    Muted = data.Muted;
    MasterVolume = data.MasterVolume;
  }
}
