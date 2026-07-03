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
