namespace Library

open Types

module Functions =
    let convert source target value : float =
        (source, target)
        ||> (fun source target -> value * source.Value / target.Value)
    
    let tryConvertTemplate valueFrom source target value : float option =
        (valueFrom source, valueFrom target)
        ||> Option.map2 (fun source target -> value * source / target)    

module Lets =
    let meter = { Name = "meter"; Abbreviation = "m"; Value = 1.0 }
    let millimeter = { Name = "millimeter"; Abbreviation = "mm"; Value = 1e-3 }
    let kilometer = { Name = "kilometer"; Abbreviation = "km"; Value = 1e3 }
    let ``US foot`` = { Name = "US foot"; Abbreviation = "ft"; Value = 0.3048 }

module ``DU -> Record`` =
    open Microsoft.FSharp.Reflection
    type Length = Meter | Millimeter | Kilometer | USFoot

    let get = function
    | Meter -> { Name = "meter"; Abbreviation = "m"; Value = 1.0 }
    | Millimeter -> { Name = "millimeter"; Abbreviation = "mm"; Value = 1e-3 }
    | Kilometer -> { Name = "kilometer"; Abbreviation = "km"; Value = 1e3 }
    | USFoot -> { Name = "US foot"; Abbreviation = "ft"; Value = 0.3048 }

    let lengths =
            FSharpType.GetUnionCases typeof<Length>
            |> Array.map (fun c -> FSharpValue.MakeUnion(c, [||]) :?> Length |> get)
    
    let convert source target value : float =
        (get source, get target)
        ||> (fun source target -> value * source.Value / target.Value)

    let tryGetLength s = lengths |> Array.tryFind (fun l -> l.Name = s)
    
module ``Record array`` =
    let lengths : Unit[] = [|
        { Name = "meter"; Abbreviation = "m"; Value = 1.0 }
        { Name = "millimeter"; Abbreviation = "mm"; Value = 1e-3 }
        { Name = "kilometer"; Abbreviation = "km"; Value = 1e3 }
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

    let tryConvert source target value = 
        Functions.tryConvertTemplate valueFrom source target value

module Csv =
    let lengths = Storage.Length.lengths

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

    let tryConvert source target value = 
        Functions.tryConvertTemplate valueFrom source target value