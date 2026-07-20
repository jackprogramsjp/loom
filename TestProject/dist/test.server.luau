type Foo = {
  read do_foo: (Foo) -> (),
}
type Bar = {
  read foo: Foo,
}
const bar = (nil :: never) :: Bar
bar.foo:do_foo()
