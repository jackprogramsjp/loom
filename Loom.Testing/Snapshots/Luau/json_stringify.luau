type User = {
  read name: string,
  read age: number,
  read tags: { string },
}
const function stringify(user: User): string
  return "{\n" .. "  name: '" .. user.name .. "',\n" .. "  age: " .. tostring(user.age) .. ",\n" .. "  tags: [" .. table.concat(user.tags, ", ") .. "]\n" .. "}"
end
const user = { name = "Alice", age = 20, tags = {"admin", "staff"} }
print(stringify(user))