import { NodeWinAudio } from "../ts/NodeWinAudio"

test("get device info", () => {
  expect(NodeWinAudio.getAllDevices()).toBeDefined()
  const volume = NodeWinAudio.getDefaultDeviceVolume()
  expect(volume).toBeGreaterThanOrEqual(0)
  expect(volume).toBeLessThanOrEqual(1)
  expect(NodeWinAudio.isDefaultDeviceMuted()).toBeDefined()
})

test("set volume", () => {
  const currentVolume = NodeWinAudio.getDefaultDeviceVolume()

  const newVolume = Math.floor(Math.random() * 100) * 0.01
  console.log(`Setting volume to ${newVolume} ...`)
  NodeWinAudio.setDefaultDeviceVolume(newVolume)
  const deviceVolume = Math.round(NodeWinAudio.getDefaultDeviceVolume() * 100) * 0.01

  NodeWinAudio.setDefaultDeviceVolume(currentVolume) // reset

  expect(deviceVolume).toEqual(newVolume)
})

test("set mute", () => {
  const currentMuteState = NodeWinAudio.isDefaultDeviceMuted()

  const newMuteState = !currentMuteState
  console.log(`Setting mute to ${newMuteState} ...`)
  NodeWinAudio.setDefaultDeviceMuted(newMuteState)
  const deviceMuteState = NodeWinAudio.isDefaultDeviceMuted()

  NodeWinAudio.setDefaultDeviceMuted(currentMuteState) // reset

  expect(deviceMuteState).toEqual(newMuteState)
})
