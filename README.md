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
mut x = 1;
```
```luau
local x = 1
```
##
```rs
let s = "abc" + "def";
```
```luau
local s = "abc" .. "def"
```
##
```rs
let x = 1 & 2 & 3;
```
```luau
local x = bit32.band(1, 2, 3)
```
##
```rs
type Union<A, B> = A | B;
let x: Union<bool, string> = false;
```
```luau
type Union<A, B> = A | B
const x: Union<boolean, string> = false
```
##
```rs
let a = 10s;
let b = 100ms;
let c = 10m;
let d = 1h;
let e = 16hz;
let f = 100_000_000
let hex = 0xF00D;
let binary = 0b11001;
let octal = 0o400;
```
```luau
const a = 10
const b = 0.1
const c = 600
const d = 3600
const e = 0.0625
const f = 100000000
const hex = 61453
const binary = 25
const octal = 256
```
##
```rs
mut x = 69;
x = 420;
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
##
```rs
fn one -> 1;
```
```luau
const function one()
    return 1
end
```
##
```rs
fn id<T>(value: T) -> value;
```
```luau
const function id<T>(value: T)
    return value
end
```
##
```rs
fn id<T: number>(value: T): T {
    return value;
}
id::<number>(69)
```
```luau
const function id<T>(value: T & number): T & number
    return value
end
id(69)
```
##
```rs
let arr: number[] = [1, 2, 3];
```
```luau
const arr: { number } = {1, 2, 3}
```
##
```rs
let arr: number[mut] = mut [1, 2, 3];
```
```luau
const arr: { number } = {1, 2, 3}
```
##
```rs
let arr: number[mut] = mut [1, 2, 3];
let x = arr[1] = 69;
```
```luau
const arr: { number } = {1, 2, 3}
const x = 69
arr[1] = x
```
##
```rs
let abc = 69;
let name = nameof(abc)
```
```luau
const abc = 69;
const name = "abc"
```
##
```rs
let range = 1..10;
```
```luau
const range = { minimum = 1, maximum = 10 }
```
##
```rs
let range = 1..3;
let arr = [1, 2, 3, 4, 5];
let slice = arr[range];
```
```luau
const range = { minimum = 1, maximum = 3 }
const arr = {1, 2, 3, 4, 5}
const _length = #arr
const slice = table.move(arr, math.clamp(range.minimum, 1, _length), math.clamp(range.maximum, 1, _length), 1, {})
```
##
```rs
let arr = [1, 2, 3, 4, 5];
let slice = arr[1..3];
```
```luau
const arr = {1, 2, 3, 4, 5}
const _length = #arr
const slice = table.move(arr, math.clamp(1, 1, _length), math.clamp(3, 1, _length), 1, {})
```
##
```rs
let s = "abcdef";
let slice = s[1..3];
```
```luau
const s = "abcdef"
const slice = string.sub(s, 1, 3)
```
##
```rs
let s = "abcdef";
let char = s[1];
```
```luau
const s = "abcdef"
const char = string.sub(s, 1, 1)
```
##
```rs
let min = (1..10).minimum;
```
```luau
const min = ({ minimum = 1, maximum = 10 }).minimum
```
##
```rs
let range = 1..10;
let name = nameof(range.minimum);
```
```luau
const range = { minimum = 1, maximum = 10 }
const name = "range.minimum"
```
##
```rs
enum Abc { A, B = 69, C }
let a = Abc.A;
let b = Abc.B;
let c = Abc.C;
```
```luau
type Abc = number
const a = 0
const b = 69
const c = 70
```
##
```rs
enum Tag: string {
    Lava = "lava",
    Something = "something"
}
let tag = Tag.Lava
```
```luau
type Tag = "lava" | "something"
const tag = "lava"
```
##
```rs
if 69 == 420 {
    let foo = 69
} else if 69 == 69 {
    let yes = "yes"
}
```
```luau
if 69 == 420 then
    const foo = 69
elseif 69 == 69 then
    const yes = "yes"
end
```
##
```rs
declare fn print(msg: unknown): void;
print("hello, world!");
```
```luau
print("hello, world!")
```
##
```ts
declare let x: number;
let y = x + 1;
```
```luau
const y = x + 1
```
##
```rs
let unknown = 69 as unknown;
```
```luau
const unknown = (69 :: unknown)
```
##
```rs
type Callback = fn(): void
```
```luau
type Callback = () -> ()
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
