type Def = {
  read def: string,
}
type Abc = Def & {
  read abc: number,
}
const abc = { abc = 69, def = "foo" }
