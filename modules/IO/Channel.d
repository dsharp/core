﻿// Channels replace sequences, streams,
// * byte.. :≡ Readable`Channel of zero or more bytes

Channel protocol { 
  status -> Channel`Status
  unread -> i64 >= 0

  read () -> 
    | * 
	  | Awaiter
	  | ∎

  write async -> 
    | Commited 
	  | Awaiter 
	  | NotConnectedError
}


Seekable_Channel = Channel & Seekable

Channel `Status enum = Closed | Connected | Terminated | ∎

ReadableChannel protocol {
  available : i64 >= 0
  read ƒ    -> Message | Backpressure
}

// * T = Alias for Channel of T


WriteableChannel protocol {
  async write ƒ(message: Message) -> OK | Awaiter | Closed
}

Channel`Awaiter<T> protocol { 
  reason : NoMessages | Throttled
  result : T
  ready  : event
}


// Create a multicast wrapper around a channel

Observerable<T> protocol {
  subscriptions           : [ Subscription<T> ]

  subscribe(ƒ(Message|∎)) -> Subscription<T>
}



