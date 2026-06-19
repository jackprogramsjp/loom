type Struct<T> = {
  read data: T
}
const _ = ((1 :: never) :: Struct<number>)
