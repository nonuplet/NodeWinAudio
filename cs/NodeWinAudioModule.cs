using Microsoft.JavaScript.NodeApi;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace NodeWinAudio.cs;

[JSExport]
public class NodeWinAudioModule
{
  public static NodeWinAudioModule Instance { get; } = new(); // Singleton

  private MMDevice _defaultDevice;
  private AudioEndpointVolume _defaultDeviceEndpoint;
  private readonly DeviceNotification _deviceNotification;
  private readonly Dictionary<string, VolumeChangeCallback> _volumeChangeCallbacks = new();

  public delegate void VolumeChangeCallback(JSVolumeNotification data);

  private class DeviceNotification : IMMNotificationClient
  {
    public void OnDeviceStateChanged(string deviceId, DeviceState newState)
    {
      throw new NotImplementedException();
    }

    public void OnDeviceAdded(string pwstrDeviceId)
    {
      throw new NotImplementedException();
    }

    public void OnDeviceRemoved(string deviceId)
    {
      throw new NotImplementedException();
    }

    public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
    {
      if (flow == DataFlow.Render && role == Role.Console)
      {
        Instance.OnDefaultDeviceChanged();
      }
    }

    public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
    {
      throw new NotImplementedException();
    }
  }

  private NodeWinAudioModule()
  {
    using var enumerator = new MMDeviceEnumerator();
    _defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
    _defaultDeviceEndpoint = _defaultDevice.AudioEndpointVolume;

    /*
      This is a hack to resolve the COM issue.
      https://github.com/naudio/NAudio/issues/425
     */
    _ = _defaultDevice.FriendlyName; // OMG
    _deviceNotification = new DeviceNotification();
    enumerator.RegisterEndpointNotificationCallback(_deviceNotification);
  }

  private void SetDefaultDevice()
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
  }

  private void StopVolumeMonitoring()
  {
    _defaultDeviceEndpoint.OnVolumeNotification -= OnVolumeChanged;
  }

  private void OnVolumeChanged(AudioVolumeNotificationData data)
  {
    OnVolumeChangedJS(new JSVolumeNotification(
      _defaultDevice,
      data.Muted,
      data.MasterVolume,
      JSNotificationType.VolumeChanged
    ));
  }

  private void OnVolumeChangedJS(JSVolumeNotification notification)
  {
    foreach (var callback in _volumeChangeCallbacks.Values)
    {
      callback(notification);
    }
  }

  public void RegisterVolumeChangeCallback(VolumeChangeCallback callback, string uuid)
  {
    if (_volumeChangeCallbacks.Count == 0)
    {
      StartVolumeMonitoring();
    }

    _volumeChangeCallbacks.Add(uuid, callback);
  }

  public void UnregisterVolumeChangeCallback(string uuid)
  {
    if (!_volumeChangeCallbacks.ContainsKey(uuid))
    {
      throw new KeyNotFoundException();
    }

    _volumeChangeCallbacks.Remove(uuid);
    if (_volumeChangeCallbacks.Count == 0)
    {
      StopVolumeMonitoring();
    }
  }

  public void ClearVolumeChangeCallbacks()
  {
    _volumeChangeCallbacks.Clear();
    StopVolumeMonitoring();
  }

  private void OnDefaultDeviceChanged()
  {
    // Prev Device
    StopVolumeMonitoring();

    // New Device
    SetDefaultDevice();
    StartVolumeMonitoring();

    // Call VolumeChangeCallbacks
    var notification = new JSVolumeNotification(
      _defaultDevice,
      _defaultDeviceEndpoint.Mute,
      _defaultDeviceEndpoint.MasterVolumeLevelScalar,
      JSNotificationType.DeviceChanged
    );
    OnVolumeChangedJS(notification);
  }
}
