Vector2 struct {
  x, y: Number

  from (x, y) => Vector2(x, y)
}

Vector2 impl for Vector {
  count => 2

  [index: i64] => match index { 
    | 0 => x
    | 1 => y
  }
}