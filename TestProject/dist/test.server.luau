type EventObject = {}
const function on_consumer(p: string): ()
  print("param: " .. p)
end
const eo = (nil :: never) :: EventObject
eo.consumer += on_consumer
eo.consumer("abc")
eo.consumer -= on_consumer
