# JavaScript

## Executing JS

You can execute JS in the browser using the <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient.ExecuteJs(System.String)> method. The method takes in a string containing your full JS code, and executes it on the current page loaded in the browser.

```csharp
browserClient.ExecuteJs("console.log('Hello World');");
```

Which will result in the log message being logged out:

![Log](~/assets/images/articles/user/javascript/hello-world-log.webp)

## JS Methods

JS methods are a way of calling .NET methods from JS code. JS methods are invoked on the JS side by calling `uwb.ExecuteJsMethod(<method name>, arguments);`. The `ExecuteJsMethod` function, on success, will immediately return `true`.

### Argument Data Types

Arguments can be used to pass data from the JS world to .NET. The supported data types are as follows.

- [`Int`](xref:System.Int32) - `number`
- [`UInt`](xref:System.UInt32 )- `number`
- [`Double`](xref:System.Double) - `number`
- [`Bool`](xref:System.Boolean) - `boolean`
- [`String`](xref:System.String) - `string`
- [`DateTime`](xref:System.DateTime) - `Date`
- And custom objects.

> [!NOTE]
> Dates are always handled using UTC time. A JS `Date` object, even in a local timezone (such as AEST), will be converted to UTC, and passed to your method in UTC.

Arrays of any type are currently not supported.

#### Custom Objects

JS objects can be automatically converted to .NET objects. The object can use any of the data types above, as well as child custom objects.

So a JS object like so can be passed in as an argument.

```js
{
    Test: 'Test',
    Child: {
        Number: 1234,
        Test: 'Hello World!'
    }
}
```

The C# class representation of the JS objects would be:

```csharp
public class ChildClass
{
    public int Number { get; set; }
    public string Test { get; set; }
}

public class ObjectClass
{
    public string Test { get; set; }
    public ChildClass Child { get; set; }
}
```

Your C# classes must use [properties](https://learn.microsoft.com/en-us/dotnet/csharp/properties), not fields. Objects are also the only data type that can be passed in as null (or undefined, but no matter what, the .NET world will receive it as null), so you should always check for null in your C# code.

### Method Requirements

There are a few requirements of your target method that you want to call.

- It must return `void`.
- Arguments must follow the data type requirements above

### Enabling JS Methods

By default, JS methods are entirely disabled. You can enable it by setting <xref:VoltstroStudios.UnityWebBrowser.Core.Js.JsMethodManager.jsMethodsEnable> to `true`. You can do this in the Unity editor UI.

![Enable JS Methods](~/assets/images/articles/user/javascript/enable-js-methods.webp)

### Registering Methods

To register a method, use the <xref:VoltstroStudios.UnityWebBrowser.Core.WebBrowserClient.RegisterJsMethod(System.String,System.Action)> set of methods. This method takes the name of the method you want use and an <xref:System.Action>. You can register methods like so.

```csharp
public class UWBPrjDemo : MonoBehaviour
{
    [SerializeField]
    private WebBrowserUIBasic uiBasic;
    
    public void Start()
    {
        uiBasic.browserClient.RegisterJsMethod("Test", TestMethod);
        uiBasic.browserClient.RegisterJsMethod<int>("TestInt", TestMethodInt);
        uiBasic.browserClient.RegisterJsMethod<string>("TestString", TestMethodString);
        uiBasic.browserClient.RegisterJsMethod<DateTime>("TestDate", TestMethodDate);
        uiBasic.browserClient.RegisterJsMethod<TestClass>("TestObject", TestMethodObject);
        uiBasic.browserClient.RegisterJsMethod<TestClassChild>("TestObjectChild", TestMethodObjectChild);
    }

    private void TestMethod()
    {
        Debug.Log("Hello from test method!");
    }

    private void TestMethodInt(int value)
    {
        Debug.Log($"Hello from test method! Value was {value}.");
    }
    
    private void TestMethodString(string value)
    {
        Debug.Log($"Hello from test method! Value was {value}.");
    }
    
    private void TestMethodDate(DateTime value)
    {
        DateTime localTime = value.ToLocalTime();
        Debug.Log($"Hello from test method! Value in UTC time was {value:yyyy-MM-dd HH:mm:ss zzzz}. Value in local time was {localTime:yyyy-MM-dd HH:mm:ss zzzz}.");
    }

    private void TestMethodObject(TestClass? test)
    { 
        Debug.Log($"Hello from test method! Value on TestClass was {test?.Test}.");   
    }

    private void TestMethodObjectChild(TestClassChild? test)
    {
        Debug.Log($"Hello from test method! Value on TestClassChild was {test?.What}, TestClass was {test?.Test?.Test}.");
    }
}
```

> [!TIP]
> By default, RegisterJsMethod can work up to 6 arguments. If you need more arguments simply just use an object.
>
> If you cannot use an object for some reason, you can use the low-level <xref:VoltstroStudios.UnityWebBrowser.Core.Js.JsMethodManager.RegisterJsMethod(System.String,System.Reflection.MethodInfo,System.Object)> method instead.

### JS Usage

In your JS code, you can call the methods like so.

```javascript
uwb.ExecuteJsMethod('Test');
uwb.ExecuteJsMethod('TestInt', 6969);
uwb.ExecuteJsMethod('TestString', 'Hello World!');
uwb.ExecuteJsMethod('TestDate', new Date());
uwb.ExecuteJsMethod('TestObject', {Test: 'Hello World!'});
uwb.ExecuteJsMethod('TestObjectChild', {Test: {Test: 'Hello World!'}, What: 'Voltstro Woz Here'});
```

![Results](~/assets/images/articles/user/javascript/executed-results.webp)
