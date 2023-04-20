using System;
using System.IO;
using System.Linq;
using System.Text;
using td.common;
using UnityEditor;
using UnityEngine;

namespace td.editor
{
#if UNITY_EDITOR
    public class ComponentProviderGenerator : ScriptableObject
    {
        private const string ComponentProviderTemplate = "ComponentProvider.cs.txt";
        
        [MenuItem("TD/Generate Providers", false, -200)]
        public static void Generate()
        {
            Debug.Log("> Providers Generation started...");
            
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .Where(t => Attribute.IsDefined(t, typeof(GenerateProviderAttribute)) && t.IsValueType && !t.IsAbstract);

            if (!AssetDatabase.IsValidFolder("Assets/Scripts/__generated"))
            {
                AssetDatabase.CreateFolder("Assets/Scripts", "__generated");
            }

            if (!AssetDatabase.IsValidFolder("Assets/Scripts/__generated/ComponentProviders"))
            {
                AssetDatabase.CreateFolder("Assets/Scripts/__generated", "ComponentProviders");
            }

            var folderPath = "Assets/Scripts/__generated/ComponentProviders";
            
            foreach (var type in types)
            {
                var attribute = Attribute.GetCustomAttribute(type, typeof(GenerateProviderAttribute));
                if (attribute == null)
                {
                    Debug.LogWarning($"Can't get attribute from type: {type.Name}");
                    continue;
                }
                
                Debug.Log($"> > generate Provider for {type.FullName}...");
                
                // = GetFolder(GeneratedComponentProvidersPath);
                
                CreateTemplate(
                    GetTemplateContent(ComponentProviderTemplate), 
                    $"{folderPath}/{type.Name}Provider.cs",
                    $"{folderPath}/{type.Name}.cs",
                    type.Namespace
                );
            }
            AssetDatabase.Refresh();
        }

        private static string GetFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var guid = AssetDatabase.CreateFolder(Path.GetDirectoryName(path), Path.GetFileName(path));
                return AssetDatabase.GUIDToAssetPath(guid);
            }

            return path;
        }

        private static string GetTemplateContent(string proto)
        {
            // hack: its only one way to get current editor script path. :(
            var pathHelper = CreateInstance<ComponentProviderGenerator>();
            var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(pathHelper)));
            DestroyImmediate(pathHelper);
            try
            {
                return File.ReadAllText(Path.Combine(path ?? "", proto));
            }
            catch
            {
                return null;
            }
        }
        
        private static string CreateTemplate(string proto, string fileName, string scriptName, string nameSpace)
        {
            if (string.IsNullOrEmpty(scriptName))
            {
                return "Invalid fileName";
            }
            
            if (string.IsNullOrEmpty(nameSpace))
            {
                return "Invalid nameSpace";
            }
            
            proto = proto.Replace("#NS#", nameSpace);
            proto = proto.Replace("#SCRIPTNAME#", SanitizeClassName(Path.GetFileNameWithoutExtension(scriptName)));
            try
            {
                if (File.Exists(fileName))
                {
                    File.WriteAllText(fileName, proto);
                }
                else
                {
                    var path = AssetDatabase.GenerateUniqueAssetPath(fileName);
                    File.WriteAllText(path, proto);
                }
                
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
            return null;
        }
        
        private static string SanitizeClassName(string className)
        {
            var sb = new StringBuilder();
            var needUp = true;
            foreach (var c in className)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(needUp ? char.ToUpperInvariant(c) : c);
                    needUp = false;
                }
                else
                {
                    needUp = true;
                }
            }

            return sb.ToString();
        }
    }
#endif
}