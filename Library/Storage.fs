namespace Storage

module Length =
    open Types
    open FSharp.Data
    let [<Literal>] private LengthFilePath = __SOURCE_DIRECTORY__ + "/Length.csv"
    
    let lengths =
        let lengthProvider = CsvProvider<LengthFilePath, HasHeaders = true>.GetSample()
        lengthProvider.Rows
        |> Seq.map (fun x -> 
            { Name = x.Name; Abbreviation = x.Abbreviation; Factor = x.Value })
        |> Seq.toArray  