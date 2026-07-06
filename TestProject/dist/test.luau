type Expression = {}
type Binary = Expression & {
  read left: Expression,
  read right: Expression,
}
