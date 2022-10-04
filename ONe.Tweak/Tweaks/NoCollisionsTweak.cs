using ONe.Tweak;
using UnityEngine;

namespace Kitchen.ONe.Tweak.Tweaks;

public static class NoCollisionsTweak
{
    private static bool _collisionsEnabled = true;
    
    public static void ToggleCollisions()
    {
        _collisionsEnabled = !_collisionsEnabled;
        
        foreach (var playerView in GameObject.FindObjectsOfType<PlayerView>())
        {
            var player = playerView.gameObject;
            var colliders = playerView.GetComponents<Collider>();

            foreach (var collider in colliders)
            {
                collider.enabled = _collisionsEnabled;
            }
        }
    }
}

public class NoCollisionsGUIConfig : TweakGUIConfig
{
    public override string Section => "Creative";

    public override string Name => "Toggle collisions";
    
    public override void Init()
    {
        BindButton("Toggle collisions", Run);
    }

    private void Run()
    {
        NoCollisionsTweak.ToggleCollisions();
    }
}