# Configuring MusicSwitcher

This document lists all supported music Controllers and how to configure them.

- [Configuring MusicSwitcher](#configuring-musicswitcher)
  - [MUSICCONTROLLER\_FLIGHT](#musiccontroller_flight)
    - [SpaceShuffle](#spaceshuffle)
    - [QCtlr](#qctlr)

## MUSICCONTROLLER_FLIGHT

Fields common to all Flight Controllers:
* `debugName = string-here` sets an internal debug name, to identify this node in debug messages
* `typeName = Name.Space.Class` indicates the *fully qualified* name of the class to instantiate for
  this node \(via reflection\). This class **MUST** implement `IController` and **MUST** have a default
  constructor. MusicSwitcher ships with a few such classes:

### SpaceShuffle
```
MUSICCONTROLLER_FLIGHT
{
    typeName = MusicSwitcher.Controllers.SpaceShuffle
    trackPaths
    {
        Item = gdb/path/to/clip
        Item = path/to/other/clip
        // this may continue indefinitely
    }
}
```
Shuffles the specified tracks and plays them while the active vessel is in space, just like in stock. But your music.

### QCtlr
```
MUSICCONTROLLER_FLIGHT
{
    typeName = MusicSwitcher.Controllers.QCtlr
    trackPaths
    {
        Item = gdb/path/to/clip
        Item = path/to/other/clip
        // this may continue indefinitely
    }
}
```
Honestly, you probably don't want this one. It just queues up the specified music tracks and plays them in sequence. It
does not loop.