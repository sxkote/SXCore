# SXCore
SXCore is a library with a common useful shared tools...

##SXCore.Lexems
Library that helps you to get the Expression from the string and calculate it on the fly.


For example, you can create an Expression from the string:
```
var expression = new SXExpression("2 + 2");
```

and calculate it:
```
var result = expression.Calculate().Value; // result == 4 is true 
```

or you can calculate the Expression on the fly:
```
var result = SXExpression.Calculate("2+2").Value; //result == 4 
```

You can use functions:
```
var result = SXExpression.Calculate("2 + sin(pi()/2)").Value; //result == 3
```



**Supported Types:**
* Numbers
* Dates
* TimeSpan
* Booleans 
* Text
* Structs
* Complex Numbers

You can use Dates like this:
```
var result = SXExpression.Calculate(" '24.06.2015'.AddDays(6).Day ").Value; //result == 30
```
