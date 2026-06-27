type I = {
  read a: number
  read b: string
}
type K = "a" | "b"
type V = index<I, K>
const x: V = 42
const _ = x
