using System;
using System.Collections.Generic;
using UnityEngine;
using Log = KSPBuildTools.Log;

namespace MusicSwitcher.Config {

    internal sealed class Util {

        public static double Double(string name, ConfigNode node) {
            if (!node.HasValue(name)) {
                throw new ArgumentException($"{node} has no field {name}");
            }

            try {
                return double.Parse(node.GetValue(name));
            } catch (Exception) {
                throw new ArgumentException($"{name} is not a double");
            }
        }

        public static float Float(string name, ConfigNode node) {
            if (!node.HasValue(name)) {
                throw new ArgumentException($"{node} has no field {name}");
            }

            try {
                return float.Parse(node.GetValue(name));
            } catch (Exception) {
                throw new ArgumentException($"{name} is not a float");
            }
        }

        public static float FloatOrDefault(string name, ConfigNode node, float def, string logTag) {
            if (!node.HasValue(name)) {
                return def;
            }

            try {
                return float.Parse(node.GetValue(name));
            } catch (Exception) {
                Log.Error($"{name} is not a float", logTag);
                return def;
            }
        }

        private static readonly string kDebugName = "debugName";
        private static readonly string kInvalid = "!INVALID!";
        public static string DebugName(ConfigNode node) {
            if (!node.HasValue(kDebugName)) {
                return kInvalid;
            }

            return node.GetValue(kDebugName);
        }
    }
}