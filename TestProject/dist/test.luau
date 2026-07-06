type Container = {
  read arr: { number },
  read range: Range,
}
const container = { arr = {1, 2, 3}, range = { minimum = 1, maximum = 10 } }
print(#container.arr)
print(table.concat(container.arr))
print(#container.arr)
print(#container.arr)
const _range = container.range
print(1 + math.abs(_range.maximum - _range.minimum))
const _range = container.range
print(math.clamp(69, _range.minimum, _range.maximum))
const _range = container["range"]
print(math.clamp(69, _range.minimum, _range.maximum))
const r = { minimum = 1, maximum = 10 }
math.clamp(69, r.minimum, r.maximum)
