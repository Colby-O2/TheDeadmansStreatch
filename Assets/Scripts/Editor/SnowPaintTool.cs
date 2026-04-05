using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace ColbyO.Untitled
{
    [EditorTool("Snow Paint Tool", typeof(Terrain))]
    public class SnowPaintTool : EditorTool
    {
        public override GUIContent toolbarIcon => new GUIContent("Snow", "Paint Snow Mask");

        private Material _brushMaterial;
        private float _brushSize = 25f;
        private float _brushStrength = 0.1f;

        private void Init(Terrain terrain, TerrainSnowData data)
        {
            if (_brushMaterial == null)
                _brushMaterial = new Material(Shader.Find("Hidden/SnowBrush"));

            if (data.SnowMask == null || !data.SnowMask.IsCreated())
            {
                data.SnowMask = new RenderTexture(1024, 1024, 0, RenderTextureFormat.R8);
                data.SnowMask.hideFlags = HideFlags.DontSave;
                data.SnowMask.Create();

                if (data.SavedSnowTexture != null)
                {
                    Graphics.Blit(data.SavedSnowTexture, data.SnowMask);
                }
                else
                {
                    RenderTexture.active = data.SnowMask;
                    GL.Clear(true, true, Color.clear);
                    RenderTexture.active = null;
                }
            }

            Shader.SetGlobalTexture("_SnowMask", data.SnowMask);
        }

        public override void OnToolGUI(EditorWindow window)
        {
            Terrain terrain = target as Terrain;
            if (terrain == null) return;

            TerrainSnowData data = terrain.GetComponent<TerrainSnowData>() ?? terrain.gameObject.AddComponent<TerrainSnowData>();
            Init(terrain, data);

            Event e = Event.current;

            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(10, 10, 250, 100), "Snow Brush Settings", GUI.skin.window);

            EditorGUI.BeginChangeCheck();
            _brushSize = EditorGUILayout.Slider("Size", _brushSize, 1f, 200f);
            _brushStrength = EditorGUILayout.Slider("Strength", _brushStrength, 0.01f, 1f);
            if (EditorGUI.EndChangeCheck())
            {
                window.Repaint();
            }

            bool isErasing = e.shift;
            GUILayout.Label(isErasing ? "MODE: ERASING" : "MODE: PAINTING");

            if (GUILayout.Button("Bake Texture"))
            {
                SaveTexture(data);
            }

            GUILayout.EndArea();
            Handles.EndGUI();

            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Handles.color = isErasing ? Color.red : Color.cyan;
                Handles.DrawWireDisc(hit.point, hit.normal, _brushSize);

                if (e.mousePosition.x < 260 && e.mousePosition.y < 110) return;

                if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0)
                {
                    Vector3 localPos = terrain.transform.InverseTransformPoint(hit.point);
                    Vector2 uv = new Vector2(localPos.x / terrain.terrainData.size.x, localPos.z / terrain.terrainData.size.z);
                    float uvRadius = _brushSize / terrain.terrainData.size.x;

                    Paint(data.SnowMask, uv, uvRadius, isErasing);

                    EditorUtility.SetDirty(data);
                    e.Use();
                }
            }

            if (e.type == EventType.MouseMove) window.Repaint();
        }

        private void Paint(RenderTexture mask, Vector2 uv, float uvRadius, bool erase)
        {
            RenderTexture temp = RenderTexture.GetTemporary(mask.descriptor);

            _brushMaterial.SetVector("_BrushParams", new Vector4(uv.x, uv.y, uvRadius, _brushStrength));
            _brushMaterial.SetFloat("_Erase", erase ? 1f : 0f);

            Graphics.Blit(mask, temp);
            Graphics.Blit(temp, mask, _brushMaterial);

            RenderTexture.ReleaseTemporary(temp);
        }

        private void SaveTexture(TerrainSnowData data)
        {
            string folderPath = Application.dataPath + "/Resources/SnowMasks";
            if (!System.IO.Directory.Exists(folderPath))
                System.IO.Directory.CreateDirectory(folderPath);

            string fileName = "SnowMask_" + data.gameObject.name;
            string fullPath = folderPath + "/" + fileName + ".png";
            string assetPath = "Assets/Resources/SnowMasks/" + fileName + ".png";

            Texture2D tex = new Texture2D(data.SnowMask.width, data.SnowMask.height, TextureFormat.R8, false);
            RenderTexture.active = data.SnowMask;
            tex.ReadPixels(new Rect(0, 0, data.SnowMask.width, data.SnowMask.height), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            System.IO.File.WriteAllBytes(fullPath, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(assetPath);

            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Default;
                importer.sRGBTexture = false;
                importer.isReadable = true;
                importer.SaveAndReimport();
            }

            DestroyImmediate(tex);
        }
    }
}