module Length

type LengthMetadata = {
        Name : string
        Abbreviation : string
        Factor : float
    } 

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
// someone might want to know what units are available
let lengths =
    // FSharpType.GetUnionCases typeof<Length>
    // |> Array.map (fun c -> FSharpValue.MakeUnion(c, [||]) :?> Length |> getUnit)

    [| Meter; Millimeter; Kilometer; USFoot |] |> Array.map getUnit //clever alternative that doesn't need reflection

let tryGetUnitFactor nameOrAbbrev = 
    lengths
    |> Array.tryFind (fun l -> l.Name = nameOrAbbrev || l.Abbreviation = nameOrAbbrev)
    |> Option.map (fun l -> l.Factor)

type ConversionResult =
| NameNotFound of unitName : string
| NamesNotFound of unitName : string * otherUnitName : string
| Success of float []
// //consider closest match

let tryConvert source target (values : float[]) =
    match (tryGetUnitFactor source, tryGetUnitFactor target)  with
    | None, Some _ -> NameNotFound source
    | Some _, None -> NameNotFound target
    | None, None -> NamesNotFound (source, target)
    | Some s, Some t -> 
        let calc value = value * s / t
        let results = values |> Array.map calc
        Success results

let values = [| 1.; 2.; 3. |]
tryConvert "meter" "US foot" values
tryConvert "metre" "US foot" values
tryConvert "meter" "US feet" values

tryConvert "m" "US foot" values