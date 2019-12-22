using System;

using UnityEngine;

namespace Assets.Scripts
{
    class MultiInstanceException : Exception
    {
        public MultiInstanceException (GameObject source) : base($"{source.name}: To many instances") { }
    }
}