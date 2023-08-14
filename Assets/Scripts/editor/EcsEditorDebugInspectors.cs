using Leopotam.EcsProto.Unity.Editor;
using td.common;
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
    
    sealed class Int2CustomInspector : ProtoComponentInspector<Int2>
    {
        protected override bool OnRender(string label, ref Int2 value)
        {
            var newValue = EditorGUILayout.Vector2IntField(label, new Vector2Int(value.x, value.y));
            if (newValue.x == value.x && newValue.y == value.y) { return false; }
            value.x = newValue.x;
            value.y = newValue.y;
            return true;
        }
    }
    
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
}
