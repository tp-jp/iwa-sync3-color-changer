using HoshinoLabs.IwaSync3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VRC.Core;

namespace TpLab.IwaSync3ColorChanger.Editor
{
    public static class AssetRepository
    {
        /// <summary>
        /// 設定ファイルのPackageパス
        /// </summary>
        const string PackagePath = "Packages/com.tp.jp.iwa-sync3-color-changer/Runtime/Setting.asset";

        /// <summary>
        /// 設定ファイルのパス
        /// </summary>
        const string AssetPath = "Assets/TpLab/IwaSync3ColorChanger/Setting.asset";

        /// <summary>
        /// サポート対象のタイプ一覧
        /// </summary>
        public static readonly Type[] SupportedTypes =
        {
            typeof(DesktopBar),
            typeof(ListTab),
            typeof(Playlist),
            typeof(Queuelist),
            // typeof(HoshinoLabs.IwaSync3.Screen),
            // typeof(Speaker),
        };

        /// <summary>
        /// タイプに対応するパスを紐づける辞書。
        /// </summary>
        static readonly Dictionary<Type, string> typeMap = new Dictionary<Type, string>();

        /// <summary>
        /// 静的コンストラクタ。
        /// </summary>
        static AssetRepository()
        {
            typeMap[typeof(IwaSync3)] = "Assets/HoshinoLabs/iwaSync3/iwaSync3.prefab";
            typeMap[typeof(DesktopBar)] = "Assets/HoshinoLabs/iwaSync3/Prefabs/iwaSync3-DesktopBar.prefab";
            typeMap[typeof(ListTab)] = "Assets/HoshinoLabs/iwaSync3/Prefabs/iwaSync3-ListTab.prefab";
            typeMap[typeof(Playlist)] = "Assets/HoshinoLabs/iwaSync3/Prefabs/iwaSync3-Playlist.prefab";
            typeMap[typeof(Queuelist)] = "Assets/HoshinoLabs/iwaSync3/Prefabs/iwaSync3-Queuelist.prefab";
        }

        /// <summary>
        /// 設定ファイルを読み込む。存在しない場合は設定クラスを新規作成する。
        /// </summary>
        /// <returns>設定</returns>
        public static T LoadSetting<T>() where T : ScriptableObject, new()
        {
            return AssetDatabase.LoadAssetAtPath<T>(AssetPath)
                   ?? AssetDatabase.LoadAssetAtPath<T>(PackagePath)
                   ?? new T();
        }

        /// <summary>
        /// 設定ファイルを読み込む。
        /// </summary>
        /// <param name="setting">設定</param>
        public static void SaveSetting<T>(T setting) where T : ScriptableObject
        {
            var dir = Path.GetDirectoryName(AssetPath);
            if (!AssetDatabase.IsValidFolder(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!AssetDatabase.Contains(setting))
            {
                AssetDatabase.CreateAsset(setting, AssetPath);
            }
            EditorUtility.SetDirty(setting);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 指定された型のPrefabsアセットを読み込む。
        /// </summary>
        /// <returns>IwaSync3Base</returns>
        public static IwaSync3Base LoadAsset(Type type)
        {
            if (!typeMap.ContainsKey(type)) return null;
            return AssetDatabase.LoadAssetAtPath(typeMap[type], type) as IwaSync3Base;
        }

        /// <summary>
        /// 指定カラーを保持するComponentの相対パスを取得する。
        /// </summary>
        /// <param name="asset">iwaSync3Base</param>
        /// <param name="color">カラー</param>
        /// <returns>パス一覧</returns>
        public static string[] GetTargetPaths(IwaSync3Base target, Color32 color)
        {
            // アセット読み込み
            var asset = LoadAsset(target.GetType());
            if (!asset) return Array.Empty<string>();

            // 対象パスの取得
            var targetPaths = GetTargetPaths<Image>(asset, color)
                .Concat(GetTargetPaths<Text>(asset, color));

            return targetPaths
                .Concat(GetAppendTargetPaths(target, targetPaths))
                .ToArray();
        }

        /// <summary>
        /// 追加の相対パスを取得する。
        /// </summary>
        /// <param name="target">対象コンポーネント</param>
        /// <param name="paths">パス一覧</param>
        /// <returns>相対パス一覧</returns>
        public static string[] GetAppendTargetPaths(IwaSync3Base target, IEnumerable<string> paths)
        {
            var type =target.GetType();
            if (type == typeof(Playlist) || type == typeof(ListTab))
            {
                var contents = target.transform.Find($"Udon ({type.Name})/Canvas/Panel/Scroll View/Scroll View/Viewport/Content");
                var contentNames = new List<string>();
                for (var i = 0; i < contents.childCount; i++)
                {
                    if (contents.GetChild(i).name == "Template") continue;
                    contentNames.Add(contents.GetChild(i).name);
                }
                return paths
                    .Where(x => x.StartsWith($"Udon ({type.Name})/Canvas/Panel/Scroll View/Scroll View/Viewport/Content/Template"))
                    .SelectMany(x => contentNames.Select(n => x.Replace("Template", n)))
                    .ToArray();
            }
            return Array.Empty<string>();
        }

        /// <summary>
        /// 指定カラーを保持するComponentの相対パスを取得する。
        /// </summary>
        /// <param name="iwaSync3Base">iwaSync3Base</param>
        /// <param name="color">カラー</param>
        /// <returns>パス一覧</returns>
        static string[] GetTargetPaths<T>(IwaSync3Base iwaSync3Base, Color32 color) where T : Graphic
        {
            var basePath = iwaSync3Base.transform.GetHierarchyPath();
            return iwaSync3Base
                .GetComponentsInChildren<T>(true)
                .Where(x =>
                {
                    var c = (Color32)x.color;
                    return color.r == c.r && color.g == c.g && color.b == c.b;
                })
                .Select(x =>
                {
                    var path = x.transform.GetHierarchyPath();
                    return path.Substring(basePath.Length + 1);
                })
                .ToArray();
        }
    }
}
