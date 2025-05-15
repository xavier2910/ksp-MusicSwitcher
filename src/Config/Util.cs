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
                throw new ArgumentException($"{name} is not a double");
            }
        }
    }
}