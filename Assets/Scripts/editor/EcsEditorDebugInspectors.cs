using Leopotam.EcsProto.Unity.Editor;
using Leopotam.Types;
using td.features._common.components;
using td.features.inputEvents;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace td.editor
{
    sealed class RefManyInputEventsHandlerInspector : ProtoComponentInspector<RefMany<IInputEventsHandler>> {
        protected override bool OnRender(string label, ref RefMany<IInputEventsHandler> value)
        {
            var count = value.references.Length;
            
            EditorGUILayout.LabelField ($"RefMany<IInputEventsHandler>", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"referenses [{count}]", "IInputEventsHandler[]");
            
            EditorGUI.indentLevel++;
            var index = 0;
            foreach (var reference in value.references)
            {
                if (reference != null) EditorGUILayout.ObjectField(index.ToString(), (UnityEngine.Object)reference, typeof(IInputEventsHandler), true);
                index++;
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.IntField("count", value.count);
            return false;
        }
    }

    sealed class UintInspector : ProtoComponentInspector<uint>
    {
        protected override bool OnRender(string label, ref uint value)
        {
            var newValue = (uint)EditorGUILayout.LongField(label, value);
            if (newValue == value) { return false; }
            value = newValue;
            return true;
        }
    }

    sealed class UshortInspector : ProtoComponentInspector<ushort>
    {
        protected override bool OnRender(string label, ref ushort value)
        {
            var newValue = (ushort)EditorGUILayout.IntField(label, (int)value);
            if (newValue == value) { return false; }
            value = newValue;
            return true;
        }
    }
    
    sealed class ByteInspector : ProtoComponentInspector<byte>
    {
        protected override bool OnRender(string label, ref byte value)
        {
            var newValue = (byte)EditorGUILayout.IntField(label, value);
            if (newValue == value) { return false; }
            value = newValue;
            return true;
        }
    }    
    
    // sealed class Int2CustomInspector : ProtoComponentInspector<int2>
    // {
    //     protected override bool OnRender(string label, ref Int2 value)
    //     {
    //         var newValue = EditorGUILayout.Vector2IntField(label, new Vector2Int(value.x, value.y));
    //         if (newValue.x == value.x && newValue.y == value.y) { return false; }
    //         value.x = newValue.x;
    //         value.y = newValue.y;
    //         return true;
    //     }
    // }
    
    sealed class Int2Inspector : ProtoComponentInspector<int2>
    {
        protected override bool OnRender(string label, ref int2 value)
        {
            var newValue = EditorGUILayout.Vector2IntField(label, new Vector2Int(value.x, value.y));
            if (newValue.x == value.x && newValue.y == value.y) { return false; }
            value.x = newValue.x;
            value.y = newValue.y;
            return true;
        }
    }
    
    sealed class Int3Inspector : ProtoComponentInspector<int3>
    {
        protected override bool OnRender(string label, ref int3 value)
        {
            var newValue = EditorGUILayout.Vector3IntField(label, new Vector3Int(value.x, value.y, value.z));
            if (newValue.x == value.x && newValue.y == value.y && newValue.z == value.z) { return false; }
            value.x = newValue.x;
            value.y = newValue.y;
            value.z = newValue.z;
            return true;
        }
    }    
    
    sealed class Vec2iInspector : ProtoComponentInspector<Vec2i>
    {
        protected override bool OnRender(string label, ref Vec2i value)
        {
            var newValue = EditorGUILayout.Vector2IntField(label, new Vector2Int(value.X, value.Y));
            if (newValue.x == value.X && newValue.y == value.Y) { return false; }
            value.X = newValue.x;
            value.Y = newValue.y;
            return true;
        }
    }
    
    sealed class Vec3iInspector : ProtoComponentInspector<Vec3i>
    {
        protected override bool OnRender(string label, ref Vec3i value)
        {
            var newValue = EditorGUILayout.Vector3IntField(label, new Vector3Int(value.X, value.Y, value.Z));
            if (newValue.x == value.X && newValue.y == value.Y && newValue.z == value.Z) { return false; }
            value.X = newValue.x;
            value.Y = newValue.y;
            value.Z = newValue.z;
            return true;
        }
    }    
    
    sealed class Float2Inspector : ProtoComponentInspector<float2>
    {
        protected override bool OnRender(string label, ref float2 value)
        {
            var newValue = EditorGUILayout.Vector2Field(label, new Vector2(value.x, value.y));
            if (Mathf.Approximately(newValue.x, value.x) && Mathf.Approximately(newValue.y , value.y)) { return false; }
            value.x = newValue.x;
            value.y = newValue.y;
            return true;
        }
    }    
    sealed class Vec2fInspector : ProtoComponentInspector<Vec2f>
    {
        protected override bool OnRender(string label, ref Vec2f value)
        {
            var newValue = EditorGUILayout.Vector2Field(label, new Vector2(value.X, value.Y));
            if (Mathf.Approximately(newValue.x, value.X) && Mathf.Approximately(newValue.y , value.Y)) { return false; }
            value.X = newValue.x;
            value.Y = newValue.y;
            return true;
        }
    }
    
    sealed class Float3Inspector : ProtoComponentInspector<float3>
    {
        protected override bool OnRender(string label, ref float3 value)
        {
            var newValue = EditorGUILayout.Vector3Field(label, new Vector3(value.x, value.y, value.z));
            if (Mathf.Approximately(newValue.x, value.x) && Mathf.Approximately(newValue.y , value.y) && Mathf.Approximately(newValue.z , value.z)) { return false; }
            value.x = newValue.x;
            value.y = newValue.y;
            value.z = newValue.z;
            return true;
        }
    }

    sealed class Vec3fInspector : ProtoComponentInspector<Vec3f>
    {
        protected override bool OnRender(string label, ref Vec3f value)
        {
            var newValue = EditorGUILayout.Vector3Field(label, new Vector3(value.X, value.Y, value.Z));
            if (Mathf.Approximately(newValue.x, value.X) && Mathf.Approximately(newValue.y , value.Y) && Mathf.Approximately(newValue.z , value.Z)) { return false; }
            value.X = newValue.x;
            value.Y = newValue.y;
            value.Z = newValue.z;
            return true;
        }
    }
    
    sealed class QuatInspector : ProtoComponentInspector<Quat>
    {
        protected override bool OnRender(string label, ref Quat value)
        {
            var euler = value.ToEuler();
            var newValue = EditorGUILayout.Vector3Field(label, new Vector3(euler.X, euler.Y, euler.Z));
            if (Mathf.Approximately(newValue.x, euler.X) && Mathf.Approximately(newValue.y , euler.Y) && Mathf.Approximately(newValue.z , euler.Z)) { return false; }
            value = Quat.Euler(newValue.x, newValue.y, newValue.z);
            return true;
        }
    }
}
