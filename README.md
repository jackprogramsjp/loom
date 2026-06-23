# Loom

[![CI Status](https://github.com/R-unic/loom/actions/workflows/ci.yml/badge.svg)](https://github.com/R-unic/loom/workflows)
[![Coverage Status](https://coveralls.io/repos/github/R-unic/loom/badge.svg?branch=master)](https://coveralls.io/github/R-unic/loom)
[![License: Apache-2.0](https://img.shields.io/badge/License-Apache--2.0-yellow.svg)](https://opensource.org/licenses/apache-2.0)

**A domain-specific language for Roblox that transpiles to Luau.**

> ⚠️ This project is a work-in-progress. Nothing is final. Breaking changes may occur at any time.

## Features

- **Immutability by default** – Variables, fields, and arrays are immutable unless explicitly marked `mut`
- **Structural type system** – Duck typing with compile-time safety
- **Modern syntax** – Familiar syntax inspired by Rust and TypeScript
- **Rich type inference** – Minimal annotations required
- **Extended number literals** – Automatic math for units of time and frequency, as well as binary/octal/hex support
- **Range expressions** – `1..10` for slicing and bounds
- **Assignment as expression** – `let x = a = b = 1`
- **`nameof` operator** – Get names as strings at compile time
- **Generic functions and types** – Full support for type parameters including constraints and defaults
- **Sealed interfaces** – Prevent interfaces from being used as constraints
- **Zero-cost abstractions** – Transpiles to idiomatic Luau with minimal overhead

## Upcoming Features
- Ternary operator
- `typeof`
- Implementors for interfaces
- Private visibility for interface fields & methods
- Event declarations
- Full module system (imports/exports)
- Error handling using the result pattern

---

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
Loom supports extended number literals that let you do boilerplate math to convert to a specific unit instantaneously.
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
Loom supports shorthand function bodies that return single expressions.
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
Arrays are immutable by default, but can be declared as mutable.
```rs
let arr: number[mut] = mut [1, 2, 3];
```
```luau
const arr: { number } = {1, 2, 3}
```
##
Assignments are expressions in loom.
```rs
let arr = mut [1, 2, 3];
let x = arr[1] = 69;
```
```luau
const arr: { number } = {1, 2, 3}
const x = 69
arr[1] = x
```
##
The `nameof` operator can be used to read the tokens of `Name` expressions as a string.
```rs
let abc = 69;
let name = nameof(abc)
```
```luau
const abc = 69;
const name = "abc"
```
##
Ranges are constructs that represent a minimum and a maximum number.
```rs
let range = 1..10;
```
```luau
const range = { minimum = 1, maximum = 10 }
```
##
They can be used to slice arrays.
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
As well as strings.
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
Enums are named compile-time constants.
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
They can also be used with strings.
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
Declare statements allow you to declare types for symbols that may not exist in your file but you know exist in your environment.
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
##
```ts
interface HasName {
    name: string;
}
interface HasAge {
    age: number;
}
interface Person: HasName, HasAge {
    job: string;
}
```
```luau
type HasName = {
	read name: string;
}
type HasAge = {
	read age: number;
}
type Person = HasName & HasAge & {
	read job: string;
}
```
##
```ts
interface ImmutRecord<K, V> {
    [K]: V;
}
```
```luau
type ImmutRecord<K, V> = { read [K]: V }
```
##
In this example `S` resolves to `number`.
```ts
interface Foo { bar: string }
type S = Foo["bar"];
```
```luau
type Foo = {
    read bar: string
}
type S = index<Foo, "bar">
```
##
```ts
interface Person {
    name: string;
    mut age: number;
}

let runic = new Person { name: "Runic", age: 21 };
runic.age = 69;
```
```luau
type Person = {
	read name: string,
	age: number
}
const runic = { name = "Runic", age = 21 }
runic.age = 69
```
##
```rs
mut i = 0;
while i < 10
    i += 1;
    
print(i)
```
```luau
local i = 0
while i < 10 do
    i += 1
end
print(i)
```
##
In this example Foo is only a type and cannot be instantiated.
```ts
declare interface Foo { bar: string }
```
```luau
type Foo = {
    read bar: string
}
```
##
In this example Foo cannot be used as a constraint to other interfaces.
```cs
sealed interface Foo { bar: string }
```
```luau
type Foo = {
    read bar: string
}
```
##
After statements are a shorthand to `task.delay`. They **never yield**.
```cs
after 100ms {
    print("done!");
}
```
```luau
task.delay(0.1, function(): ()
    print("done!")
end)
```

---

## Contributing

Contributions are welcome! Please read our [Contributing Guide](CONTRIBUTING.md) for details on the process for submitting pull requests and building language features.

---

## License

This project is licensed under the Apache-2.0 License - see the [LICENSE](LICENSE) file for details.