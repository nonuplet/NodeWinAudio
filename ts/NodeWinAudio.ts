import * as dotnet from "node-api-dotnet"
import { JSVolumeNotification, NodeWinAudioModule } from "../bin/NodeWinAudio"
import { UUID } from "node:crypto"

export class NodeWinAudio {
  private static winAudio = dotnet.require("./bin/NodeWinAudio").NodeWinAudioModule.instance as NodeWinAudioModule

  private static callbacks = new Map<(data: JSVolumeNotification) => void, UUID>()

  /**
   * Lists names of all active audio devices.
   * @returns Array of device names
   */
  public static getAllDevices(): string[] {
    return this.winAudio.getAllDevices()
  }

  /**
   * Gets the default device volume level.
   * @returns Volume level (0.0 - 1.0)
   */
  public static getDefaultDeviceVolume(): number {
    return this.winAudio.getDefaultDeviceVolume()
  }

  /**
   * Sets the default device volume level.
   * @param value Volume level (0.0 - 1.0)
   */
  public static setDefaultDeviceVolume(value: number) {
    this.winAudio.setDefaultDeviceVolume(value)
  }

  /**
   * Check if the default audio device is muted.
   * @return `true` if muted, `false` otherwise.
   */
  public static isDefaultDeviceMuted(): boolean {
    return this.winAudio.isDefaultDeviceMuted()
  }

  /**
   * Sets the mute state of default audio device.
   * @param value `true` to mute, `false` to unmute.
   */
  public static setDefaultDeviceMuted(value: boolean) {
    this.winAudio.setDefaultDeviceMuted(value)
  }

  /**
   * Registers a callback function to be invoked when the volume level of default device changes.
   *
   * The registered callback function will be called when either:
   * - The volume level of default device changes. {@link JSVolumeNotification.type} = `VolumeChanged`
   * - The default device is changes. {@link JSVolumeNotification.type} = `DeviceChanged`
   *
   * @param callback The callback function to register. Invoked with a {@link JSVolumeNotification} object.
   * @throws {Error} If callback already registered.
   */
  public static registerVolumeChangeCallback(callback: (params: JSVolumeNotification) => void) {
    if (this.callbacks.has(callback)) {
      throw new Error("This callback is already registered.")
    }

    const uuid = crypto.randomUUID()
    this.callbacks.set(callback, uuid)
    this.winAudio.registerVolumeChangeCallback(callback, uuid.toString())
  }

  /**
   * Unregisters a callback function when the volume level of default device changes.
   *
   * @param callback The callback function to unregister.
   * @throws Error If callback is not registered.
   */
  public static unregisterVolumeChangeCallback(callback: (params: JSVolumeNotification) => void) {
    const uuid = this.callbacks.get(callback)
    if (uuid === undefined) {
      throw new Error("This callback is not registered.")
    }

    this.winAudio.unregisterVolumeChangeCallback(uuid.toString())
    this.callbacks.delete(callback)
  }

  /**
   * Clear volume change callbacks.
   * Removes all registered callbacks.
   */
  public static clearVolumeChangeCallback() {
    this.winAudio.clearVolumeChangeCallbacks()
    this.callbacks.clear()
  }
}
