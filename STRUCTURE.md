# Mod Structure

This is about the internal structure of the mod code, meant for folks who want to write code. This is not
about config-writing or any such things. For those things, see [the config docs](CONFIGURING.md).

## Overview

There are three broad chunks of this program: the `MusicSwitcher`, its music `IController`s, and the "Behaviors" that create the
latter at runtime. The `MusicSwitcher` takes care of the relevant `AudioSource`s and the dis-/enabling of the stock music.
`IController` is an interface; they control the actual playing (or not) of `AudioClip`s. The job of a "Behavior" (it's a
bad name; I really need a better term for this. "Configurators"?) is to create and configure the various `IController`s
and give them to the `MusicSwitcher`.

## The `MusicSwitcher`

This is kind of the orchestrator of the whole thing, alongside its lackeys, `Patcher`, `SettingsLoader`, `StockMusicExtractor`, and
`AudioSourceWrangler`. The first three are only relevant at startup: `Patcher` finds the StockMusicLogic GameObject, instantiates the
`MusicSwitcher`, and attatches it to the MusicLogic. The `MusicSwitcher` then sets up the `StockMusicExtractor`, which extracts
references to many of the stock music `AudioClip`s. Meanwhile, the `SettingsLoader` loads the global settings.

The `MusicSwitcher` contains a variety of methods for configurating music at runtime. Part of this dance is the `AudioSourceWrangler`,
which gets instantiated by the `MusicSwitcher` and is the way that `IController`s should get references to their `AudioSources`.
Basically, this just keeps track of which ones are being used when so that we don't just keep creating `AudioSource` components
all over the place every scene change/load.

Lurking behind all this is `Statics`. It's just a place to hold constant plugin data that don't really belong to specific classes
or need to be shared internally or externally. All constants or set-once's live here. Generally.

## The `IController` interface

These are what can be actually configured by .cfg files and do the actual lifting when it comes to the dynamic behavior of the
music. They are sort of small, context specific \(as far as possible\) controllers for the music.

## "Behaviors"

Right now these are just organized by scene. With the exception of the Flight scene "Behavior", these all simply allow the Stock
music to just do its own thing. The FlightMusic one deserves a brief overview, however. It loads up configs for flight-time
`IControllers` and instantiates them. That's really it. See [the config docs](CONFIGURING.md).
