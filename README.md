# Loom

[![CI Status](https://github.com/R-unic/loom/actions/workflows/ci.yml/badge.svg)](https://github.com/R-unic/loom/workflows)
[![Coverage Status](https://coveralls.io/repos/github/R-unic/loom/badge.svg?branch=master)](https://coveralls.io/github/R-unic/loom)

Domain-specific-language for Roblox that transpiles to Luau.

## Notes

This project is a work-in-progress. Nothing is final.

## Working Examples

```
let x: bool = false;
```
```luau
const x: boolean = false
```
##
```
mut x;
```
```luau
local x
```
##
```
let x = 1 & 2 & 3
```
```luau
local x = bit32.band(1, 2, 3)
```
##
```
type Id<T> = T
let x: Id<bool> = false;
```
```luau
type Id<T> = T
const x: Id<boolean> = false
```

## Planned Features

* Immutability by default
* Structural type system
* Enums
* Events
* Extended number literals
* Compile-time reflection
* Async support
* Timing blocks (`after`/`every`)
* ...and more
