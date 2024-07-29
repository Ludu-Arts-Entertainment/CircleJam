using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Automation
{
    public abstract class BuildPostProcess : ScriptableObject
    {
        [SerializeField] private bool _enabled = true;
        public bool Enabled => _enabled;
        public abstract void Execute(BuildTarget buildTarget, string buildPath);
    }
}
