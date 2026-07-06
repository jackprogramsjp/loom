type A = {
  read a: { number },
}
type B = {
  read b: A,
}
type C = {
  read c: B,
}
const object = { c = { b = { a = {1, 2, 3} } } }
const _ = table.concat(object["c"].b.a)
