type Foo = {
  read foo: string
}
type Bar = {
  read foo: number
}
const x: Bar | Foo = (nil :: never)
const _ = x.foo
