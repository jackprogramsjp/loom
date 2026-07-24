const Loom = require("@game/ReplicatedStorage/include/loom_runtime")
const score_changed: Loom.Event<number> = Loom.Event.new()
const function on_score(n: number): ()
  print("score is now " .. tostring(n))
end
const on_score_conn = score_changed:Connect(on_score)
score_changed:Fire(100)
