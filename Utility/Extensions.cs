using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AndrewDowsett.Utility
{
    public enum EAxis
    {
        X,
        Y,
        Z,
        W,
        XY,
        XZ,
        XW,
        YZ,
        YW,
        ZW,
        XYZ,
        XYW,
        XZW,
        YZW
    }

    public static class Extensions
    {
        public static List<T> ToSortedList<T>(this HashSet<T> set)
        {
            T[] array = new T[set.Count];
            set.CopyTo(array);

            List<T> list = new List<T>(array);
            list.Sort();
            return list;
        }

        public static List<T> ToSortedList<T>(this HashSet<T> set, IComparer<T> comparer)
        {
            T[] array = new T[set.Count];
            set.CopyTo(array);

            List<T> list = new List<T>(array);
            list.Sort(comparer);
            return list;
        }

        public static List<T> ToSortedList<T>(this HashSet<T> set, Comparison<T> comparison)
        {
            T[] array = new T[set.Count];
            set.CopyTo(array);

            List<T> list = new List<T>(array);
            list.Sort(comparison);
            return list;
        }

        public static List<T> ToSortedList<T>(this HashSet<T> set, int index, int count, IComparer<T> comparer)
        {
            T[] array = new T[set.Count];
            set.CopyTo(array);

            List<T> list = new List<T>(array);
            list.Sort(index, count, comparer);
            return list;
        }

        public static bool Contains(this string source, string target, StringComparison comparison)
        {
            int index = source.IndexOf(target, comparison);
            return index >= 0;
        }

        private static T DrawRandom<T>(this List<T> list)
        {
            if (list.Count == 0)
                return default(T);

            int index = UnityEngine.Random.Range(0, list.Count);
            var result = list[index];
            list.RemoveAt(index);
            return result;
        }
        public static List<T> DrawRandom<T>(this List<T> list, int count = 1)
        {
            int resultCount = Mathf.Min(count, list.Count);
            List<T> result = new List<T>(resultCount);
            for (int i = 0; i < resultCount; ++i)
            {
                T item = list.DrawRandom();
                result.Add(item);
            }
            return result;
        }

        public static void DialogueSetup(this Button button, string text, Action action, object[] args = null)
        {
            button.gameObject.transform.GetComponentInChildren<TMP_Text>().text = text;
            button.gameObject.SetActive(true);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => action());
        }

        public static string GetSha256(this string str)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(str));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static Vector2 XY(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static Vector2 XZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector2 YZ(this Vector3 v)
        {
            return new Vector2(v.y, v.z);
        }

        public static void CopyToClipboard(this string str)
        {
            GUIUtility.systemCopyBuffer = str;
        }

        public static void ExecuteNextFrame(this MonoBehaviour behaviour, Action action)
        {
            behaviour.StartCoroutine(NextFrame(action));
        }
        private static IEnumerator NextFrame(Action action)
        {
            yield return 0;
            action();
        }

        /// <summary>
        /// Remaps a value from one range to another.
        /// </summary>
        /// <param name="s">The variable to be remapped.</param>
        /// <param name="a1">The minimum value of the original range.</param>
        /// <param name="a2">The maximum value of the original range.</param>
        /// <param name="b1">The minimum value of the new range.</param>
        /// <param name="b2">The maximum value of the new range.</param>
        /// <returns>The remapped value.</returns>
        public static float Remap(this float s, float a1, float a2, float b1, float b2)
        {
            return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        }

        /// <summary>
        /// Remaps a value from one range to another, clamped between the new range.
        /// </summary>
        /// <param name="s">The variable to be remapped.</param>
        /// <param name="a1">The minimum value of the original range.</param>
        /// <param name="a2">The maximum value of the original range.</param>
        /// <param name="b1">The minimum value of the new range.</param>
        /// <param name="b2">The maximum value of the new range.</param>
        /// <returns>The remapped value.</returns>
        public static float RemapClamped(this float s, float a1, float a2, float b1, float b2)
        {
            return Mathf.Clamp(Remap(s, a1, a2, b1, b2), b1, b2);
        }

        public static bool AddUnique<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
                return true;
            }
            return false;
        }

        public static bool RemoveUnique<T>(this List<T> list, T item)
        {
            if (list.Contains(item))
            {
                list.Remove(item);
                return true;
            }
            return false;
        }

        public static Transform RecursiveFindChild(this Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name == childName)
                {
                    return child;
                }
                else
                {
                    Transform found = RecursiveFindChild(child, childName);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }
            return null;
        }

        public static int FindFirstIndex<TValue>(this IReadOnlyList<TValue> source, TValue toFind)
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i].Equals(toFind))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int FindFirstIndex<T, TValue>(this IReadOnlyDictionary<T, TValue> source, TValue toFind)
        {
            int index = 0;
            foreach (var kvp in source)
            {
                if (kvp.Value.Equals(toFind))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static byte[] ToByteArray(this float[] array)
        {
            byte[] byteArray = new byte[sizeof(float) * array.Length];
            Buffer.BlockCopy(array, 0, byteArray, 0, byteArray.Length);
            return byteArray;
        }
        public static float[] ToFloatArray(this byte[] array)
        {
            float[] floatArray = new float[array.Length / sizeof(float)];
            Buffer.BlockCopy(array, 0, floatArray, 0, array.Length);
            return floatArray;
        }

        public static void RemoveCloneInName(this string input)
        {
            input = input.Replace("(Clone)", "");
        }

        public static string SanitizeAuthenticationString(this string input)
        {
            string output = input;
            output = output.Replace("<", "〈");
            output = output.Replace(">", "〉");
            if (output.Contains('#'))
                output = output[..output.IndexOf('#')];

            return output;
        }

        public static T RandomElement<T>(this T[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        public static Texture2D TextureFromGradient(Gradient g, int resolution)
        {
            Texture2D t = new Texture2D(resolution, 1);
            t.filterMode = FilterMode.Point;
            Color[] colors = new Color[resolution];
            for (int i = 0; i < resolution; ++i)
            {
                colors[i] = g.Evaluate((float)i / resolution);
            }
            t.SetPixels(colors);
            t.Apply();
            return t;
        }

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

        public static bool IsValidIndex(this Array array, int index)
        {
            return array.Length > index && index >= 0;
        }

        public static bool IsValidIndex<T>(this List<T> array, int index)
        {
            return array.Count > index && index >= 0;
        }

        public static Vector2 IgnoreAxis(this Vector2 vector, EAxis ignoredAxis, float ignoredAxisValue = 0)
        {
            switch (ignoredAxis)
            {
                case EAxis.X:
                    return new Vector2(ignoredAxisValue, vector.y);
                case EAxis.Y:
                    return new Vector2(vector.x, ignoredAxisValue);
            }
            Debug.Log("Unsupported ignored axis given");
            return vector;
        }

        public static Vector3 IgnoreAxis(this Vector3 vector, EAxis ignoredAxis, float ignoredAxisValue = 0)
        {
            switch (ignoredAxis)
            {
                case EAxis.X:
                    return new Vector3(ignoredAxisValue, vector.y, vector.z);
                case EAxis.Y:
                    return new Vector3(vector.x, ignoredAxisValue, vector.z);
                case EAxis.Z:
                    return new Vector3(vector.x, vector.y, ignoredAxisValue);
                case EAxis.XY:
                    return new Vector3(ignoredAxisValue, ignoredAxisValue, vector.z);
                case EAxis.XZ:
                    return new Vector3(ignoredAxisValue, vector.y, ignoredAxisValue);
                case EAxis.YZ:
                    return new Vector3(vector.x, ignoredAxisValue, ignoredAxisValue);
            }
            Debug.Log("Unsupported ignored axis given");
            return vector;
        }

        public static Vector4 IgnoreAxis(this Vector4 vector, EAxis ignoredAxis, float ignoredAxisValue = 0)
        {
            switch (ignoredAxis)
            {
                case EAxis.X:
                    return new Vector4(ignoredAxisValue, vector.y, vector.z, vector.w);
                case EAxis.Y:
                    return new Vector4(vector.x, ignoredAxisValue, vector.z, vector.w);
                case EAxis.Z:
                    return new Vector4(vector.x, vector.y, ignoredAxisValue, vector.w);
                case EAxis.W:
                    return new Vector4(vector.x, vector.y, vector.z, ignoredAxisValue);
                case EAxis.XY:
                    return new Vector4(ignoredAxisValue, ignoredAxisValue, vector.z, vector.w);
                case EAxis.XZ:
                    return new Vector4(ignoredAxisValue, vector.y, ignoredAxisValue, vector.w);
                case EAxis.XW:
                    return new Vector4(ignoredAxisValue, vector.y, vector.z, ignoredAxisValue);
                case EAxis.YZ:
                    return new Vector4(vector.x, ignoredAxisValue, ignoredAxisValue, vector.w);
                case EAxis.YW:
                    return new Vector4(vector.x, ignoredAxisValue, vector.z, ignoredAxisValue);
                case EAxis.ZW:
                    return new Vector4(vector.x, vector.y, ignoredAxisValue, ignoredAxisValue);
                case EAxis.XYZ:
                    return new Vector4(ignoredAxisValue, ignoredAxisValue, ignoredAxisValue, vector.w);
                case EAxis.XYW:
                    return new Vector4(ignoredAxisValue, ignoredAxisValue, vector.z, ignoredAxisValue);
                case EAxis.XZW:
                    return new Vector4(ignoredAxisValue, vector.y, ignoredAxisValue, ignoredAxisValue);
                case EAxis.YZW:
                    return new Vector4(vector.x, ignoredAxisValue, ignoredAxisValue, ignoredAxisValue);
            }
            Debug.Log("Unsupported ignored axis given");
            return vector;
        }

        public static void SaveAsPNG(this Texture2D texture, string fullPath)
        {
            byte[] bytes = texture.EncodeToPNG();
            System.IO.File.WriteAllBytes(fullPath, bytes);
        }

        public static Vector3 RandomUnitVectorInCone(this Quaternion targetDirection, float angle)
        {
            var angleInRad = UnityEngine.Random.Range(0.0f, angle) * Mathf.Deg2Rad;
            var PointOnCircle = (UnityEngine.Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
            var V = new Vector3(PointOnCircle.x, PointOnCircle.y, Mathf.Cos(angleInRad));
            return targetDirection * V;
        }

        public static Vector3 RandomUnitVectorInCone(this Vector3 targetDirection, float angle)
        {
            return RandomUnitVectorInCone(Quaternion.LookRotation(targetDirection), angle);
        }

        public static void AttachTo(this SkinnedMeshRenderer smr, SkinnedMeshRenderer parent)
        {
            Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
            foreach (Transform bone in parent.bones)
                boneMap[bone.gameObject.name] = bone;


            SkinnedMeshRenderer myRenderer = smr;

            Transform[] newBones = new Transform[myRenderer.bones.Length];
            for (int i = 0; i < myRenderer.bones.Length; ++i)
            {
                GameObject bone = myRenderer.bones[i].gameObject;
                if (!boneMap.TryGetValue(bone.name, out newBones[i]))
                {
                    Debug.Log("Unable to map bone \"" + bone.name + "\" to target skeleton.");
                    continue;
                }
            }
            myRenderer.bones = newBones;

            smr.rootBone = parent.rootBone;
            smr.transform.parent = parent.transform.parent;
        }

        public static string SanitizeAndFormatTags(this string text)
        {
            return text.SanitizeText().FormatTags();
        }

        public static string SanitizeText(this string text)
        {
            string oldText;
            do
            {
                oldText = text;
                text = Regex.Replace(text, @"<\/noparse>", "", RegexOptions.IgnoreCase);
            } while (oldText != text);

            return text;
        }

        public static string SanitizeTextNoTMP(this string text)
        {
            string oldText;
            do
            {
                oldText = text;
                text = Regex.Replace(text, @"<\/noparse>", "", RegexOptions.IgnoreCase);
                text = Regex.Replace(text, @"<noparse>", "", RegexOptions.IgnoreCase);
            } while (oldText != text);

            return text;
        }

        public static string FormatTags(this string text)
        {
            int characterIndex = -1;
            while (characterIndex < text.Length)
            {
                characterIndex++;
                characterIndex = text.IndexOf("$", characterIndex);
                if (characterIndex < 0) break;
                if (characterIndex < text.Length - 3) // Text has at least 3 more characters after this '$'
                {
                    string str = text.Substring(characterIndex + 1, 3);
                    if (str.OnlyHexInString())
                    {
                        text = text.ReplaceSectionWithFormat(characterIndex, 4, $"</color><color={GetHexColorStringFrom3Chars(str)}>");
                    }
                }
                if (characterIndex < text.Length - 2) // Text has at least 2 more characters after this '$'
                {
                    if (text[characterIndex + 1] == 'l')
                    {
                        int linkEndChar = text.IndexOf(' ', characterIndex);
                        if (linkEndChar == -1)
                            linkEndChar = text.Length;
                        string link = text.Substring(characterIndex + 2, linkEndChar - characterIndex - 2);
                        text = text.ReplaceSectionWithFormat(characterIndex, linkEndChar - characterIndex, $"<color=#0645AD><u><link=\"{link}\">{link}</link></u></color>");
                    }
                }
            }
            return text;
        }

        public static bool OnlyHexInString(this string text)
        {
            return Regex.IsMatch(text, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        public static string ReplaceSectionWithFormat(this string text, int startIndex, int length, string newSection)
        {
            string beforeText = text.Substring(0, startIndex);
            string afterText = text.Substring(startIndex + length);
            return beforeText + "</noparse>" + newSection + "<noparse>" + afterText;
        }

        public static string GetHexColorStringFrom3Chars(this string text)
        {
            if (text.Length == 3)
            {
                string r = text[0].ToString().ToUpper() + text[0].ToString().ToUpper();
                string g = text[1].ToString().ToUpper() + text[1].ToString().ToUpper();
                string b = text[2].ToString().ToUpper() + text[2].ToString().ToUpper();
                return "#" + r + g + b;
            }
            return "#000000";
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.TryGetComponent(out T component) ? component : gameObject.AddComponent<T>();
        }

        public static string Nicify(this string s)
        {
            string result = "";
            if (s.Length > 1)
                s = char.ToUpper(s[0]) + s.Substring(1);
            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsUpper(s[i]) == true && i != 0)
                {
                    result += " ";
                }

                result += s[i];
            }
            return result;
        }

        public static void Materialify(this GameObject go, Material mat)
        {
            go.SetActive(true);
            MeshRenderer[] meshRenderers = go.GetComponents<MeshRenderer>();
            MeshRenderer[] childMeshRenderers = go.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var item in meshRenderers)
            {
                item.sharedMaterial = mat;
            }
            foreach (var item in childMeshRenderers)
            {
                item.sharedMaterial = mat;
            }

            SkinnedMeshRenderer[] skinnedMeshRenderers = go.GetComponents<SkinnedMeshRenderer>();
            SkinnedMeshRenderer[] skinnedChildMeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var item in skinnedMeshRenderers)
            {
                item.sharedMaterial = mat;
            }
            foreach (var item in skinnedChildMeshRenderers)
            {
                item.sharedMaterial = mat;
            }
        }

        public static void AddOrReplace<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, T2 value)
        {
            if (dictionary.ContainsKey(key))
                dictionary.Remove(key);
            dictionary.Add(key, value);
        }

        public static float DistanceSquared(this Vector3 vectorA, Vector3 vectorB) => (vectorB - vectorA).sqrMagnitude;

        // Remove unwanted decimals from a float, return default if failed
        public static float ToDecimals(this float value, int decimals) => (float)Math.Round(value, decimals);

        // Parse a float, return default if failed
        public static float ParseToFloat(this string toParse, int decimals, float _default) => (float.TryParse(toParse, out float f) ? f : _default).ToDecimals(decimals);

        // Parse a int, return default if failed
        public static int ParseToInt(this string toParse, int _default) => int.TryParse(toParse, out int f) ? f : _default;
    }
}