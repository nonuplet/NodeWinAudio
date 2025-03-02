import { JSVolumeNotification, JSNotificationType } from "../bin/NodeWinAudio"
import { NodeWinAudio } from "./NodeWinAudio"

console.log("hello from typescript!")

const cb = (args: JSVolumeNotification): void => {
  console.log("name: ", args.name)
  console.log("volume: ", args.masterVolume)
  console.log("muted: ", args.muted)
  console.log("type: ", JSNotificationType[args.type])
}

try {
  console.log("----- devices -----")
  console.log(NodeWinAudio.getAllDevices())

  console.log("----- volume -----")
  console.log(NodeWinAudio.getDefaultDeviceVolume() * 100)

  console.log("----- mute? -----")
  console.log(NodeWinAudio.isDefaultDeviceMuted())

  NodeWinAudio.registerVolumeChangeCallback(cb)

  // console.log("----- set volume 20 (0.2) -----")
  // nodeWinAudio.NodeWinAudioModule.setDefaultDeviceVolume(0.2)
  //
  // console.log("----- set mute -----")
  // nodeWinAudio.NodeWinAudioModule.setDefaultDeviceMuted(true)

  setInterval(() => {}, 1000)
  // setTimeout(() => {
  //   console.log("unregistered")
  //   NodeWinAudio.unregisterVolumeChangeCallback(cb)
  // }, 5000)
} catch (e) {
  console.error(e)
}
