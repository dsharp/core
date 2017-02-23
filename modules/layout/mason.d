Masonary'Layout type {
  gap     : Number
  columns : Box [ ]
}

Masonary'Layout impl {
  from (columnCount: Int32, columnWidth: Int32, gap = 10) {
    let mutable left = 0
    
    let mutable columns = [ ] Column;

    for i in 0..<columnCount {
      columns.add {
        width  : columnWidth,
        height : 0,
        top    : 0,
        left   : left
      }

      left += columnWidth + gap
    }

    return Masonary'Layout { gap, columns }
  }

  shortestColumn ƒ() => 
    from columns 
    orderby $0.height descending |> first
}

Layout impl for Masonary'Layout {
  doLayout (elements) { 
    for elements {
      let column = shortestColumn()
                
      // Add bottom gutter
      if column.height > 0 {
        column.height += columnGap
      }

      $0.left = column.left
      $0.top  = column.height

      // Add the item height to the column
      column.height += $0.height
    }
    
    return Size {
      width  : (columnGap * (columns.count - 1)) + (columnWidth * columnCount),
      height : columns |> map(c => c.height) |> max
    }
  }
}

Size type {
  width  : Number
  height : Number
}

Box type { 
  width          : Number  
  mutable height : Number  
  top            : Number 
  left           : Number
}