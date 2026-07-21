const world = game:GetService("Workspace")
const is_root = world:IsA("WorldRoot")
const part = world:FindFirstChildOfClass("Part")
if part ~= nil then
  print(part.name)
end
