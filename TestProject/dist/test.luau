type Instantiate<T> = (T) -> T
const function id<T>(x: T)
  return x
end
const idNumber: Instantiate<number> = (id :: never)
idNumber(69)
