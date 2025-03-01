import * as dotnet from "node-api-dotnet"
import type * as NodeWinAudioModule from "../bin/NodeWinAudio"
import { JSVolumeNotification } from "../bin/NodeWinAudio"
import { UUID } from "node:crypto"

export class NodeWinAudio {
  private static winAudio = dotnet.require("./bin/NodeWinAudio")
    .NodeWinAudioModule as typeof NodeWinAudioModule.NodeWinAudioModule

  private static callbacks = new Map<(data: JSVolumeNotification) => void, UUID>()

  public static getAllDevices(): string[] {
    return this.winAudio.getAllDevices()
  }

  public static getDefaultDeviceVolume(): number {
    return this.winAudio.getDefaultDeviceVolume()
  }

  public static setDefaultDeviceVolume(value: number) {
    this.winAudio.setDefaultDeviceVolume(value)
  }

  public static isDefaultDeviceMuted(): boolean {
    return this.winAudio.isDefaultDeviceMuted()
  }

  public static setDefaultDeviceMuted(value: boolean) {
    this.winAudio.setDefaultDeviceMuted(value)
  }

  public static registerVolumeChangeCallback(callback: (params: JSVolumeNotification) => void) {
    if (this.callbacks.has(callback)) {
      throw new Error("This callback is already registered.")
    }

    const uuid = crypto.randomUUID()
    this.callbacks.set(callback, uuid)
    this.winAudio.registerVolumeChangeCallback(callback, uuid.toString())
  }

  public static unregisterVolumeChangeCallback(callback: (params: JSVolumeNotification) => void) {
    const uuid = this.callbacks.get(callback)
    if (uuid === undefined) {
      throw new Error("This callback is not registered.")
    }

    this.winAudio.unregisterVolumeChangeCallback(uuid.toString())
    this.callbacks.delete(callback)
  }

  public static clearVolumeChangeCallback() {
    this.winAudio.clearVolumeChangeCallbacks()
    this.callbacks.clear()
  }
}
