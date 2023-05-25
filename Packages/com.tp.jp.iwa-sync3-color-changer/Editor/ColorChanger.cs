using HoshinoLabs.IwaSync3;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TpLab.IwaSync3ColorChanger.Editor
{
    /// <summary>
    /// IwaSync3カラーチェンジャー。
    /// </summary>
    public class ColorChanger : EditorWindow
    {
        /// <summary>
        /// IwaSync3カラーチェンジャーの設定
        /// </summary>
        ColorChangerSetting setting;

        /// <summary>
        /// 拡張エディタを開く。
        /// </summary>
        [MenuItem("TpLab/IwaSync3ColorChanger", false)]
        static void ShowWindow()
        {
            OpenSavedWindow(AssetRepository.LoadSetting<ColorChangerSetting>());
        }

        /// <summary>
        /// 拡張エディタを開く。
        /// </summary>
        /// <param name="settings">設定</param>
        static void OpenSavedWindow(ColorChangerSetting setting)
        {
            var window = GetWindow<ColorChanger>("IwaSync3 ColorChanger");
            window.setting = setting;
            window.minSize = new Vector2(400, 160);
        }

        /// <summary>
        /// GUIを表示する。
        /// </summary>
        void OnGUI()
        {
            setting.target = EditorGUILayout.ObjectField("対象のiwaSync3", setting.target, typeof(IwaSync3Base), true) as IwaSync3Base;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("変更後の色", EditorStyles.boldLabel);
            setting.mainColor = EditorGUILayout.ColorField("メインカラー", setting.mainColor);
            setting.backgroundColor = EditorGUILayout.ColorField("バックグラウンドカラー", setting.backgroundColor);
            setting.borderColor = EditorGUILayout.ColorField("ボーダーカラー", setting.borderColor);
            EditorGUILayout.Space(4);
            using (new EditorGUI.DisabledGroupScope(setting.target == null))
            {
                if (GUILayout.Button("Apply"))
                {
                    ChangeColor(setting.target, DefaultColor.Main, setting.mainColor);
                    ChangeColor(setting.target, DefaultColor.Background, setting.backgroundColor);
                    ChangeColor(setting.target, DefaultColor.Border, setting.borderColor);
                    Save();
                }
                if (GUILayout.Button("Revert"))
                {
                    ChangeColor(setting.target, DefaultColor.Main, DefaultColor.Main);
                    ChangeColor(setting.target, DefaultColor.Background, DefaultColor.Background);
                    ChangeColor(setting.target, DefaultColor.Border, DefaultColor.Border);
                    Save();
                }
            }
        }

        /// <summary>
        /// iwaSync3のカラーを変更する。
        /// </summary>
        /// <param name="target">対象のiwaSync3</param>
        /// <param name="changedColor">変更後のカラー</param>
        /// <param name="isAccentColor">アクセントカラーかどうか</param>
        void ChangeColor(IwaSync3Base target, Color32 targetColor, Color32 changedColor)
        {
            var graphics = GetChangeTargets(target, targetColor);
            if (target.GetType() == typeof(IwaSync3))
            {
                // IwaSync3の場合は配下のサポート対象の型も追加する
                foreach (var supportedType in AssetRepository.SupportedTypes)
                {
                    var components = target.GetComponentsInChildren(supportedType, true);
                    foreach (var component in components)
                    {
                        graphics = graphics.Concat(GetChangeTargets(component as IwaSync3Base, targetColor));
                    }
                }
            }

            foreach (var graphic in graphics)
            {
                Undo.RecordObject(graphic, "Change color");
                graphic.color = changedColor;
                EditorUtility.SetDirty(graphic);
            }
        }

        /// <summary>
        /// 変更対象一覧を取得する。
        /// </summary>
        /// <param name="target">対象オブジェクト</param>
        /// <param name="targetColor">対象カラー</param>
        /// <returns>変更対象一覧</returns>
        IEnumerable<Graphic> GetChangeTargets(IwaSync3Base target, Color32 targetColor)
        {
            // PrefabからデフォルトカラーのImageとTextのパスを抽出し、対象オブジェクトから変更対象のGraphicを取得
            return AssetRepository.GetTargetPaths(target, targetColor)
                .Select(x => target.transform.Find(x)?.GetComponent<Graphic>())
                .Where(x => x);
        }

        /// <summary>
        /// ウィンドウを閉じた際に呼ばれるイベント。
        /// </summary>
        void OnDestroy()
        {
            Save();
        }

        /// <summary>
        /// 設定を保存する。
        /// </summary>
        void Save()
        {
            AssetRepository.SaveSetting(setting);
        }
    }
}
