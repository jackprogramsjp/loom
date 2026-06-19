type Id<T> = T
const y: Id<number> = 69
const _ = y
type Record<K, V> = {
  [K]: V
  read size: K
}
const x = ((nil :: never) :: Record<string, boolean>)
const _ = x.size
