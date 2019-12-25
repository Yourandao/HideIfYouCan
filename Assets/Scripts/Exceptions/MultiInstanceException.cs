using System;

using UnityEngine;

namespace Assets.Scripts.Exceptions
{
    public sealed class MultiInstanceException : Exception
    {
        public MultiInstanceException (GameObject source) : base($"{source.name}: To many instances") { }
    }
}