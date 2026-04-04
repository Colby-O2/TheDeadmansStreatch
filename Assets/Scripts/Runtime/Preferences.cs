using UnityEngine;


namespace ColbyO.Untitled
{
    [CreateAssetMenu(fileName = "DefaultPreferences", menuName = "Preferences")]
    public class Preferences : ScriptableObject
    {
        public Texture2D Cursor;
        public float DialogueSpeedMul = 1f;
    }
}
