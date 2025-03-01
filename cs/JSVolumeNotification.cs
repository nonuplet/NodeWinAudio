using Microsoft.JavaScript.NodeApi;
using NAudio.CoreAudioApi;

namespace NodeWinAudio.cs;

[JSExport]
public class JSVolumeNotification
{
  public string Name { get; private set; }
  public bool Muted { get; private set; }
  public float MasterVolume { get; private set; }

  public JSVolumeNotification(MMDevice device, AudioVolumeNotificationData data)
  {
    Name = device.FriendlyName;
    Muted = data.Muted;
    MasterVolume = data.MasterVolume;
  }
}
