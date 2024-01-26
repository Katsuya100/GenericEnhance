# GenericEnhance
[日本語](README_ja.md)

## Summary
This library "GenericEnhance" provides strong support for Generic programming.  

- Generic Specialization
- TypeDef
- Generic Non-type argument
- None Type
- Variadic type arguments
- Default type argument

These incorporate some of the features that were available in C++ templates for use in C# Generic programming.

## System Requirements
|  Environment  |  Version  |
| ---- | ---- |
| Unity | 2021.3.15f1, 2022.3.2f1 |
| .Net | 4.x, Standard 2.1 |

## How to install
### Install dependenies
Install the following packages.  

- [ILPostProcessorCommon v2.1.0](https://github.com/Katsuya100/ILPostProcessorCommon/tree/v2.1.0)
- [MemoizationForUnity v1.4.0](https://github.com/Katsuya100/MemoizationForUnity/tree/v1.4.0)

### Installing GenericEnhance
1. Open [Window > Package Manager].
2. click [+ > Add package from git url...].
3. Type `https://github.com/Katsuya100/GenericEnhance.git?path=packages` and click [Add].

#### If it doesn't work
The above method may not work well in environments where git is not installed.  
Download the appropriate version of `com.katuusagi.genericenhance.tgz` from [Releases](https://github.com/Katsuya100/MemoizationForUnity/releases), and then [Package Manager > + > Add package from tarball...] Use [Package Manager > + > Add package from tarball...] to install the package.

#### If it still doesn't work
Download the appropriate version of `Katuusagi.GenericEnhance.unitypackage` from [Releases](https://github.com/Katsuya100/MemoizationForUnity/releases) and Import it into your project from [Assets > Import Package > Custom Package].

## Generic Specialization
*This feature is limited to Generic methods only.  

Generic specialization is a mechanism that calls different implementations depending on the type arguments given.  
This allows you to

- Calling optimized processing for each type
- Branching implementations by type

This makes it possible to do things such as

### How to implementation
Specialization is done using the `SpecializationMethod` and `SpecializedMethod` attributes.  
The `SpecializationMethod` attribute specifies the function to be called by default.  
The `SpecializedMethod` attribute specifies the function to be called at the time of specialization and the type argument of the specialization condition.  
In the example, GetValue method is specialized.  

```.cs
using Katuusagi.GenericEnhance;

[SpecializationMethod(nameof(GetInternal))]  // Basically, GetInternal is called.
[SpecializedMethod(nameof(GetInteger), typeof(int))]  // GetInteger is called if the type argument contains an int.
public partial T GetValue<T>();

public T GetInternal<T>()
{
    return default;
}

public int GetInteger()
{
    return 100;
}

:

// GetValueInternal is called.
var f = GetValue<float>();
Debug.Log(f); // 0

// GetInteger is called.
var i = GetValue<int>();
Debug.Log(i); // 100;
```

### Algorithm specification
There are several algorithms for specialization.  
The algorithm can be specified by the second argument of the `SpecializationMethod` attribute.  
Since each algorithm has different characteristics, you should use the algorithm according to the actual implementation environment.  

|  Algorithm  |  feature  |
| ---- | ---- |
| VirtualStrategy | It is implemented in a strategy pattern using virtual method.<br>Creates a cache on the first call.<br>The overhead of the virtual function call is incurred.  |
| DelegateStrategy | It is implemented in a strategy pattern using Delegate.<br>Creates a cache on the first call.<br>After that, a small cache is created for each type combination.<br>Calls are fast. |
| TypeComparison | Linear comparison of types.<br>No cache is created.<br>It is very fast in the JIT environment because it is optimized. |
| TypeIdComparison | Comparisons are made using IDs assigned to type combinations.<br>A cache is created for type comparisons.<br>After that, a small cache is created for each type combination.<br>It can be called at high speed even in an AOT environment. |

See below for results of performance measurements.

#### Measurements on the Editor
[Test Code](packages/Tests/Runtime/SpecializationPerformanceTest.cs)  

|  Process  |  Performance  |
| ---- | ---- |
| Direct | 0.014535 ms |
| VirtualStrategy | 0.07683 ms |
| DelegateStrategy | 0.067435 ms |
| TypeComparison | 0.036765 ms |
| TypeIdComparison | 0.04558 ms |

The `TypeComparison` gives high performance because the editor allows JIT compilation.  

#### Post-build measurements
The measurement code is as follows.
```.cs
private readonly ref struct Measure
{
    private readonly string _label;
    private readonly StringBuilder _builder;
    private readonly float _time;

    public Measure(string label, StringBuilder builder)
    {
        _label = label;
        _builder = builder;
        _time = (Time.realtimeSinceStartup * 1000);
    }

    public void Dispose()
    {
        _builder.AppendLine($"{_label}: {(Time.realtimeSinceStartup * 1000) - _time} ms");
    }
}

:

var log = new StringBuilder();
int testCount = 10000;
using (new Measure("Direct", log))
{
    for (int i = 0; i < testCount; ++i)
    {
        Add(10.0, 20.0);
    }
}

using (new Measure("Static Strategy", log))
{
    for (int i = 0; i < testCount; ++i)
    {
        WrappedAdd_VirtualStrategy<double, double, double>(10, 20);
    }
}

:
```

|  Process  |  Mono(Debug)  |  Mono(Release)  |  IL2CPP(Debug)  |  IL2CPP(Release)  |
| ---- | ---- | ---- | ---- | ---- |
| Direct | 0.01612091 ms | 0.002952576 ms | 0.0004882813 ms | 0 ms(unmeasurable) |
| VirtualStrategy | 0.161232 ms | 0.06460953 ms | 2.692383 ms | 2.297852 ms |
| DelegateStrategy | 0.1258011 ms | 0.06257629 ms | 0.05908203 ms | 0.04199219 ms |
| TypeComparison | 0.08607483 ms | 0.02434921 ms | 5.053223 ms | 3.029297 ms |
| TypeIdComparison | 0.0706749 ms | 0.03424454 ms | 0.07226563 ms | 0.07714844 ms |

Both are inferior to direct calls, but if judged solely on processing speed, the following are recommended.

- *Mono(Debug)* : TypeIdComparison
- *Mono(Release)* : TypeComparison
- *IL2CPP* : DelegateStrategy

If you do not consider differences due to environment, `TypeIdComparison` will give you stable performance.  

However, since the timing and frequency of cache creation varies depending on the algorithm, please select one in consideration of the intended use.  
`VirtualStrategy` has a smaller cache size and less frequent cache creation, so it has a lower negative impact on performance.  
`TypeComparison` does not create a cache and therefore offers the highest performance in terms of memory.  

### Replacement Optimization
Only if the following conditions are met will the call be replaced by a direct call, regardless of the algorithm.  
The same performance as a direct call can be achieved, so active use of this feature is recommended.  

1. The specified function is public.  
```.cs
[SpecializationMethod(nameof(GetInternal))]
[SpecializedMethod(nameof(GetInteger), typeof(int))]
public partial T GetValue<T>();

// OK
public T GetInternal<T>()
{
    return default;
}

// NG
private T GetInternal<T>()
{
    return default;
}

:
```
2. Do not give other type arguments to type arguments.  
```.cs
// OK
public int WrappedGetValue<int>()
{
    return GetValue<int>();
    // ↓
    // return GetInteger();
}

// NG
public T WrappedGetValue<T>()
{
    return GetValue<T>();
}
```

## TypeDef
The `TypeDef` attribute can be used to redefine an already existing type with an alias.  
Besides its use as an alias, it can also be used to return a type generated from within Generic.  

### How to Implementation
Redefine the type by implementing the following.

```.cs
using Katuusagi.GenericEnhance;

[TypeDef(typeof(int))]
public struct DefInt
{
}
```

### Redefining Members
A type with the `TypeDef` attribute can have a member of the same name to call the member that the original type has.  
```.cs
using Katuusagi.GenericEnhance;

[TypeDef(typeof(Vector3))]
public struct Vector
{
    public float x;
    public float magnitude { get; }
}

:

Vector v = default;
v.x = 10.0f
Debug.Log(v.magnitude); // 10
```

### Redefinition of Type Arguments
If you want to redefine a Type Argument, specify the type name as a string.  
You can also use the `nameof` clause.  

```.cs
using Katuusagi.GenericEnhance;

public struct Any<T>
    where T : struct
{
    [TypeDef(nameof(T))]
    public struct Element
    {
    }
}

:

Debug.Log(typeof(Any<int>.Element)); // Int32
```

### Reinterpretation of type
Types with the `TypeDef` attribute are replaced at the IL level.  
However, from the C# side, they are interpreted as different types.  
If you want to reinterpret the type from C#, use the `CastUtils.SafeAs` function.
```.cs
[TypeDef(typeof(int))]
public struct DefInt
{
}

:

int intValue = 100;
DefInt defaultValue = default;
ref DefInt defIntValue = ref CastUtils.SafeAs(ref intValue, ref defaultValue);

Debug.Log(defIntValue); // 100
```

Use the `CastUtils.TryAs` function to detect reinterpretation failures.
```.cs
int intValue = 100;
if (!CastUtils.TryAs(ref intValue, out DefInt defIntValue))
{
    // failed.
}

Debug.Log(defIntValue); // 100
```

### Note
Value types may specify the `TypeDef` attribute only between value types, and reference types may specify it only between reference types.  
```.cs
using Katuusagi.GenericEnhance;

// OK
[TypeDef(typeof(int))]
public struct DefInt
{
}

// NG
[TypeDef(typeof(int))]
public class DefInt
{
}
```


## Generic Non-type argument
After the introduction of this library, it is possible to give untyped arguments as Generic arguments.  
Strictly speaking, nontype information is wrapped in type information and passed as a generic argument.  
This allows non-type information to be passed from type arguments and guarantees constant nature.  
When used in conjunction with Generic specialization, it is also possible to branch using type information.  

### How to Implementation
非型パラメーターの記法は以下です。

|  種類  |  例  |  記法例  |
| ---- | ---- | ---- |
| 正の整数 | 100 | _100 |
| 負の整数 | -100 | _n100 |
| 正の少数 | 10.1 | _10_1 |
| 負の少数 | -10.1 | _n10_1 |
| 真 | true | _true |
| 偽 | false | _false |

実行時の値は`TypeFormula.GetValue`関数で取得します。

```.cs
var value = TypeFormula.GetValue<_100, int>();
Debug.Log(value); // 100
```

### Operation
To perform operations, use the Operator type.  
The following example uses the `Add` type, which is an Operator type for addition.

```.cs
var value = TypeFormula.GetValue<Add<int, _100, _10>, int>();
Debug.Log(value); // 110
```

#### Type of Operator
##### Arithmetic Operations
|  Operator  |  Implementation  |
| ---- | ---- |
| Add<T, TX, TY> | (T)TX + (T)TY |
| Sub<T, TX, TY> | (T)TX - (T)TY |
| Mul<T, TX, TY> | (T)TX * (T)TY |
| Div<T, TX, TY> | (T)TX / (T)TY |
| Mod<T, TX, TY> | (T)TX % (T)TY |
| Minus<T, TX> | -((T)TX) |

##### Conditional Logical Operations
|  Operator  |  Implementation  |
| ---- | ---- |
| Equal<T, TX, TY> | (T)TX == (T)TY |
| NotEqual<T, TX, TY> | (T)TX != (T)TY |
| Greater<T, TX, TY> | (T)TX > (T)TY |
| GreaterOeEqual<T, TX, TY> | (T)TX >= (T)TY |
| Less<T, TX, TY> | (T)TX < (T)TY |
| LessOeEqual<T, TX, TY> | (T)TX <= (T)TY |
| And<TX, TY> | (bool)TX && (bool)TY |
| Or<TX, TY> | (bool)TX \|\| (bool)TY |
| Not\<TX> | !(bool)TX |

##### Bit Logical Operations
|  Operator  |  Implementation  |
| ---- | ---- |
| BitAnd<T, TX, TY> | (T)TX & (T)TY |
| BitOr<T, TX, TY> | (T)TX \| (T)TY |
| BitXor<T, TX, TY> | (T)TX ^ (T)TY |
| BitNot<T, TX> | ~(T)TX |
| LShift<T, TX, TY> | (T)TX << (int)TY |
| RShift<T, TX, TY> | (T)TX >> (int)TY |

##### Cast Operations
|  Operator  |  Implementation  |
| ---- | ---- |
| CastNumeric<T, TX> | (Numeric Type)(T)TX |

### Use TypeDef to treat types like constants
Operator types are subject to replacement by `TypeDef`.  
So the types defined by `TypeDef` can be treated like constants.

```.cs
[TypeDef(typeof(Add<int, _100, _200>))]
public struct IntValue : ITypeFormula<int>
{
    int ITypeFormula<int>.Result => default;
}

:

var value = TypeFormula.GetValue<Add<int, IntValue, _10>, int>();
Debug.Log(value); // 310
```

### Replacement Optimization
If a different type argument is not given, the result of the operation is replaced.  

```.cs
// OK
Debug.Log(typeof(Add<int, _100, _10>)); // _110
// ↓
// Debug.Log(typeof(_110));

// NG
Debug.Log(typeof(Add<int, _100, T>)); // Add<int, _100, T>
```

## None Type
A `NoneType` argument of type `NoneType` disables that argument internally.  
This is mainly used as padding for Generic arguments.  

### How to Implementation
If `NoneType` type is specified as a type argument, the type argument is omitted.  
Also, if you specify `NoneType.Default` as a type argument, that argument will be omitted.  

```.cs
Debug.Log(typeof(Action<int, NoneType>)); // Action`1
Debug.Log(typeof(Action<NoneType>)); // Action

Action<int, NoneType> action = (x, y) => {};
action(0, NoneType.Default);
// ↓
// action(0);
```

## Variadic type arguments
*This function is limited to the Generic method.  

Duplicates the type argument up to the specified count.  
Note that this is not a variadic type arguments in the strict sense.  

### How to Implementation
`VariadicGeneric` attribute is used to implement variadic type arguments.  
The `VariadicGeneric` attribute specifies the minimum count of elements in the type argument and the maximum count of elements in the type argument.  

```.cs
[VariadicGeneric(0, 16)]
public static Type GetActionType<T>()
{
    return typeof(Action<T>);
}
```
The implementation is then replicated as follows.  
```.cs
[global::Katuusagi.GenericEnhance.VariadicGenerated(0)]
[VariadicGeneric(0, 16)]
public static Type GetActionType()
{
    return typeof(Action<global::Katuusagi.GenericEnhance.NoneType>);
}

:

[global::Katuusagi.GenericEnhance.VariadicGenerated(16)]
[VariadicGeneric(0, 16)]
public static Type GetActionType<T__0, T__1, T__2, T__3, T__4, T__5, T__6, T__7, T__8, T__9, T__10, T__11, T__12, T__13, T__14, T__15>()
{
    return typeof(Action<T__0, T__1, T__2, T__3, T__4, T__5, T__6, T__7, T__8, T__9, T__10, T__11, T__12, T__13, T__14, T__15>);
}
```
Only the trailing type argument is duplicated.

### Variadic arguments
This implementation can duplicate the number of elements in the argument by the same number of elements as the Variadic type arguments.  
If you want to duplicate an argument, the argument must end with the same type as the trailing type argument.  
An example is shown below.
```.cs
[VariadicGeneric(0, 16)]
public static void InvokeAction<TVar>(Action<TVar> action, TVar args)
{
    action?.Invoke(args);
}
```
In this case, the implementation is replicated as follows.
```.cs
[global::Katuusagi.GenericEnhance.VariadicGenerated(0)]
[VariadicGeneric(0, 16)]
public static void InvokeAction(Action<global::Katuusagi.GenericEnhance.NoneType> action)
{
    action?.Invoke(global::Katuusagi.GenericEnhance.NoneType.Default);
}

:

[global::Katuusagi.GenericEnhance.VariadicGenerated(16)]
[VariadicGeneric(0, 16)]
public static void InvokeAction<TVar__0, TVar__1, TVar__2, TVar__3, TVar__4, TVar__5, TVar__6, TVar__7, TVar__8, TVar__9, TVar__10, TVar__11, TVar__12, TVar__13, TVar__14, TVar__15>(Action<TVar__0, TVar__1, TVar__2, TVar__3, TVar__4, TVar__5, TVar__6, TVar__7, TVar__8, TVar__9, TVar__10, TVar__11, TVar__12, TVar__13, TVar__14, TVar__15> action, TVar__0 args__0, TVar__1 args__1, TVar__2 args__2, TVar__3 args__3, TVar__4 args__4, TVar__5 args__5, TVar__6 args__6, TVar__7 args__7, TVar__8 args__8, TVar__9 args__9, TVar__10 args__10, TVar__11 args__11, TVar__12 args__12, TVar__13 args__13, TVar__14 args__14, TVar__15 args__15)
{
    action?.Invoke(args__0, args__1, args__2, args__3, args__4, args__5, args__6, args__7, args__8, args__9, args__10, args__11, args__12, args__13, args__14, args__15);
}
```

### Extracting type argument element in an iterative process
By writing `VariadicForEach` scope, you can iterate over variable length type arguments like a foreach statement.  
Within the `VariadicForEach` scope, type arguments and arguments are not expanded when invoked.  
All elements are called repeatedly.  
The following example uses the `VariadicForEach` scope to concatenate variable-length arguments.  
```.cs
[VariadicGeneric(1, 16)]
public static string ForEachJoin<T>(string separator, T args)
{
    var str = string.Empty;
    using (new VariadicForEach())
    {
        str = $"{str}{(T)args}{separator}";
    }

    if (string.IsNullOrEmpty(str))
    {
        return string.Empty;
    }

    return str.Remove(str.Length - separator.Length, separator.Length);
}
```
In this case, the iterative process is generated as follows.
```.cs

[global::Katuusagi.GenericEnhance.VariadicGenerated(16)]
[VariadicGeneric(1, 16)]
public static string ForEachJoin<T__0, T__1, T__2, T__3, T__4, T__5, T__6, T__7, T__8, T__9, T__10, T__11, T__12, T__13, T__14, T__15>(string separator, T__0 args__0, T__1 args__1, T__2 args__2, T__3 args__3, T__4 args__4, T__5 args__5, T__6 args__6, T__7 args__7, T__8 args__8, T__9 args__9, T__10 args__10, T__11 args__11, T__12 args__12, T__13 args__13, T__14 args__14, T__15 args__15)
{
    var str = string.Empty;
    using (new VariadicForEach())
    {
        {
            str = $"{str}{(T__0)args__0}{separator}";
        }
        global::Katuusagi.GenericEnhance.VariadicUtils.ContinueTarget();

        :

        {
            str = $"{str}{(T__15)args__15}{separator}";
        }
        global::Katuusagi.GenericEnhance.VariadicUtils.ContinueTarget();
    }
    if (string.IsNullOrEmpty(str))
    {
        return string.Empty;
    }
    return str.Remove(str.Length - separator.Length, separator.Length);
}
```

#### continue clause and break clause
`Variadic.Break` and `VariadicUtils.Continue` methods can be used to express break and continue clauses.  

```.cs
using (new VariadicForEach())
{
    if (isBreak)
    {
        Variadic.Break();
    }

    if (isContinue)
    {
        Variadic.Continue();
    }
}
```

#### Expand and Pick
Use the `ExpandVariadicParameters` scope if you want to expand type arguments or arguments inside the `VariadicForEach` scope.  

```.cs
using (new VariadicForEach())
{
    using (new ExpandVariadicParameters()
    {
        Action<T> action = ...;
        action?.Invoke(args);
        // ↓
        // Action<T__0, T__1, ...> action = ...;
        // action?.Invoke(args__0, args__1, ...);
    }
}
```
In addition, the `PickVariadicParameter` scope can again be used to retrieve individual type argument elements.

```.cs
using (new VariadicForEach())
{
    using (new ExpandVariadicParameters()
    {
        :

        using(new PickVariadicParameter())
        {
            Debug.Log((T)args);
            // ↓
            // Debug.Log((T__x)args__x);
        }
    }
}
```

### Check the count of elements in the type argument
You can use `VariadicUtils.VariadicParameterCount` to get the number of elements in a variable-length argument.  
The following example converts the given arguments to an array.
```.cs
[VariadicGeneric(1, 16)]
public static object[] MakeArray<T>(T args)
{
    int i = 0;
    var values = new object[VariadicUtils.VariadicParameterCount];
    using (new VariadicForEach())
    {
        values[i] = args;
        ++i;
    }

    return values;
}
```

### Recursively retrieve type argument element.
There is also a technique of sequential retrieval from the first element by recursive processing.  
The following example concatenates variadic arguments.  

```.cs
public static string RecursiveJoin<TFirst>(string separator, TFirst arg)
{
    return arg.ToString();
}

[VariadicGeneric(1, 16)]
public static string RecursiveJoin<TFirst, TVar>(string separator, TFirst arg, TVar argVars)
{
    var str = RecursiveJoin<TVar>(separator, argVars);
    return $"{arg}{separator}{str}";
}
```

The following code is generated.

```.cs
[global::Katuusagi.GenericEnhance.VariadicGenerated(2)]
[VariadicGeneric(1, 16)]
public static string RecursiveJoin<TFirst, TVar__0, TVar__1>(string separator, TFirst arg, TVar__0 argVars__0, TVar__1 argVars__1)
{
    var str = RecursiveJoin<TVar__0, TVar__1>(separator, argVars__0, argVars__1);
    return $"{arg}{separator}{str}";
}

:

[global::Katuusagi.GenericEnhance.VariadicGenerated(16)]
[VariadicGeneric(1, 16)]
public static string RecursiveJoin<TFirst, TVar__0, TVar__1, TVar__2, TVar__3, TVar__4, TVar__5, TVar__6, TVar__7, TVar__8, TVar__9, TVar__10, TVar__11, TVar__12, TVar__13, TVar__14, TVar__15>(string separator, TFirst arg, TVar__0 argVars__0, TVar__1 argVars__1, TVar__2 argVars__2, TVar__3 argVars__3, TVar__4 argVars__4, TVar__5 argVars__5, TVar__6 argVars__6, TVar__7 argVars__7, TVar__8 argVars__8, TVar__9 argVars__9, TVar__10 argVars__10, TVar__11 argVars__11, TVar__12 argVars__12, TVar__13 argVars__13, TVar__14 argVars__14, TVar__15 argVars__15)
{
    var str = RecursiveJoin<TVar__0, TVar__1, TVar__2, TVar__3, TVar__4, TVar__5, TVar__6, TVar__7, TVar__8, TVar__9, TVar__10, TVar__11, TVar__12, TVar__13, TVar__14, TVar__15>(separator, argVars__0, argVars__1, argVars__2, argVars__3, argVars__4, argVars__5, argVars__6, argVars__7, argVars__8, argVars__9, argVars__10, argVars__11, argVars__12, argVars__13, argVars__14, argVars__15);
    return $"{arg}{separator}{str}";
}
```

The elements are taken out in order from the top by decreasing the number of arguments one by one and recursing.  

## Default type argument
*This function is limited to the Generic method.  

Specifying a default type argument for a type argument allows the type argument to be omitted.  

### How To Implementation
Use the `DefaultType` attribute to specify a default type argument.  
The following example specifies the default type of int for T2 type arguments.  
The default type argument must be specified from the end.  

```.cs
public static string ToString<T1, [DefaultType(typeof(int))] T2>(T1 a, T2 b = default)
{
    return $"{a}, {b}";
}

:

var s = ToString(5);
Debug.Log(s); // 5, 0
```
