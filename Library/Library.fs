module Library

open Types

type Length = Meter | Millimeter | Kilometer | USFoot

let convert (getFactor : Length -> float) source target value : float =
    (getFactor source, getFactor target)
    ||> (fun source target -> value * source / target)

let tryConvertTemplate (tryGetFactor : string -> float option) (source : string) (target : string) value : float option =
    (tryGetFactor source, tryGetFactor target)
    ||> Option.map2 (fun source target -> value * source / target)    

//ok from F#
module Lets =
    type Dummy = | Dummy
    let meter = { Name = "meter"; Abbreviation = "m"; Factor = 1.0 }
    let millimeter = { Name = "millimeter"; Abbreviation = "mm"; Factor = 1e-3 }
    let kilometer = { Name = "kilometer"; Abbreviation = "km"; Factor = 1e3 }
    let ``US foot`` = { Name = "US foot"; Abbreviation = "ft"; Factor = 0.3048 }

let lengths = typeof<Lets.Dummy>.DeclaringType.GetProperties() |> Array.map(fun p -> p.GetValue(null, null) :?> Unit)
let tryConvert = //

// maybe we can define what a successful implementation looks like
type ILengthConverterDraft = {
    Lengths : Unit[]
    Convert: Unit -> Unit -> float -> float
    TryConvert: string -> string -> float -> float option
}

let letsLengthConverter : ILengthConverterDraft = {
    Lengths = lengths
    Convert = convert
    TryConvert = tryConvertTemplate ?
}

//best overall
module ``DU -> Record`` =
    open Microsoft.FSharp.Reflection

    let get = function
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

    let tryGetFactor s = lengths |> Array.tryFind (fun l -> l.Name = s) |> Option.map (fun l -> l.Factor)

type ILengthConverter = {
    Lengths : Unit[]
    Convert: Length -> Length -> float -> float
    TryConvert: string -> string -> float -> float option
}

let duLengthConverter : ILengthConverter = {
    Lengths = ``DU -> Record``.lengths
    Convert = ``DU -> Record``.convert
    TryConvert = tryConvertTemplate ``DU -> Record``.tryGetFactor
}

// best as a web service
module ``Record array`` =
    let lengths : Unit[] = [|
        { Name = "meter"; Abbreviation = "m"; Factor = 1.0 }
        { Name = "millimeter"; Abbreviation = "mm"; Factor = 1e-3 }
        { Name = "kilometer"; Abbreviation = "km"; Factor = 1e3 }
        { Name = "US foot"; Abbreviation = "ft"; Factor = 0.3048 }
    |]

    let getFactor = function        
    | Meter -> lengths.[0].Factor
    | Millimeter -> lengths.[1].Factor
    | Kilometer -> lengths.[2].Factor
    | USFoot -> lengths.[3].Factor

    let predicates : (Unit -> string -> bool) list = 
        [
            fun u x -> x = u.Name
            fun u x -> x = u.Abbreviation
        ]

    let any u s = predicates |> Seq.exists (fun f -> f u s) 

    let tryGetFactor nameOrAbbreviation =
        lengths
        |> Seq.tryFind (fun u -> any u nameOrAbbreviation)
        |> Option.map (fun u -> u.Factor)

    let tryConvert source target value = 
        tryConvertTemplate tryGetFactor source target value

let recordArrayLengthConverter : ILengthConverter = {
    Lengths = ``Record array``.lengths
    Convert = convert ``Record array``.getFactor
    TryConvert = ``Record array``.tryConvert
}

// best as a web service
module Csv =
    let lengths = Storage.Length.lengths

    let predicates : (Unit -> string -> bool) list = 
        [
            fun u x -> x = u.Name
            fun u x -> x = u.Abbreviation
        ]

    let any u s = predicates |> Seq.exists (fun f -> f u s)        

    let tryGetFactor nameOrAbbreviation =
        lengths
        |> Seq.tryFind (fun u -> any u nameOrAbbreviation)
        |> Option.map (fun u -> u.Factor)

    let getFactor length =
        let finder name = lengths |> Array.find (fun l -> l.Name = name) |> fun x -> x.Factor
        match length with
        | Meter -> finder "meter"
        | Millimeter -> finder "millimeter"
        | Kilometer -> finder "kilometer"
        | USFoot -> finder "US foot"  

    let tryConvert source target value = 
        tryConvertTemplate tryGetFactor source target value

let csvLengthConverter : ILengthConverter = {
    Lengths = Csv.lengths
    Convert = convert Csv.getFactor
    TryConvert = Csv.tryConvert
}    