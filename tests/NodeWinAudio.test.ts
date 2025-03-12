import { NodeWinAudio } from "../ts/NodeWinAudio"
import { JSVolumeNotification } from "../bin/NodeWinAudio"

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

test("register callback", async () => {
  const currentVolume = NodeWinAudio.getDefaultDeviceVolume()
  const newVolume = Math.floor(Math.random() * 100) * 0.01

  const mockCallback = jest.fn((params: JSVolumeNotification) => {
    console.log(`${params.name}, ${params.masterVolume}, ${params.muted}, ${params.type}`)
  })

  // register check
  NodeWinAudio.registerVolumeChangeCallback(mockCallback)
  NodeWinAudio.setDefaultDeviceVolume(newVolume)
  await new Promise((r) => setTimeout(r, 500))
  expect(mockCallback).toHaveBeenCalledTimes(1)

  // unregister check
  NodeWinAudio.unregisterVolumeChangeCallback(mockCallback)
  NodeWinAudio.setDefaultDeviceVolume(newVolume)
  await new Promise((r) => setTimeout(r, 500))
  expect(mockCallback).toHaveBeenCalledTimes(1)

  // clear check
  const mockCallback2 = jest.fn((params: JSVolumeNotification) => {
    console.log(`${params.name}, ${params.masterVolume}, ${params.muted}, ${params.type}`)
  })
  NodeWinAudio.registerVolumeChangeCallback(mockCallback)
  NodeWinAudio.registerVolumeChangeCallback(mockCallback2)
  NodeWinAudio.clearVolumeChangeCallback()
  await new Promise((r) => setTimeout(r, 500))
  NodeWinAudio.setDefaultDeviceVolume(newVolume)
  expect(mockCallback).toHaveBeenCalledTimes(1)
  expect(mockCallback2).toHaveBeenCalledTimes(0)

  NodeWinAudio.setDefaultDeviceVolume(currentVolume) // Reset
})
