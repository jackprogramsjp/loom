const s = "abcdef"
const _length = #s
string.sub(s, math.clamp(1, 1, _length), math.clamp(3, 1, _length))
