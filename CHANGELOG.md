# Changelog for MusicSwitcher

## Unreleased

- Takes over stock music and replaces it with your own. The loader is able to load controllers for any scene,
  but currently only the following are supported:
  - Flight
  - Main Menu
  - Editor
  - Tracking Station
- Music may be configured in .cfg files using a variety of controllers. Only flight-relevant controllers have been written,
  such as:
  - replacing stock in-space music
  - running different playlists when the active vessel is in different situations
  See [the configuring docs](CONFIGURING.md) for more info.
- Track fading on transitions and pauses is very doable and done.