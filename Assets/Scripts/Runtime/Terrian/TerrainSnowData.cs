using UnityEngine;

namespace ColbyO.Untitled
{
    [ExecuteAlways]
    public class TerrainSnowData : MonoBehaviour
    {
        public RenderTexture SnowMask;
        public Texture2D SavedSnowTexture;

        private void OnEnable()
        {
            if (SavedSnowTexture == null)
            {
                string fileName = "SnowMasks/SnowMask_" + gameObject.name;
                SavedSnowTexture = Resources.Load<Texture2D>(fileName);
            }

            UpdateGlobalTexture();
        }

        private void Update()
        {
            UpdateGlobalTexture();
        }

        public void UpdateGlobalTexture()
        {
            if (SavedSnowTexture != null)
            {
                Shader.SetGlobalTexture("_SnowMask", SavedSnowTexture);
            }
            else if (Application.isEditor && SnowMask != null)
            {
                Shader.SetGlobalTexture("_SnowMask", SnowMask);
            }
        }
    }
}
