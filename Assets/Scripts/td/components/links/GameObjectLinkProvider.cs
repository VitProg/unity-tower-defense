using System;
using Mitfart.LeoECSLite.UniLeo.Providers;
using UnityEngine;

namespace td.components.links
{
    [Serializable]
    public struct GameObjectLink
    {
        public GameObject gameObject;
    }

    public class GameObjectLinkProvider : EcsProvider<GameObjectLink>
    {
    }
}