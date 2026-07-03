type Abc<T, U> = {
  read t: T
  read u: U
}
const function abc<T, U>(t: T): Abc<T, number>
  return { t = t, u = 69 }
end
abc(69)
