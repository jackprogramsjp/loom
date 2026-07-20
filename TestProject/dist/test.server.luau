type Base = {
  read my_method: (Base) -> (),
}
type Derived = Base & {}
const d = (nil :: never) :: Derived
d:my_method()
