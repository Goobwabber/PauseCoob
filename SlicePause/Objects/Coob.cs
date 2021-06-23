using UnityEngine;

namespace SlicePause.Objects
{
    public class Coob : MonoBehaviour
    {
        MeshRenderer renderer = null!;
        Material material = null!;

        public Color color {
            get => material.color;
            set => SetColor(value);
        }

        public float scale
        {
            get => Plugin.Config.Scale;
            set => SetScale(value);
        }

        public void Awake()
        {
            renderer = gameObject.GetComponent<MeshRenderer>();
            material = renderer.material;
        }

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            Plugin.Config.PosX = position.x;
            Plugin.Config.PosY = position.y;
            Plugin.Config.PosZ = position.z;

            transform.rotation = rotation;
            Plugin.Config.RotX = rotation.x;
            Plugin.Config.RotY = rotation.y;
            Plugin.Config.RotZ = rotation.z;
            Plugin.Config.RotW = rotation.w;
        }

        public void SetColor(Color color)
        {
            material.color = color;
            Plugin.Config.Color = "#" + ColorUtility.ToHtmlStringRGBA(color);
        }

        public void SetScale(float scale)
        {
            transform.localScale = Vector3.one * scale;
            Plugin.Config.Scale = scale;
        }

        public void Refresh()
        {
            Vector3 position = new Vector3(Plugin.Config.PosX, Plugin.Config.PosY, Plugin.Config.PosZ);
            Quaternion rotation = new Quaternion(Plugin.Config.RotX, Plugin.Config.RotY, Plugin.Config.RotZ, Plugin.Config.RotW);
            SetPositionAndRotation(position, rotation);
            SetScale(Plugin.Config.Scale);

            Color color;
            if (ColorUtility.TryParseHtmlString(Plugin.Config.Color, out color))
            {
                SetColor(color);
            }
        }
    }
}
