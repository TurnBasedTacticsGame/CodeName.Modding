using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeName.Modding.Mods
{
    [Serializable]
    public class ModResource
    {
        [ReadOnly]
        [SerializeField] private string key;

        [ReadOnly]
        [SerializeField] private Object asset;

        public ModResource(string key, Object asset)
        {
            this.key = key;
            this.asset = asset;
        }

        public string Key => key;
        public Object Asset => asset;
    }
}
