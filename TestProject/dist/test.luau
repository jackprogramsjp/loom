type Result<T, Error> = {
  read value: T?
  read error: Error?
}
const function ok<T, Error>(value: T): Result<T, Error>
  return { value = value, error = ((nil :: never) :: Error) }
end
type MyErrors = "do_something failed to execute"
const function do_something(): Result<number, MyErrors>
  return ok(69)
end
const result = do_something()
if result.value ~= nil then
  print(result.error)
end
