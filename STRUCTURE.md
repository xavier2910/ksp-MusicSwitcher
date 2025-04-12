# Mod Structure

This is about the internal structure of the mod code, meant for folks who want to write code. This is not
about config-writing or any such things.

## Overview

There are three broad chunks of this program: the `MusicSwitcher`, its `MusicController`s, and the "Behaviors" that create the
latter at runtime. The `MusicSwitcher` takes care of the relevant `AudioSource`s and the application of global settings to them.
`MusicController` is an interface; they control the actual playing (or not) of `AudioClip`s. The job of a "Behavior" (it's a
bad name; I really need a better term for this. "Configurators"?) is to create and configure the various `MusicController`s
and give them to the `MusicSwitcher`.

## The `MusicSwitcher`

## The `MusicController` interface

## "Behaviors"


