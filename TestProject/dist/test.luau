const function fib(n: number): number
  if (n <= 1) then
    return n
  end
  return fib(n - 1) + fib(n - 2)
end
const result = fib(10)
print(result)
