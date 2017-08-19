Authorization protocol {
  revoke()
  cancel()

  account   : Account 
  signature : Signature  // A signature from an account holder
}

Authorization record {
  account   : Account
  entity    : Entity
  amount    : Decimal
  signature : Signature
  expires   : DateTime?
}

// Authorizes and entity to debit a specific amount from an account