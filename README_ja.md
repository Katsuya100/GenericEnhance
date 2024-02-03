# GenericEnhance
## 概要
本ライブラリ「GenericEnhance」はGenericプログラミングを強力にサポートします。  

- Generic特殊化
- TypeDef
- Generic非型引数
- 省略型
- 可変長型引数
- デフォルト型引数

これらはC++のテンプレートで可能だった機能の一部をC#のGenericプログラミングで使えるよう取り入れています。

## 動作確認環境
|  環境  |  バージョン  |
| ---- | ---- |
| Unity | 2021.3.15f1, 2022.3.2f1 |
| .Net | 4.x, Standard 2.1 |

## インストール方法
### 依存パッケージをインストール
以下のパッケージをインストールする。  

- [ILPostProcessorCommon v2.1.0](https://github.com/Katsuya100/ILPostProcessorCommon/tree/v2.1.0)
- [MemoizationForUnity v1.4.1](https://github.com/Katsuya100/MemoizationForUnity/tree/v1.4.1)

### GenericEnhanceのインストール
1. [Window > Package Manager]を開く。
2. [+ > Add package from git url...]をクリックする。
3. `https://github.com/Katsuya100/GenericEnhance.git?path=packages`と入力し[Add]をクリックする。

#### うまくいかない場合
上記方法は、gitがインストールされていない環境ではうまく動作しない場合があります。
[Releases](https://github.com/Katsuya100/GenericEnhance/releases)から該当のバージョンの`com.katuusagi.genericenhance.tgz`をダウンロードし
[Package Manager > + > Add package from tarball...]を使ってインストールしてください。

#### それでもうまくいかない場合
[Releases](https://github.com/Katsuya100/GenericEnhance/releases)から該当のバージョンの`Katuusagi.GenericEnhance.unitypackage`をダウンロードし
[Assets > Import Package > Custom Package]からプロジェクトにインポートしてください。

## Generic特殊化
※Genericメソッド限定の機能です。  

Generic特殊化とは、与えられた型引数に応じて異なる実装を呼び出す仕組みです。  
これによって

- 型ごとに最適化した処理を呼び出す
- 型を使って実装を分岐する

といったことが可能です。

### 実装方法
特殊化は`SpecializationMethod`属性と`SpecializedMethod`属性を使用します。  
`SpecializationMethod`属性ではデフォルトで呼び出される関数を指定します。  
`SpecializedMethod`属性では特殊化時に呼び出される関数と特殊化条件の型引数をしていします。  
例ではGetValueを特殊化しています。  

```.cs
using Katuusagi.GenericEnhance;

[SpecializationMethod(nameof(GetInternal))]  // 基本的にGetInternalが呼び出される
[SpecializedMethod(nameof(GetInteger), typeof(int))]  // 型引数にintが入っている場合はGetIntegerが呼び出される
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

// GetValueInternalが呼び出される
var f = GetValue<float>();
Debug.Log(f); // 0

// GetIntegerが呼び出される
var i = GetValue<int>();
Debug.Log(i); // 100;
```

### アルゴリズムの指定
特殊化には複数のアルゴリズムが存在します。  
アルゴリズムは`SpecializationMethod`属性の第2引数で指定できます。  
アルゴリズムによって特徴が異なるため、実際の実装環境に応じて使い分けましょう。  

|  アルゴリズム  |  特徴  |
| ---- | ---- |
| VirtualStrategy | 仮想関数を用いたストラテジーパターンで実装します。<br>初回呼び出し時にキャッシュを作成します。<br>仮想関数呼び出しのオーバーヘッドが発生します。 |
| DelegateStrategy | Delegateを用いたストラテジーパターンで実装します。<br>初回呼び出し時にキャッシュを作成します。<br>それ以降も型の組み合わせごとに小さなキャッシュが作成されます。<br>呼び出しが高速です。 |
| TypeComparison | 型を一つ一つ比較します。<br>キャッシュを作成しません。<br>JIT環境では最適化されるため非常に高速です。 |
| TypeIdComparison | 型の組み合わせにIDを割り当てて一括で比較します。<br>型比較用の小さなキャッシュが作成されます。<br>それ以降も型の組み合わせごとに小さなキャッシュが作成されます。<br>AOT環境でも高速に呼び出せます。 |

パフォーマンスの計測結果は以下を参照してください。

#### エディタ上の計測
[テストコード](packages/Tests/Runtime/SpecializationPerformanceTest.cs)  

|  実行処理  |  性能  |
| ---- | ---- |
| Direct | 0.014535 ms |
| VirtualStrategy | 0.07683 ms |
| DelegateStrategy | 0.067435 ms |
| TypeComparison | 0.036765 ms |
| TypeIdComparison | 0.04558 ms |

エディタではJITコンパイルが可能なため`TypeComparison`が高いパフォーマンスを発揮します。  

#### ビルド後の計測
計測コードは以下です。
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

|  実行処理  |  Mono(Debug)  |  Mono(Release)  |  IL2CPP(Debug)  |  IL2CPP(Release)  |
| ---- | ---- | ---- | ---- | ---- |
| Direct | 0.01612091 ms | 0.002952576 ms | 0.0004882813 ms | 0 ms(計測不能) |
| VirtualStrategy | 0.161232 ms | 0.06460953 ms | 2.692383 ms | 2.297852 ms |
| DelegateStrategy | 0.1258011 ms | 0.06257629 ms | 0.05908203 ms | 0.04199219 ms |
| TypeComparison | 0.08607483 ms | 0.02434921 ms | 5.053223 ms | 3.029297 ms |
| TypeIdComparison | 0.0706749 ms | 0.03424454 ms | 0.07226563 ms | 0.07714844 ms |

いずれも直接呼び出しには劣りますが、処理速度だけで判断するならおすすめは以下です。

- *Mono(Debug)* : TypeIdComparison
- *Mono(Release)* : TypeComparison
- *IL2CPP* : DelegateStrategy

環境による差を考慮しない場合は`TypeIdComparison`が安定したパフォーマンスを発揮します。  

ただし、アルゴリズムによってキャッシュ作成のタイミングや頻度に差があるため、用途を考慮して選択してください。  
`VirtualStrategy`はキャッシュサイズが小さく、キャッシュ作成の頻度が低いためパフォーマンスへの悪影響も低いと言えます。  
`TypeComparison`はキャッシュを作成しないため、メモリ面で最も高いパフォーマンスを発揮できます。  

### リプレース最適化
以下の条件が揃っている場合に限り、アルゴリズムを問わず直接呼び出しにリプレースされます。  
直接呼び出しと同じパフォーマンスを発揮できますので、積極的な活用を推奨します。  

1. 指定した関数がpublicである  
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
2. 型引数に他の型引数を与えない  
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
`TypeDef`属性を用いることで、すでにある型を別名で再定義できます。  
エイリアスとしての利用の他、Generic内部から生成した型を返却するのにも使えます。  

### 実装方法
以下のように実装することで型の再定義を行います。

```.cs
using Katuusagi.GenericEnhance;

[TypeDef(typeof(int))]
public struct DefInt
{
}
```

### メンバの再定義
`TypeDef`属性を持つ型に同名のメンバを持たせることで、元の型が持つメンバを呼び出すことが可能です。  

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

### 型引数の再定義
型引数を再定義したい場合は文字列で型名を指定します。  
`nameof`句を使うことも可能です。  

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

### 型の再解釈
`TypeDef`属性の付いている型はILレベルでリプレースされます。  
しかし、C#側から見ると別の型として解釈されています。  
C#から型を再解釈したい場合は`CastUtils.SafeAs`関数を用いましょう。
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

再解釈の失敗を検知する場合は`CastUtils.TryAs`関数を用いましょう。
```.cs
int intValue = 100;
if (!CastUtils.TryAs(ref intValue, out DefInt defIntValue))
{
    // failed.
}

Debug.Log(defIntValue); // 100
```

### 注意点
値型は値型同士、参照型は参照型同士でしか`TypeDef`属性を指定できません。  
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

## Generic非型引数
このライブラリの導入後、Generic引数として非型引数を与えられるようになります。  
厳密には非型情報を型情報にラップしそれをGeneric引数に渡します。  
これにより、非型情報を型引数から渡せるようになり、定数性を保証できるようになります。  
Generic特殊化と組み合わせることで型情報を使った分岐も可能になります。  

### 実装方法
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

### 演算処理
演算を行う場合はOperator型を使用します。  
下記の例では`Add`型という足し算用Operator型を使用しています。

```.cs
var value = TypeFormula.GetValue<Add<int, _100, _10>, int>();
Debug.Log(value); // 110
```

#### Operator型の種類
##### 四則演算
|  Operator型  |  内容  |
| ---- | ---- |
| Add<T, TX, TY> | (T)TX + (T)TY |
| Sub<T, TX, TY> | (T)TX - (T)TY |
| Mul<T, TX, TY> | (T)TX * (T)TY |
| Div<T, TX, TY> | (T)TX / (T)TY |
| Mod<T, TX, TY> | (T)TX % (T)TY |
| Minus<T, TX> | -((T)TX) |

##### 条件論理演算
|  Operator型  |  内容  |
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

##### ビット論理演算
|  Operator型  |  内容  |
| ---- | ---- |
| BitAnd<T, TX, TY> | (T)TX & (T)TY |
| BitOr<T, TX, TY> | (T)TX \| (T)TY |
| BitXor<T, TX, TY> | (T)TX ^ (T)TY |
| BitNot<T, TX> | ~(T)TX |
| LShift<T, TX, TY> | (T)TX << (int)TY |
| RShift<T, TX, TY> | (T)TX >> (int)TY |

##### キャスト演算
|  Operator型  |  内容  |
| ---- | ---- |
| CastNumeric<T, TX> | (Numeric型)(T)TX |

### TypeDefを使って型を定数のように扱う
Operator型は`TypeDef`の置換の対象となるため`TypeDef`で定義した型を定数のように扱うことができます。

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

### リプレース最適化
異なる型引数が与えられていない場合、演算結果がリプレースされます。  

```.cs
// OK
Debug.Log(typeof(Add<int, _100, _10>)); // _110
// ↓
// Debug.Log(typeof(_110));

// NG
Debug.Log(typeof(Add<int, _100, T>)); // Add<int, _100, T>
```

## 省略型
引数に`NoneType`型を指定すると、その引数が内部的に無効化されます。  
主にGeneric引数のパディングとして用います。  

### 実装方法
`NoneType`型を型引数に指定すると、その型引数が省略されます。  
また、`NoneType.Default`を引数に指定すると、その引数が省略されます。  

```.cs
Debug.Log(typeof(Action<int, NoneType>)); // Action`1
Debug.Log(typeof(Action<NoneType>)); // Action

Action<int, NoneType> action = (x, y) => {};
action(0, NoneType.Default);
// ↓
// action(0);
```

## 可変長型引数
※Genericメソッド限定の機能です。  

指定した数まで型引数を複製します。  
厳密な意味での可変長型引数ではないため要注意です。  

### 実装方法
可変長引数を実装するには`VariadicGeneric`属性を用います。  
`VariadicGeneric`属性には型引数の最小要素数と型引数の最大要素数を指定します。  

```.cs
[VariadicGeneric(0, 16)]
public static Type GetActionType<T>()
{
    return typeof(Action<T>);
}
```
すると、以下のように実装が複製されます。
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
複製されるのは末尾の型引数のみです。

### 引数の可変長化
この実装は、引数の要素数を可変長型引数と同じ要素数だけ複製することが可能です。  
引数を複製したい場合は引数の末尾を末尾の型引数と同じ型にする必要があります。  
例は以下です。
```.cs
[VariadicGeneric(0, 16)]
public static void InvokeAction<TVar>(Action<TVar> action, TVar args)
{
    action?.Invoke(args);
}
```
この場合、以下のように実装が複製されます。
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

### 繰り返し処理で型引数要素を取り出す
`VariadicForEach`スコープを記述することで、foreach文のように可変長型引数をイテレーションできます。  
`VariadicForEach`スコープの中では型引数や引数を呼び出しても展開されません。  
全要素が繰り返し呼び出されます。  
以下の例は`VariadicForEach`スコープを用いて可変長引数を連結しています。  
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
この場合、以下のように繰り返し処理が生成されます。
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

#### continue句やbreak句
`VariadicUtils.Break`関数や`VariadicUtils.Continue`関数を用いればbreak句やcontinue句を表現できます。  

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

#### 展開と抽出
`VariadicForEach`スコープの中で型引数や引数を展開したい場合は`ExpandVariadicParameters`スコープを用います。  

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
さらに`PickVariadicParameter`スコープを用いれば、再び個別の型引数要素を取得できるようになります。

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

### 型引数の要素数を調べる
`VariadicUtils.VariadicParameterCount`を使うことで可変長型引数の要素数を取得できます。  
以下の例では与えられた引数を配列に変換しています。
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

### 再帰的に型引数要素を取り出す
再帰処理によって先頭要素から順に取り出すテクニックもあります。  
以下の例は可変長引数を連結しています。  

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

生成されるのは以下のコードです。

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

一つずつ引数を減らしながら再起することで先頭から順に要素が取り出されています。  

## デフォルト型引数
※Genericメソッド限定の機能です。  

型引数にデフォルト型引数を指定すると、型引数を省略して記述できるようになります。  

### 実装方法
デフォルト型引数を指定するには`DefaultType`属性を使用します。  
以下の例ではT2型引数にintのデフォルト型を指定しています。  
デフォルト型引数は末尾から指定する必要があります。  

```.cs
public static string ToString<T1, [DefaultType(typeof(int))] T2>(T1 a, T2 b = default)
{
    return $"{a}, {b}";
}

:

var s = ToString(5);
Debug.Log(s); // 5, 0
```
