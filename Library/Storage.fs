namespace Storage
open Types

 module Lets =
    let meter = { Name = "meter"; Abbreviation = "m"; Value = 1.0 }
    let usFoot = { Name = "US foot"; Abbreviation = "ft"; Value = 0.3048 }

    // let convert source target value //DEAD END

 module ``Lets Take 2`` =
    type Length = 
        | Meter of Unit
        | USFoot of Unit

    let meter = Meter { Name = "meter"; Abbreviation = "m"; Value = 1.0 }
    let usFoot = USFoot { Name = "US foot"; Abbreviation = "ft"; Value = 0.3048 }

    let convert source target value : float =
        (source, target)
        ||> (fun source target -> value * source.Value / target.Value)

    // convert Meter USFoot 10. //DEAD END

module ``DU -> Record`` =
    open Microsoft.FSharp.Reflection
    type Length = Meter | USFoot

    let get = function
    | Meter -> { Name = "meter"; Abbreviation = "m"; Value = 1.0 }
    | USFoot -> { Name = "US foot"; Abbreviation = "ft"; Value = 0.3048 }

    let convert source target value : float =
        (get source, get target)
        ||> (fun source target -> value * source.Value / target.Value)

    let lengthNames = 
        FSharpType.GetUnionCases(typeof<Length>)
        |> Array.map (fun u -> u.Name)

    let tryGetLength (s:string) : Length option =
        FSharpType.GetUnionCases typeof<Length> 
        |> Array.tryFind (fun case -> case.Name = s)
        |> Option.bind (fun case -> FSharpValue.MakeUnion(case, [||]) :?> Length |> Some)

    tryGetLength "Meter"

module ``Record array`` =
    let lengths : Unit[] = [|
        { Name = "meter"; Abbreviation = "m"; Value = 1.0 }
        { Name = "US foot"; Abbreviation = "ft"; Value = 0.3048 }
    |]

    let lengthNames = lengths |> Array.map (fun u -> u.Name)

    let predicates : (Unit -> string -> bool) list = 
        [
            fun u x -> x = u.Name
            fun u x -> x = u.Abbreviation
        ]

    let any u s = predicates |> Seq.exists (fun f -> f u s) 

    let valueFrom nameOrAbbreviation =
        lengths
        |> Seq.tryFind (fun u -> any u nameOrAbbreviation)
        |> Option.map (fun u -> u.Value)

    let convert source target value : float option =
            (valueFrom source, valueFrom target)
            ||> Option.map2 (fun source target -> value * source / target)

module Csv =
    open FSharp.Data
    let [<Literal>] LengthFilePath = __SOURCE_DIRECTORY__ + "/Length.csv"
    let lengthProvider = CsvProvider<LengthFilePath, HasHeaders = true>.GetSample()
    let lengths =
        lengthProvider.Rows
        |> Seq.map (fun x -> 
            { Name = x.Name; Abbreviation = x.Abbreviation; Value = x.Value })
        |> Seq.toArray