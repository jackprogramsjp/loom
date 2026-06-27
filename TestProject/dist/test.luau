type Set<T> = {
  read cache: MutRecord<T, boolean>
}
const function set_add<T>(set: Set<T>, item: T)
  return set.cache[item] = true
end
const function set_delete<T>(set: Set<T>, item: T)
  return set.cache[item] = (nil :: never)
end
const function set_has<T>(set: Set<T>, item: T): boolean
  return set.cache[item] ~= nil
end
const function set_id<T>(set: Set<T>): Set<T>
  return set
end
const names = { cache = {} }
set_add(names, "runic")
set_add(names, "xeon")
print(set_has(names, "runic"))
