# Loom

[![CI Status](https://github.com/R-unic/loom/actions/workflows/ci.yml/badge.svg)](https://github.com/R-unic/loom/workflows)
[![Coverage Status](https://coveralls.io/repos/github/R-unic/loom/badge.svg?branch=master)](https://coveralls.io/github/R-unic/loom)

Domain-specific-language for Roblox that transpiles to Luau.

## Notes

This project is a work-in-progress. Nothing is final.

## Working Examples

```rs
let x: bool = false;
```
```luau
const x: boolean = false
```
##
```rs
mut x;
```
```luau
local x
```
##
```rs
let x = 1 & 2 & 3
```
```luau
local x = bit32.band(1, 2, 3)
```
##
```rs
type Id<T> = T
let x: Id<bool> = false;
```
```luau
type Id<T> = T
const x: Id<boolean> = false
```
##
```rs
let a = 10s
let b = 100ms
let c = 10m
let d = 1h
let e = 16hz
let hex = 0xF00D
let binary = 0b11001
let octal = 0o400
```
```luau
const a = 10
const b = 0.1
const c = 600
const d = 3600
const e = 0.0625
const hex = 61453
const binary = 25
const octal = 256
```
##
```rs
mut x = 69;
x = 420
```
```luau
local x = 69
x = 420
```
##
```rs
mut x = 69;
mut y = 420;
let z = x = y = 1;
```
```luau
local x = 69
local y = 420
y = 1
x = y
const z = x
```

## Planned Features

* Immutability by default
* Structural type system
* Enums
* Events
* Extended number literals for time
* Compile-time reflection
* Async support
* Timing blocks (`after`/`every`)
* ...and more
