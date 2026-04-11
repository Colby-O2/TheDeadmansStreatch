using System.Collections.Generic;
using UnityEngine;

namespace ColbyO.Untitled
{
    public class InventoryMonoSystem : MonoBehaviour, IInventoryMonoSystem
    {
        private Dictionary<string, bool> _inventroy = new Dictionary<string, bool>();

        public void GiveItem(string tag)
        {
            if (_inventroy.ContainsKey(tag)) _inventroy[tag] = true;
            else _inventroy.Add(tag, true);
        }

        public void TakeItem(string tag)
        {
            if (_inventroy.ContainsKey(tag)) _inventroy[tag] = false;
            else _inventroy.Add(tag, false);
        }

        public bool HasItem(string tag)
        {
            return _inventroy.ContainsKey(tag) && _inventroy[tag];
        }
    }
}