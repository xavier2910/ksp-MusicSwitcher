# Configuring MusicSwitcher

This document lists all supported music controllers and how to configure them.

- [Configuring MusicSwitcher](#configuring-musicswitcher)
  - [`MUSICCONTROLLER_xxx`](#musiccontroller_xxx)
  - [`_FLIGHT` specific controllers](#_flight-specific-controllers)
    - [`SituationalShuffle`](#situationalshuffle)
    - [`SpaceShuffle`](#spaceshuffle)
  - [`_xxx` controllers](#_xxx-controllers)
    - [`QCtlr`](#qctlr)

## `MUSICCONTROLLER_xxx`

This should be suffixed with the relevant scene, like so: `MUSICCONTROLLER_FLIGHT` or `MUSICCONTROLLER_MAINMENU`. Currently
`_FLIGHT`, `_MAINMENU`, `_EDITOR`, and `_TRACKINGSTATION` are supported.

Fields common to all music controllers:
* `debugName = string-here` sets an internal debug name, to identify this controller in debug messages
* `typeName = Name.Space.Class` indicates the *fully qualified* name of the class to instantiate for
  this node \(via reflection\). This class **MUST** implement `IController` and **MUST** have a default
  constructor. MusicSwitcher ships with a few such classes, listed below.

## `_FLIGHT` specific controllers

These controllers can *only* be used as `_FLIGHT` controllers. Using them with other suffixes is undefined behavior!

### `SituationalShuffle`
```go
MUSICCONTROLLER_FLIGHT
{
    typeName = MusicSwitcher.Controller.SituationalShuffle
    situation = <situation id>
    // <situation id>'s:
    // LANDED      =   1
    // SPLASHED    =   2
    // PRELAUNCH   =   4
    // FLYING      =   8
    // SUB_ORBITAL =  16
    // ORBITING    =  32
    // ESCAPING    =  64
    // DOCKED      = 128
    volume = <float on [0,1]> // OPTIONAL default 1
    fadeoutDelta = <float on [0,1]> // applied per frame; small is probably good. OPTIONAL default 0.05
    trackPaths
    {
        Item = gdb/path/to/clip
        Item = path/to/other/clip
        // this may continue indefinitely
    }
}
```
Plays a shuffle of the specified music tracks while the active vessel is in the specified Vessel.Situations. Does fading.

### `SpaceShuffle`
```go
MUSICCONTROLLER_FLIGHT
{
    typeName = MusicSwitcher.Controller.SpaceShuffle
    fadeoutDelta = <float on [0,1]> // applied per frame; small is probably good. OPTIONAL default 0.05
    trackPaths
    {
        Item = gdb/path/to/clip
        Item = path/to/other/clip
        // this may continue indefinitely
    }
}
```
Shuffles the specified tracks and plays them while the active vessel is in space, just like in stock. But your music. Does fading.

## `_xxx` controllers

These controllers can be used in any scene without problems. N.B. that you must replace the `_xxx` with a valid scene suffix.

### `QCtlr`
```go
MUSICCONTROLLER_xxx
{
    typeName = MusicSwitcher.Controller.QCtlr
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