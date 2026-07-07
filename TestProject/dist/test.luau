type Def = {
  read abc: string,
}
type Abc = Def & {
  read abc: number,
}
const abc = { abc = 69 }
