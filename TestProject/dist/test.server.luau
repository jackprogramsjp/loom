type FooProp = index<Foo, "foo">
const x: FooProp = 69
const _ = x + 69
type Foo = {
  read foo: number,
}
