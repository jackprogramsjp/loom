type Foo = {
  read do_foo: (Foo) -> (),
}
type Bar = {
  read foo: Foo,
}
type Baz = {
  read do_baz: (Baz, number) -> (),
}
const bar = (nil :: never) :: Bar
bar.foo:do_foo()
const baz = (nil :: never) :: Baz
baz:do_baz(69)