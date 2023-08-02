using System.Collections.Generic;
using Leopotam.EcsLite;
// using NaughtyAttributes;
using td.features._common;
using td.features.shard.components;
using td.features.state;
using td.utils.di;
using TMPro;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

namespace td.features.shard.mb
{
    public class ShardUIButton : Button, IBeginDragHandler, IEndDragHandler, IDragHandler
    { 
        [FormerlySerializedAs("onPointerClicked")][SerializeField]private UnityEvent<Vector2> m_onPointerClicked = new ();
        [FormerlySerializedAs("onPointerEntered")][SerializeField]private UnityEvent<Vector2> m_onPointerEntered = new ();
        [FormerlySerializedAs("onPointerExited")][SerializeField]private UnityEvent<Vector2> m_onPointerExited = new ();
        [FormerlySerializedAs("onDragStart")][SerializeField]private UnityEvent<Vector2> m_onDragStart = new ();
        [FormerlySerializedAs("onDragMove")][SerializeField]private UnityEvent<Vector2> m_onDragMove = new ();
        [FormerlySerializedAs("onDragFinish")][SerializeField]private UnityEvent<Vector2> m_onDragFinish = new ();

        public UnityEvent<Vector2> onPointerClicked { get => m_onPointerClicked; set => m_onPointerClicked = value; }
        public UnityEvent<Vector2> onPointerEntered { get => m_onPointerEntered; set => m_onPointerEntered = value; }
        public UnityEvent<Vector2> onPointerExited { get => m_onPointerExited; set => m_onPointerExited = value; }
        public UnityEvent<Vector2> onDragStart { get => m_onDragStart; set => m_onDragStart = value; }
        public UnityEvent<Vector2> onDragMove { get => m_onDragMove; set => m_onDragMove = value; }
        public UnityEvent<Vector2> onDragFinish { get => m_onDragFinish; set => m_onDragFinish = value; }

        private readonly EcsInject<IState> state;

        public Image plus;
        public TMP_Text costText;

        // [OnValueChanged("Refresh")]
        public bool showPlus = false;
        public bool hasShard = false;
        public uint cost = 0;
        public bool hidden = false;
        public ShardConrol shardConrol;
        
        public bool canDrag;
        
        protected override void Awake()
        {
            base.Awake();
            var container = ServiceContainer.GetCurrentContainer();
            if (container != null && container.TryGet<IEcsSystems>(out var systems))
            {
                systems.ResolveMonoBehaviour(this, container);
            }
        }

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
            shardConrol.gameObject.SetActive(!hidden && hasShard);
            plus.gameObject.SetActive(!hidden && !hasShard && showPlus);
            costText.gameObject.SetActive(!hidden && hasShard && cost > 0);

            if (hasShard)
            {
                shardConrol.Refresh();
                costText.text = CommonUtils.CostFormat(cost);
            }
        }

        public void SetHidden(bool h)
        {
            hidden = h;
            Refresh();
        }

        public void ClearShard()
        {
            hasShard = false;
        }

        public void SetShard(ref Shard shard)
        {
            shardConrol.SetShard(ref shard);
            hasShard = true;
        }

        public ref Shard GetShard()
        {
            return ref shardConrol.GetShard();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            shardConrol.OnPointerEnter(eventData);
            m_onPointerEntered.Invoke(eventData.position);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            shardConrol.OnPointerExit(eventData);
            m_onPointerExited.Invoke(eventData.position);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            shardConrol.OnPointerDown(eventData);
            base.OnPointerClick(eventData);
            m_onPointerClicked.Invoke(eventData.position);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!canDrag) return;
            m_onDragStart.Invoke(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!canDrag) return;
            m_onDragFinish.Invoke(eventData.position);
        }

        // todo move logic to UI_ShardCollection
        public void OnDrag(PointerEventData eventData)
        {
            if (!canDrag) return;
            m_onDragMove.Invoke(eventData.position);
        }

        public bool IsHovered => shardConrol.IsHovered;
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ShardUIButton))] // If I comment this out the public variablbes show
// If it is uncommented, then just the button with "Generate" text shows
    public class ShardUIButtonEditor : ButtonEditor
    {
        private List<SerializedProperty> props;
        protected ShardUIButton shardButton;

        protected override void OnEnable()
        {
            base.OnEnable();
            shardButton = (ShardUIButton)target;
            props = new List<SerializedProperty>
            {
                serializedObject.FindProperty("m_onPointerClicked"),
                serializedObject.FindProperty("m_onPointerEntered"),
                serializedObject.FindProperty("m_onPointerExited"),
                serializedObject.FindProperty("m_onDragStart"),
                serializedObject.FindProperty("m_onDragMove"),
                serializedObject.FindProperty("m_onDragFinish"),
                serializedObject.FindProperty("plus"),
                serializedObject.FindProperty("costText"),
                serializedObject.FindProperty("showPlus"),
                serializedObject.FindProperty("hasShard"),
                serializedObject.FindProperty("cost"),
                serializedObject.FindProperty("hidden"),
                serializedObject.FindProperty("shardConrol"),
                serializedObject.FindProperty("canDrag"),
            };
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            
            serializedObject.Update();
            foreach (var prop in props)
            {
                EditorGUILayout.PropertyField(prop);
                prop.serializedObject.ApplyModifiedProperties();
            }
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Refresh"))
            {
                shardButton.Refresh();
            }

   //          if (EditorGUI.EndChangeCheck())
   //          {
   // foreach (var prop in props)
   //              {
   //                  prop.serializedObject.ApplyModifiedProperties();
   //                  Debug.Log(prop.displayName);
   //              }
   //          }

        }
    }
#endif
}