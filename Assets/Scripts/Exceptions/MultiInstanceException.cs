using System;

using UnityEngine;

namespace Scripts.Exceptions
{
    public sealed class MultiInstanceException : Exception
    {
        public MultiInstanceException (GameObject source) : base($"{source.name}: To many instances") { }
    }
}