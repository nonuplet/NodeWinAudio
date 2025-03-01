using Microsoft.JavaScript.NodeApi;
using NAudio.CoreAudioApi;

namespace NodeWinAudio;

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

[JSExport]
public static class NodeWinAudioModule
{
  private static MMDevice? _defaultDevice;
  private static AudioEndpointVolume? _defaultDeviceEndpoint;
  private static bool _isVolumeMonitoring = false;
  private static readonly Dictionary<string, VolumeChangeCallback> VolumeChangeCallbacks = new();

  public delegate void VolumeChangeCallback(JSVolumeNotification data);

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

  private static void CheckDefaultDevice()
  {
    if (_defaultDevice is not null) return;

    using var enumerator = new MMDeviceEnumerator();
    _defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
    _defaultDeviceEndpoint = _defaultDevice.AudioEndpointVolume;
  }

  private static void StartVolumeMonitoring()
  {
    CheckDefaultDevice();
    _defaultDeviceEndpoint!.OnVolumeNotification += OnVolumeChanged;
    _isVolumeMonitoring = true;
  }

  private static void StopVolumeMonitoring()
  {
    CheckDefaultDevice();
    _defaultDeviceEndpoint!.OnVolumeNotification -= OnVolumeChanged;
    _isVolumeMonitoring = false;
  }

  private static void OnVolumeChanged(AudioVolumeNotificationData data)
  {
    try
    {
      foreach (var callback in VolumeChangeCallbacks.Values)
      {
        callback(new JSVolumeNotification(data));
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
    }
  }

  public static void RegisterVolumeChangeCallback(VolumeChangeCallback callback, string uuid)
  {
    if (!_isVolumeMonitoring)
    {
      StartVolumeMonitoring();
    }

    VolumeChangeCallbacks.Add(uuid, callback);
  }

  public static void UnregisterVolumeChangeCallback(string uuid)
  {
    if (!VolumeChangeCallbacks.ContainsKey(uuid))
    {
      throw new KeyNotFoundException();
    }
    
    VolumeChangeCallbacks.Remove(uuid);
    if (VolumeChangeCallbacks.Count == 0)
    {
      StopVolumeMonitoring();
    }
  }

  public static void ClearVolumeChangeCallbacks()
  {
    VolumeChangeCallbacks.Clear();
    StopVolumeMonitoring();
  }
}

