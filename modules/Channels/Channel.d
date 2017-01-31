﻿// Channels replace sequences, streams,
// * byte.. :≡ Readable`Channel of zero or more_byte'


Channel protocal { 
  status   -> Channel`Status
  unread   -> Int64 ≥ 0

  read () -> 
    | * 
	  | Awaiter
	  | ∎

  write async -> 
    | Commited 
	  | Awaiter 
	  | NotConnectedError
}


Seekable protocal { 
  position : Int64 ≥ 0
  length   : Int64 > 0

  seek (Int64 ≥ 0) -> 
   | Seeked
   | OutsideRange	
}

Seekable_Channel = Channel & Seekable

Channel `Status type = Closed | Connected | Terminated | ∎;

ReadableChannel protocal {
  available : Int64 ≥ 0
  read ƒ    -> Message | Backpressure
}

// * T = Alias for Channel of T


WriteableChannel protocal {
  async write ƒ(message: Message) -> OK | Awaiter | Closed
}

Channel`Awaiter protocal for T { 
  reason : NoMessages | Throttled
  result : T
  ready  : event
}


// Create a multicast wrapper around a channel

Observerable<T> protocal {
  subscriptions : [ ] Subscription<T>
  subscribe(ƒ(Message|∎)) -> Subscription<T>
}  

Connection protocal : Channel { 
  * connect   : connecting   // send packet 
  * ack       : connected    // acknowledged. GTG 
  * send      : sending
  * receive   : receiving
  * close   ∎ : closed

  open   () -> * Connected | Error
  close  () -> * Disconnected | Error
}



