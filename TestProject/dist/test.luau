type Foo = {
  [number]: boolean
  read foo: number
}
const x = ((nil :: never) :: Foo)
x.foo = 69
