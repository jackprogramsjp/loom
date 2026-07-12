const Loom = require("@game/ReplicatedStorage/include/loom_runtime")
type ToString = {
  to_string: (ToString) -> string,
}
type User = {
  read name: string,
  read age: number,
} & ToString
local ToString_for_User = {}
ToString_for_User.__index = ToString_for_User
ToString_for_User = ToString_for_User :: User
function ToString_for_User.to_string(self: User)
  return "User" .. " { name: ''" .. self.name .. "', age: " .. tostring(self.age) .. " }"
end
const user = setmetatable({ name = "Runic", age = 21 }, Loom.merge_meta(ToString_for_User)) :: User
print(user:to_string())