const world = game:GetService("Workspace")
const children = world:GetChildren()
const _result: { BasePart } = {}
for _, child in world:GetChildren() do
  if not child:IsA("BasePart") then continue end
  table.insert(_result, child)
end
const parts = _result
const descendants = world:GetDescendants()
const _result_1: { BasePart } = {}
for _, child in world:GetDescendants() do
  if not child:IsA("BasePart") then continue end
  table.insert(_result_1, child)
end
const descendant_parts = _result_1