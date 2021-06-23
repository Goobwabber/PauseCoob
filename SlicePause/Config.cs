namespace SlicePause
{
    public class PluginConfig
    {
        public virtual float PosX { get; set; } = 0f;
        public virtual float PosY { get; set; } = 0f;
        public virtual float PosZ { get; set; } = 0f;
        public virtual float RotX { get; set; } = 0f;
        public virtual float RotY { get; set; } = 0f;
        public virtual float RotZ { get; set; } = 0f;
        public virtual float RotW { get; set; } = 0f;
        public virtual float Scale { get; set; } = 1f;
        public virtual string Color { get; set; } = "#00000000";
    }
}
