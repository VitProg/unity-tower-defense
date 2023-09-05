#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using td.features._common;
using td.features.shard.components;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace td.features.shard.mb
{
    public class UI_Shard_Button : Button, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        #region Events
        [FormerlySerializedAs("onPointerClicked")][SerializeField]private UnityEvent<Vector2> m_onPointerClicked = new ();
        [FormerlySerializedAs("onPointerEntered")][SerializeField]private UnityEvent<Vector2> m_onPointerEntered = new ();
        [FormerlySerializedAs("onPointerExited")][SerializeField]private UnityEvent<Vector2> m_onPointerExited = new ();
        [FormerlySerializedAs("onDragStart")][SerializeField]private UnityEvent<Vector2> m_onDragStart = new ();
        [FormerlySerializedAs("onDragMove")][SerializeField]private UnityEvent<Vector2> m_onDragMove = new ();
        [FormerlySerializedAs("onDragFinish")][SerializeField]private UnityEvent<Vector2> m_onDragFinish = new ();

        public UnityEvent<Vector2> OnPointerClicked { get => m_onPointerClicked; set => m_onPointerClicked = value; }
        public UnityEvent<Vector2> OnPointerEntered { get => m_onPointerEntered; set => m_onPointerEntered = value; }
        public UnityEvent<Vector2> OnPointerExited { get => m_onPointerExited; set => m_onPointerExited = value; }
        public UnityEvent<Vector2> OnDragStart { get => m_onDragStart; set => m_onDragStart = value; }
        public UnityEvent<Vector2> OnDragMove { get => m_onDragMove; set => m_onDragMove = value; }
        public UnityEvent<Vector2> OnDragFinish { get => m_onDragFinish; set => m_onDragFinish = value; }
        #endregion

        public Image plus;
        public TMP_Text priceText;
        public UI_Shard uiShard;
        
        public bool showPlus = false;
        public bool hasShard = false;
        public uint price = 0;
        public bool hidden = false;
        public bool canDrag;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_onPointerClicked.RemoveAllListeners();
            m_onPointerEntered.RemoveAllListeners();
            m_onPointerExited.RemoveAllListeners();
            m_onDragStart.RemoveAllListeners();
            m_onDragMove.RemoveAllListeners();
            m_onDragFinish.RemoveAllListeners();
        }

        // [Button("Refresh")]
        public void Refresh()
        {
            uiShard.gameObject.SetActive(!hidden && hasShard);
            plus.gameObject.SetActive(!hidden && !hasShard && showPlus);
            priceText.gameObject.SetActive(!hidden && hasShard && price > 0);

            if (hasShard)
            {
                uiShard.FullRefresh();
                priceText.text = CommonUtils.PriceFormat(price);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetHidden(bool h)
        {
            hidden = h;
            Refresh();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearShard()
        {
            hasShard = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetShard(ref Shard shard)
        {
            uiShard.shard = shard;
            hasShard = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Shard GetShard() => ref uiShard.shard;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("OnPointerEnter");
            
            base.OnPointerEnter(eventData);
            uiShard.SetHover(true);
            m_onPointerEntered.Invoke(eventData.position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            uiShard.SetHover(false);
            m_onPointerExited.Invoke(eventData.position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            m_onPointerClicked.Invoke(eventData.position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!canDrag) return;
            m_onDragStart.Invoke(eventData.position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!canDrag) return;
            m_onDragFinish.Invoke(eventData.position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnDrag(PointerEventData eventData)
        {
            if (!canDrag) return;
            m_onDragMove.Invoke(eventData.position);
        }

        public bool IsHovered
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => uiShard.IsHovered;
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(UI_Shard_Button))] // If I comment this out the public variablbes show
// If it is uncommented, then just the button with "Generate" text shows
    public class ShardUIButtonEditor : ButtonEditor
    {
        private List<SerializedProperty> props;
        protected UI_Shard_Button UIShardButton;

        protected override void OnEnable()
        {
            base.OnEnable();
            UIShardButton = (UI_Shard_Button)target;
            props = new List<SerializedProperty>
            {
                serializedObject.FindProperty("m_onPointerClicked"),
                serializedObject.FindProperty("m_onPointerEntered"),
                serializedObject.FindProperty("m_onPointerExited"),
                serializedObject.FindProperty("m_onDragStart"),
                serializedObject.FindProperty("m_onDragMove"),
                serializedObject.FindProperty("m_onDragFinish"),
                serializedObject.FindProperty("plus"),
                serializedObject.FindProperty("priceText"),
                serializedObject.FindProperty("showPlus"),
                serializedObject.FindProperty("hasShard"),
                serializedObject.FindProperty("price"),
                serializedObject.FindProperty("hidden"),
                serializedObject.FindProperty("uiShard"),
                serializedObject.FindProperty("canDrag"),
            };
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            if (props == null || props.Count == 0 || props[0] == null)
            {
                props = new List<SerializedProperty>
                {
                    serializedObject.FindProperty("m_onPointerClicked"),
                    serializedObject.FindProperty("m_onPointerEntered"),
                    serializedObject.FindProperty("m_onPointerExited"),
                    serializedObject.FindProperty("m_onDragStart"),
                    serializedObject.FindProperty("m_onDragMove"),
                    serializedObject.FindProperty("m_onDragFinish"),
                    serializedObject.FindProperty("plus"),
                    serializedObject.FindProperty("priceText"),
                    serializedObject.FindProperty("showPlus"),
                    serializedObject.FindProperty("hasShard"),
                    serializedObject.FindProperty("price"),
                    serializedObject.FindProperty("hidden"),
                    serializedObject.FindProperty("uiShard"),
                    serializedObject.FindProperty("canDrag"),
                };
            }
            
            serializedObject.Update();
            foreach (var prop in props)
            {
                if (prop == null) continue;
                EditorGUILayout.PropertyField(prop);
                prop.serializedObject.ApplyModifiedProperties();
            }
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Refresh"))
            {
                UIShardButton.Refresh();
            }
        }
    }
#endif
}