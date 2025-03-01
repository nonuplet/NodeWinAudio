using Microsoft.JavaScript.NodeApi;
using NAudio.CoreAudioApi;

namespace NodeWinAudio.cs;

[JSExport]
public class NodeWinAudioModule
{
  public static NodeWinAudioModule Instance { get; } = new NodeWinAudioModule(); // Singleton

  private MMDevice _defaultDevice;
  private AudioEndpointVolume _defaultDeviceEndpoint;
  private bool _isVolumeMonitoring = false;
  private readonly Dictionary<string, VolumeChangeCallback> VolumeChangeCallbacks = new();

  public delegate void VolumeChangeCallback(JSVolumeNotification data);

  private NodeWinAudioModule()
  {
    using var enumerator = new MMDeviceEnumerator();
    _defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
    _defaultDeviceEndpoint = _defaultDevice.AudioEndpointVolume;
  }

  public List<string> GetAllDevices()
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

  public float GetDefaultDeviceVolume()
  {
    return _defaultDeviceEndpoint.MasterVolumeLevelScalar;
  }

  public void SetDefaultDeviceVolume(float level)
  {
    if (level is < 0.0f or > 1.0f)
    {
      throw new ArgumentOutOfRangeException(nameof(level));
    }
    
    _defaultDeviceEndpoint.MasterVolumeLevelScalar = level;
  }

  public bool IsDefaultDeviceMuted()
  {
    return _defaultDeviceEndpoint.Mute;
  }

  public void SetDefaultDeviceMuted(bool mute)
  {
    _defaultDeviceEndpoint.Mute = mute;
  }

  private void StartVolumeMonitoring()
  {
    _defaultDeviceEndpoint.OnVolumeNotification += OnVolumeChanged;
    _isVolumeMonitoring = true;
  }

  private void StopVolumeMonitoring()
  {
    _defaultDeviceEndpoint.OnVolumeNotification -= OnVolumeChanged;
    _isVolumeMonitoring = false;
  }

  private void OnVolumeChanged(AudioVolumeNotificationData data)
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

  public void RegisterVolumeChangeCallback(VolumeChangeCallback callback, string uuid)
  {
    if (!_isVolumeMonitoring)
    {
      StartVolumeMonitoring();
    }

    VolumeChangeCallbacks.Add(uuid, callback);
  }

  public void UnregisterVolumeChangeCallback(string uuid)
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

  public void ClearVolumeChangeCallbacks()
  {
    VolumeChangeCallbacks.Clear();
    StopVolumeMonitoring();
  }
}

