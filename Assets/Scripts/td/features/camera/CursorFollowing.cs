using System;
using Cinemachine;
using td.common;
using UnityEngine;

namespace td.features.camera
{
    [Serializable]
    [GenerateProvider]
    public struct CursorFollowing
    {
        public CinemachineVirtualCamera virtualCamera;
    }
}