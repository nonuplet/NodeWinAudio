using Microsoft.JavaScript.NodeApi;
using NAudio.CoreAudioApi;

namespace NodeWinAudio;

[JSExport]
public static class NodeWinAudioModule
{
  public static List<string> GetAllDevices()
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

  public static float GetDefaultDeviceVolume()
  {
    using var enumerator = new MMDeviceEnumerator();
    var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
    using var volume = device.AudioEndpointVolume;
    return volume.MasterVolumeLevelScalar;
  }

  public static void SetDefaultDeviceVolume(float level)
  {
    if (level is < 0.0f or > 1.0f)
    {
      throw new ArgumentOutOfRangeException(nameof(level));
    }
    
    using var enumerator = new MMDeviceEnumerator();
    var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
    using var volume = device.AudioEndpointVolume;
    volume.MasterVolumeLevelScalar = level;
  }

  public static bool IsDefaultDeviceMuted()
  {
    using var enumerator = new MMDeviceEnumerator();
    var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
    using var volume = device.AudioEndpointVolume;
    return volume.Mute;
  }

  public static void SetDefaultDeviceMuted(bool mute)
  {
    using var enumerator = new MMDeviceEnumerator();
    var device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
    using var volume = device.AudioEndpointVolume;
    volume.Mute = mute;
  }
}
