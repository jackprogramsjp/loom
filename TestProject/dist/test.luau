type ResultKind = number
type ResultOk<T> = {
  read kind: index<ResultKind, "Ok">
  read value: T
}
type ResultError<Error> = {
  read kind: index<ResultKind, "Err">
  read error: Error
}
type Result<T, Error> = ResultOk<T> | ResultError<Error>
const function ok<T, Error>(value: T): Result<T, Error>
  return { kind = 0, value = value }
end
type MyErrors = "do_something failed to execute"
const function do_something(): Result<number, MyErrors>
  return ok(69)
end
const result = do_something()
if result.kind == 1 then
  print("do_something failed to execute")
end
