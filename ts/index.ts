import * as dotnet from "node-api-dotnet"
import type * as NodeWinAudio from "../bin/NodeWinAudio"

console.log("hello from typescript!")

const nodeWinAudio = dotnet.require("./bin/NodeWinAudio") as typeof NodeWinAudio
try {
  console.log("----- devices -----")
  console.log(nodeWinAudio.NodeWinAudioModule.getAllDevices())

  console.log("----- volume -----")
  console.log(nodeWinAudio.NodeWinAudioModule.getDefaultDeviceVolume() * 100)

  console.log("----- mute? -----")
  console.log(nodeWinAudio.NodeWinAudioModule.isDefaultDeviceMuted())

  console.log("----- set volume 20 (0.2) -----")
  nodeWinAudio.NodeWinAudioModule.setDefaultDeviceVolume(0.2)

  console.log("----- set mute -----")
  nodeWinAudio.NodeWinAudioModule.setDefaultDeviceMuted(true)
} catch (e) {
  console.error(e)
}
