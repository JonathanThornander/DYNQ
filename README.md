# DYNQ
![android-chrome-192x192](https://user-images.githubusercontent.com/43991450/216784507-f1bf470e-6314-4136-9123-1bedbf251e2b.png)

DYNQ is a light weight pub/sub -library in .NET for creating creating publishers and subscribers dynamicly, in runtime.

# DYNQ for ASPNetCore
Dynq suits very well in many ASP.NET-applicaion where dynamic components are behing used. To get started, add the following line in conjunction with other calls to 'builder.Services' in Program.cs

```csharp
builder.Services.AddDynqServices();
```

Let's say you're building a Blazor component that need to react to changes from the application. By injecting IDynqService into the component, the component can deynamicly update depending on the content of a message for which the component subscribes to.
