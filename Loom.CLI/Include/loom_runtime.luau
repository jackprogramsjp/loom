--!strict
--!native
--!optimize 2
const Loom = {}

export type Range = { minimum: number, maximum: number }
export type ResultOk<T> = { ok: true, value: T }
export type ResultError<Error> = { ok: false, error: Error }
export type Result<T, Error> = ResultOk<T> | ResultError<Error>

export type Event<A...> = {
	connect: (Event<A...>, (A...) -> ()) -> (),
	disconnect: (Event<A...>, (A...) -> ()) -> (),
	fire: (Event<A...>, A...) -> ()
}

type EventPrivate<T = () -> ()> = Event<T> & {
	_connected: { [T]: boolean }
}

const Event = {}
Event.__index = Event

function Event.new<A...>(): Event<A...>
	return setmetatable({ _connected = {} }, Event) :: Event<A...>
end

function Event.connect<A...>(self: EventPrivate<(A...) -> ()>, callback: (A...) -> ())
	self._connected[callback] = true
end

function Event.disconnect<A...>(self: EventPrivate<(A...) -> ()>, callback: (A...) -> ())
	self._connected[callback] = nil
end

function Event.fire<A...>(self: EventPrivate<(A...) -> ()>, ...: A...)
	for f in self._connected do
		f(...)
	end
end

table.freeze(Loom)
return Loom