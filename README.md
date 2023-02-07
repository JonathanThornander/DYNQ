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

# DYNQ for ASPNetCore
![android-chrome-192x192](https://user-images.githubusercontent.com/43991450/216784972-00582b51-c98b-4048-9cd2-e92b3fc82fc3.png)

DYNQ suits very well in many ASP.NET-applicaion where dynamic components are behing used. To get started, add the following line in conjunction with other calls to 'builder.Services' in Program.cs

```csharp
builder.Services.AddDynqServices();
```

Let's say you're building a Blazor component that need to react to changes from the application. By injecting IDynqService into the component, the component can deynamicly update depending on the content of a message for which the component subscribes to.

## Example
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


```csharp
@page "/"
@inject IDynqService Dynq

<PageTitle>Index</PageTitle>

<InputText @bind-Value="MessageToSend" />
<button @onclick="SendAsync">Send</button>

@code {
    public string MessageToSend { get; set; } = string.Empty;

    private async Task SendAsync()
    {
        await Dynq.BroadcastAsync(new InfoMessage() { Payload = MessageToSend });
    }
}
```

```csharp
@page "/other"

@implements IDisposable
@inject IDynqService Dynq

@Message

@code {
    private MessageSubscription? _infoMessageSubscription;

    public string Message { get; set; } = "Send a message and it will appear here";

    protected override void OnInitialized()
    {
        _infoMessageSubscription = Dynq.Subscribe<InfoMessage>(HandleInfoMessage);
    }

    private void HandleInfoMessage(InfoMessage message)
    {
        Message = message.Payload;
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        _infoMessageSubscription?.Dispose();
    }
}

```
