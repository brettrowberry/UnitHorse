#load "Types.fs"
#r "/Users/brett/.nuget/packages/fsharp.data/3.0.1/lib/net45/FSharp.Data.dll"
#load "Storage.fs"
#load "Library.fs"
open Library
open Functions

open Lets
// can't list all lengths
convert millimeter meter 1000.
convert ``US foot`` kilometer 5280.

lengths

open ``DU -> Record``
convert Millimeter Meter 1000. //names must be capitalized
convert USFoot Kilometer 5280.

lengths

tryGetLength "meter"
tryGetLength "metre"

open ``Record array``
tryConvert "mm" "m" 1000.
tryConvert "ft" "km" 5280.

lengths

open Csv
tryConvert "mm" "m" 1000.
tryConvert "ft" "km" 5280.

lengths