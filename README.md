# UnitHorse 📏🐴
Unit conversion library for .NET

# Why Am I Doing This?
I have had a great time learning F#. I wanted the experience of making a simple, high-quality F# library to get some practice.

# Background
Let's talk about units for a minute. What is a unit of measurement? Well, it's a way to express a quantity. The SI System defines units for these quantities:
- electric current
- temperature
- time
- length
- mass
- luminous intensity
- amount of substance

Converting between two units can be done with a function where the values of the parameters are determined by the source and target units.
For a quantity like temperature, 0ºC is not the same as 0ºF. Perhaps you have seen the function to convert from Celsius to Fahrenheit:

``` fsharp
let convertToF degreesC = 9./5. * degreesC + 32.
```

Converting between two different units of measure for the temperature quantity must take an offset and a slope into account. So, we have a function to convert between 2 different specific units in a single direction. To perform the opposite conversion, we need another function. 
C --> F
F --> C
For 2 units of measure, we need 2 functions.
Saw we want to add Kelvin (K) to the mix. Now we have:
C --> F, C --> K
F --> C, F --> K
K --> C, K --> F
For 3 units of measure, we need 6 functions.
How about Rankine (R)?
C --> F, C --> K, C --> R
F --> C, F --> K, F --> R
K --> C, K --> F, K --> R
R --> C, R --> F, R --> K

For 4 units of measure, we need 24 functions.
I'm not writing those. That's n factorial functions!
For length, 0 is 0 is 0. Said another way, 0 US Survey Feet is the same as 0 meters. Perhaps you're thinking of the y = ax + b from school. In the case of length, there is no 'b' or offset term, just different slopes, 'a'. So, what would such a function look like? 
let convert value source target = value * source / target
Let's talk about length. Some common units for length are the meter and the US Survey Foot, defined as 1200/2927 meters https://en.wikipedia.org/wiki/Foot_(unit)#US_survey_foot. Is 0 meters the same as 0 feet? Yes, it is. Length is nice in that two different length units differ only in their slope, not their offset.
