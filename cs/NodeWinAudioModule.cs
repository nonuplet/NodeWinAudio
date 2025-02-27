using Microsoft.JavaScript.NodeApi;
using NAudio.CoreAudioApi;

namespace NodeWinAudio;

[JSExport]
public static class NodeWinAudioModule
{
  public static List<string> GetDevices()
  {
    var devices = new List<string>();
    using var enumerator = new MMDeviceEnumerator();
    var endpoints = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
    foreach (var device in endpoints)
    {
      devices.Add(device.FriendlyName);
      device.Dispose();
    }

    return devices;
  }

  public static float GetVolume()
  {
    using var enumerator = new MMDeviceEnumerator();
    var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
    using var volume = device.AudioEndpointVolume;
    return volume.MasterVolumeLevelScalar;
  }
}
