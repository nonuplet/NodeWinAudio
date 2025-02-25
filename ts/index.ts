import * as dotnet from "node-api-dotnet"
import type * as NodeWinAudio from "../bin/NodeWinAudio"

console.log("hello from typescript!");

const nodeWinAudio = dotnet.require("./bin/NodeWinAudio") as typeof NodeWinAudio;
console.log(nodeWinAudio.HelloWorld.sayHello(".NET"));
