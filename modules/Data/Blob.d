Blob protocol {
  open   ()                        -> * byte ↺     | Error    ∎ | End ∎
  slice  (offset: i64, count: i64) -> * byte ↺     | Error    ∎ | End ∎
  link   (record: Record)          -> * Linked   ∎ | Failed   ∎
  unlink (record: Record)          -> * Unlinked ∎ | Failed   ∎

                                   // sql -----------------------------------------------------
  blocks      -> [ Blob `Block ]   // select block     from Blob'blocks        where blob = $0
  hashes      -> [ Hash ]
  attributes  -> [ Attribute ]
}

Blob record { 
  size: i64 > 0
}