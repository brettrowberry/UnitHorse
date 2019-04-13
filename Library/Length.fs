module Length
open Types

type Length = Meter | Millimeter | Kilometer | USFoot

let getUnit = function
| Meter -> { Name = "meter"; Abbreviation = "m"; Factor = 1.0 }
| Millimeter -> { Name = "millimeter"; Abbreviation = "mm"; Factor = 1e-3 }
| Kilometer -> { Name = "kilometer"; Abbreviation = "km"; Factor = 1e3 }
| USFoot -> { Name = "US foot"; Abbreviation = "ft"; Factor = 0.3048 }

let convert source target values =
    let s = (getUnit source).Factor
    let t = (getUnit target).Factor
    let calc value = value * s / t
    values |> Array.map calc

convert Meter USFoot [| 1.; 2.; 3. |]

open Microsoft.FSharp.Reflection
let lengths =
    FSharpType.GetUnionCases typeof<Length>
    |> Array.map (fun c -> FSharpValue.MakeUnion(c, [||]) :?> Length |> getUnit)

let tryGetUnitFactor s = 
    lengths
    |> Array.tryFind (fun l -> l.Name = s)
    |> Option.map (fun l -> l.Factor)

type ConversionResult =
| UnitOrAbbrevNotFound of unitName : string
| UnitsOrAbbrevsNotFound of unitName : string * otherUnitName : string
| Success of float []
//consider closest match

let tryConvert source target (values : float[]) =
    match (tryGetUnitFactor source, tryGetUnitFactor target)  with
    | None, Some _ -> UnitOrAbbrevNotFound source
    | Some _, None -> UnitOrAbbrevNotFound target
    | None, None -> UnitsOrAbbrevsNotFound (source, target)
    | Some s, Some t -> 
        let calc value = value * s / t
        let results = values |> Array.map calc
        Success results

tryConvert "meter" "US foot" [| 1.; 2.; 3. |]
tryConvert "metre" "US foot" [| 1.; 2.; 3. |]
tryConvert "meter" "US feet" [| 1.; 2.; 3. |]