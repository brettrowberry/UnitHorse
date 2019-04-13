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

///
/// //best overall
module DU_Record =
    open Types
    open Microsoft.FSharp.Reflection

    let private get = function
    | Meter -> { Name = "meter"; Abbreviation = "m"; Factor = 1.0 }
    | Millimeter -> { Name = "millimeter"; Abbreviation = "mm"; Factor = 1e-3 }
    | Kilometer -> { Name = "kilometer"; Abbreviation = "km"; Factor = 1e3 }
    | USFoot -> { Name = "US foot"; Abbreviation = "ft"; Factor = 0.3048 }

    let lengths =
        FSharpType.GetUnionCases typeof<Length>
        |> Array.map (fun c -> FSharpValue.MakeUnion(c, [||]) :?> Length |> get)
    
    let convert source target value : float =
        (get source, get target)
        ||> (fun source target -> value * source.Factor / target.Factor)

    let private tryGetFactor s = lengths |> Array.tryFind (fun l -> l.Name = s) |> Option.map (fun l -> l.Factor)

    let tryConvert = tryConvertTemplate tryGetFactor

    let tryConvert source target value : float option =
    (tryGetFactor source, tryGetFactor target)
    ||> Option.map2 (fun source target -> value * source / target)