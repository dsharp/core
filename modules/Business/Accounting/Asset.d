Asset record {
  purchase	             : Purchase
  depreciation'Schedule  : Depreciation`Schedule	// An asset is written_down in steps according to it's schedule
}

Asset protocal {
  * purchase    : owned
  * writedown ↺ 
  * | sell    ∎ : sold
    | dispose ∎ : disposed

  purchase(
    seller : Entity, 
    amount : Money, 
    terms  : [ ] Terms
  ) -> Purchase        
  
  writedown (amount: Money) -> Writedown
  
  sell(
    buyer  : Entity,  
    amount : Money, 
    terms? : [ ] Terms
  ) -> Sale

  dispose () -> Disposal
  
  book'value	-> Money	
  writedowns  -> [ ] Writedown

  price       => purchase.price
}

Asset `Writedown event {
  asset  : Asset
  amount : Money
}

Depreciation `Schedule record {
  interval : Interval				
  callback : (Asset) -> Asset'Writedown		// percentage: 10%, fixed: $100
}