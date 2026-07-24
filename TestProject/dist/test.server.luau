const world = game:GetService("Workspace")
const part = world:FindFirstChildOfClass("Part")
const function on_touch(hit: Loom.BasePart)
  return print(hit)
end
if part ~= nil then
  part.touched += on_touch
end
