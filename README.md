# SilK.Unturned.Extras
An OpenMod Unturned plugin which adds extra functionality for other plugins to use.

## Instance-based Event Listeners
The instance-based event listeners included with this plugin add the ability to easily define event callbacks within singleton services and plugins without needing to use an event bus or another class.

To use instance-based event listeners, you can inherit `IInstanceEventListener` or `IInstanceAsyncEventListener` on the following:
- Global services with a singleton lifetime
- Plugin services with a singleton lifetime
- Plugins

`IInstanceEventListener`'s callback will run synchronously with the game thread in most cases. This is why `IInstanceAsyncEventListener` has been added. The `IInstanceAsyncEventListener`'s callback will run on an alternate thread.

### Instance-based Event Listeners Example

By default, if you wanted to run an event callback in your service, you would have to do one of two things:
- Subscribe the callback using an event bus
- Create a separate IEventListener class which calls a method in your service

With the instance-based event listeners, this plugin will automatically register event callbacks within your singleton services and plugin classes by checking for classes which inherit `IInstanceEventListener` and `IAsyncInstanceEventListener`.

If I wanted to create a god mode manager service which subscribes to the `UnturnedPlayerDamagingEvent` callback, I could use the following code:

```csharp
[Service]
public class IGodManager
{
    void ToggleGodMode(ulong steamId);
}

[ServiceImplementation(Lifetime = Lifetime.Singleton)]
public class GodManager : IGodManager,
    IInstanceEventListener<UnturnedPlayerDamagingEvent>
{
    private readonly HashSet<ulong> _gods;
    
    public GodManager()
    {
        _gods = new HashSet<ulong>();
    }
    
    public void ToggleGodMode(ulong steamId)
    {
        if (_gods.Contains(steamId))
            _gods.Remove(steamId);
        else
            _gods.Add(steamId);
    }
    
    public UniTask HandleEventAsync(object? sender, UnturnedPlayerDamagingEvent @event)
    {
        if (_gods.Contains(@event.Player.SteamId.m_SteamID)
            @event.IsCancelled = true;
        return UniTask.CompletedTask;
    }
}
```
