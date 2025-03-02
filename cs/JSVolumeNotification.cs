using Microsoft.JavaScript.NodeApi;
using NAudio.CoreAudioApi;

namespace NodeWinAudio.cs;

[JSExport]
public enum JSNotificationType
{
  VolumeChanged,
  DeviceChanged,
}

[JSExport]
public class JSVolumeNotification
{
  public string Name { get; private set; }
  public bool Muted { get; private set; }
  public float MasterVolume { get; private set; }
  public JSNotificationType Type { get; private set; }

  public JSVolumeNotification(MMDevice device, bool muted, float masterVolume, JSNotificationType notificationType)
  {
    Name = device.FriendlyName;
    Muted = muted;
    MasterVolume = masterVolume;
    Type = notificationType;
  }

  public override string ToString()
  {
    return "hello! tostring";
  }
}
