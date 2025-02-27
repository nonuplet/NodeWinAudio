import * as dotnet from "node-api-dotnet"
import type * as NodeWinAudio from "../bin/NodeWinAudio"

console.log("hello from typescript!")

const nodeWinAudio = dotnet.require("./bin/NodeWinAudio") as typeof NodeWinAudio
console.log("----- devices -----")
console.log(nodeWinAudio.NodeWinAudioModule.getDevices())
console.log("----- volume -----")
try {
  console.log(nodeWinAudio.NodeWinAudioModule.getVolume() * 100)
} catch (e) {
  console.error(e)
}
