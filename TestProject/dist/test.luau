type A = {
  read kind: "A"
  read val: number
}
type B = {
  read kind: "B"
  read val: string
}
type Inner = A | B
type Outer = {
  read inner: Inner
}
const o = ((nil :: never) :: Outer)
if o.inner.kind == "A" then
  const _ = o.inner.val + 1
end
