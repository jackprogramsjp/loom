const function unsafe_function(condition: boolean): Result<number, string>
  return if condition then { kind = 0, value = 69 } else { kind = 1, error = "function failed!" }
end
const result = unsafe_function(true)
print(if result.kind == 0 then result.value else result.error)
