type Data = {
  read name: string
  read age: number
  read dob: string
}
for v in { name = "Runic", age = 21, dob = "11/23/04" } do
  print(v)
end
