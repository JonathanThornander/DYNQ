# DYNQ
![android-chrome-192x192](https://user-images.githubusercontent.com/43991450/216784507-f1bf470e-6314-4136-9123-1bedbf251e2b.png)

DYNQ (Dynamic Queue) is a light weight pub/sub -library in .NET for creating publishers and subscribers dynamicly, in runtime.

## Installation
Install DYNQ either by browsing the nuget.org feed Under 'Project -> Manage Nuget Packages'. You can also use the developer console:
```powershell
dotnet add package Dynq
```
Or with Nuget Package Manager:
```powershell
Install-Package Dynq
```

# DYNQ for Blazor
![android-chrome-192x192](https://user-images.githubusercontent.com/43991450/217499236-0d5b91c5-0595-4268-a845-80a2b113ac42.png)

DYNQ goes very well with Blazor. Let's say you're building a Blazor component that need to react to changes from the application. By injecting IDynqService into the component, the component can deynamicly update depending on the content of a message for which the component subscribes to.

## Example
To get started, add the following line in Program.cs. This is everything you need to set up DYNQ.
```csharp
builder.Services.AddDynqServices();
```

A message can be created by implementing the Dyqn.Message class.
```csharp
using Dynq;

namespace Dynisplay.Messages
{
    public class InfoMessage : Message
    {
        public required string Payload { get; set; }
    }
}
```

Lets create a simple page (Blazor Component) for creating and publishing a message.
```csharp
@using Dynq;

@page "/"
@inject IDynqService DynqService;

<PageTitle>Sender</PageTitle>

<InputText @bind-Value="TextToSend" />
<button @onclick="SendMessage">Send</button>

@code {
    public string TextToSend { get; set; } = string.Empty;

    private async Task SendMessage()
    {
        await DynqService.BroadcastAsync(new SampleMessage { Payload = TextToSend });
    }
}
```

Let's now create a page (Blazor Component) that listens for new messages and displays it's content on screen. By wrapping other components with DynqListen, the components will be triggered for re-render whenever a new message of the given type is broadcasted. The content of the message can be accessed through the '@context'-variable.
```csharp
@using Dynq.Blazor

@page "/receiver"

<PageTitle>Receiver</PageTitle>

<DynqListen TMessage="SampleMessage">
    @(context?.Payload ?? "Listening...")
</DynqListen>
```

![image](https://user-images.githubusercontent.com/43991450/217478562-64fba31b-d2cb-45cb-9cc6-39220aab58d4.png)
![image](https://user-images.githubusercontent.com/43991450/217478693-c92f2777-731a-499f-8f1c-ae247930cfb9.png)
