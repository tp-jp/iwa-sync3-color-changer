using HoshinoLabs.IwaSync3;
using UnityEngine;

namespace TpLab.IwaSync3ColorChanger.Editor
{
    /// <summary>
    /// IwaSync3カラーチェンジャーの設定。
    /// </summary>
    public class ColorChangerSetting : ScriptableObject
    {
        public IwaSync3Base target;
        public Color mainColor = DefaultColor.Main;
        public Color backgroundColor = DefaultColor.Background;
        public Color borderColor = DefaultColor.Border;
    }
}
