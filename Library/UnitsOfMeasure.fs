// Live coding : Dynamic typing :: Live uncommenting : Static typing

module UnitsOfMeasure
// https://fsharpforfunandprofit.com/posts/correctness-type-checking/

type [<Measure>] m
type [<Measure>] mm =
   static member ToMeters(millimeters : float<mm>) : float<m> = 
      millimeters * 1e-3<m/mm>

let millimeters = 1e3<mm>
let millimetersInMeters = mm.ToMeters(millimeters)

millimeters + millimetersInMeters // can't mix and match!

// A B, n = 2
// A->B
// B->A
// n(n-1) -> 2

// A B C, n = 3
// A->B, A->C
// B->A, B->C
// C->A, C->B
// n(n-1) -> 6

// A B C D, n = 4
// A->B, A->C, A->D
// B->A, B->C, B->D
// C->A, C->B, C->D
// D->A, D->B, D->C
// n(n-1) -> 12