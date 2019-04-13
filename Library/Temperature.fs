module Temperature

type TemperatureMetadata = {
    Name : string
    Abbreviation : string
    Zero: float
    Scale: float
}

type Temp = F | K

let getUnit = function
| F -> { Name = "Fahrenheit"; Abbreviation = "F"; Zero = 459.67; Scale = 5./9. }
| K -> { Name = "Kelvin"; Abbreviation = "K"; Zero = 0.; Scale = 1. } 

let convert source target value =
    let s = getUnit source
    let t = getUnit target
    (value + s.Zero) * (s.Scale / t.Scale) - t.Zero

convert F K -459.67 //0.
convert F K -10. //249.82
convert F K 0. //255.37
convert F K 10. //260.93

convert K F 0. //-459.67
convert K F 249.82 //-10.
convert K F 255.37 //0.
convert K F 260.93 //10.