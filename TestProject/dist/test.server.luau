type List<T> = {
  read value: T,
  read next: List<T>?,
}
const function first<T>(list: List<T>): T
  return list.value
end
first({ value = 42, next = nil })
