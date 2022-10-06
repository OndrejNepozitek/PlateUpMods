namespace Kitchen.ONe.Tweak.Utils;

public class TweakRestaurantSystem<TSystem> : RestaurantSystem
    where TSystem : TweakRestaurantSystem<TSystem>
{
    public static TSystem Instance { get; private set; }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        Instance = (TSystem) this;
    }

    protected override void OnUpdate()
    {
        
    }
}