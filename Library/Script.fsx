#load "Types.fs"
#r "/Users/brett/.nuget/packages/fsharp.data/3.0.1/lib/net45/FSharp.Data.dll"
#load "Storage.fs"
#load "Library.fs"
open Library
open Functions

open Lets
// can't list all lengths
convert millimeter meter 1000.
convert meter kilometer 1000.
convert Lets.``US foot`` kilometer 5280. 

open ``DU -> Record``
lengths
convert Millimeter Meter 1000. //names must be capitalized
convert Meter Kilometer 1000.
convert USFoot Kilometer 5280.
tryGetLength "meter"
tryGetLength "metre"

open ``Record array``
lengths
tryConvert "mm" "m" 1000.
tryConvert "m" "km" 1000.
tryConvert "ft" "km" 5280.

open Csv
lengths
tryConvert "mm" "m" 1000.
tryConvert "m" "km" 1000.
tryConvert "ft" "km" 5280.