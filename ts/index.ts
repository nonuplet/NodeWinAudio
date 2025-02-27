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
} catch (e) {
  console.error(e)
}
