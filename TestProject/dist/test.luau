type Container = {
  read arr: { number },
  read range: Range,
}
const container = { arr = {1, 2, 3}, range = { minimum = 1, maximum = 10 } }
print(#container.arr)
print(table.concat(container))
print(#container.arr)
print(#container.arr)
print(1 + math.abs(container.range.maximum - container.range.minimum))
print(math.clamp(69, container.minimum, container.maximum))
print(math.clamp(69, container.range.minimum, container.range.maximum))
const r = { minimum = 1, maximum = 10 }
math.clamp(69, r.minimum, r.maximum)
