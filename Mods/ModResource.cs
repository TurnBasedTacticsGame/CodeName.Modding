using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeName.Modding.Mods
{
    [Serializable]
    public class ModResource
    {
        [EnableIf(nameof(isOverride))]
        [SerializeField] private string key;

        [EnableIf(nameof(isOverride))]
        [SerializeField] private Object asset;

        [SerializeField] private bool isOverride;

        public ModResource(string key, Object asset)
        {
            this.key = key;
            this.asset = asset;
        }

        public string Key => key;
        public Object Asset => asset;

        public bool IsOverride => isOverride;
    }
}
