/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * This Source Code Form is "Incompatible With Secondary Licenses", as
 * defined by the Mozilla Public License, v. 2.0.
 */
using System;
using System.Collections.Generic;

namespace Leopotam.EcsLite
{
	public class EventBus_EntityEvents : IEventBus_EntityEvents
	{
		private readonly List<IEcsRunSystem> _entityEventProcessors = new();
		private readonly Dictionary<Type, IEntityEventSubscription> _entitySubscriptions;
		private readonly IEventBus _root;
		
		private readonly Dictionary<Type, EcsFilter> _cachedFilters = new ();

		public EcsWorld GetEventsWorld() => _root.GetEventsWorld();


		public EventBus_EntityEvents(IEventBus root, int capacityEvents = 8)
		{
			_root = root;
			_entitySubscriptions = new Dictionary<Type, IEntityEventSubscription>(capacityEvents);
		}


		public void Destroy()
		{
			ReleaseAll();
		}


		public void ReleaseAll()
		{
			_entitySubscriptions.Clear();
			_entityEventProcessors.Clear();
		}


		public ref T Add<T>(EcsPackedEntityWithWorld targetEntity, EcsPool<T> optionalCachedPool = default)
			where T : struct, IEventEntity
		{
			var newEntity = GetEventsWorld().NewEntity();

			optionalCachedPool ??= GetPool<T>();
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Verbose)) _root.Log($"EntityEvents - Add {typeof(T).Name} - Target {targetEntity.ToString()}");
#endif
			ref var component = ref optionalCachedPool.Add(newEntity);
			component.Entity = targetEntity;
			return ref component;
		}
		
		public ref T Add<T>(int targetEntity, EcsWorld targetEntityWorld, EcsPool<T> optionalCachedPool = default)
			where T : struct, IEventEntity
		{
			var newEntity = GetEventsWorld().NewEntity();
			
			optionalCachedPool ??= GetPool<T>();
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Verbose)) _root.Log($"EntityEvents - Add {typeof(T).Name} - Target {targetEntity.ToString()}");
#endif
			ref var component = ref optionalCachedPool.Add(newEntity);
			component.Entity = targetEntityWorld.PackEntityWithWorld(targetEntity);
			return ref component;
		}

		public bool Has<T>(EcsPool<T> cachedPool = default)
			where T : struct, IEventEntity
		{
			var result = GetFilter<T>().GetEntitiesCount() > 0;
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Debug)) _root.Log($"EntityEvents - Has {typeof(T).Name} - Result {result}");
#endif
			return result;
		}
		
		// public bool Has<T>(EcsPackedEntityWithWorld targetEntity, EcsPool<T> optionalCachedPool = default)
			// where T : struct, IEventEntity
		// {
			// optionalCachedPool ??= GetPool<T>();
			// foreach (var entity in GetFilter<T>())
			// {
				// if (optionalCachedPool.Get(entity))
			// }
		// }
		
		// public bool Has<T>(int targetEntity, EcsWorld targetEntityWorld, EcsPool<T> optionalCachedPool = default)
			// where T : struct, IEventEntity
		// {
			
		// }

		public EcsPool<T> GetPool<T>() where T : struct, IEventEntity
		{
			return GetEventsWorld().GetPool<T>();
		}
		
		public EcsFilter GetFilter<T>() where T : struct, IEventEntity
		{
			var type = typeof(T);
			if (!_cachedFilters.TryGetValue(type, out var filter))
			{
				filter = GetEventsWorld().Filter<T>().End();
				_cachedFilters.Add(type, filter);
			}

			return filter;
		}

		// -------------------------------------------------------------------------------------------------- //
		
		private EntityEventSubscription<T> Fetch<T>() where T : struct, IEventEntity
		{
			IEntityEventSubscription eventSubscriptions = default;
			if (!_entitySubscriptions.TryGetValue(typeof(T), out eventSubscriptions))
			{
				eventSubscriptions = new EntityEventSubscription<T>();
				_entitySubscriptions.Add(typeof(T), eventSubscriptions);
				_entityEventProcessors.Add(ProcessorFor<T>());
			}

			if (eventSubscriptions is EntityEventSubscription<T> castedSubscription)
				return castedSubscription;

			throw new InvalidOperationException();
		}


		public void ListenTo<T>(EcsPackedEntityWithWorld targetEntity, IEventBus_EntityEvents.EntityEventActionRef<T> whenEventFired) where T : struct, IEventEntity
		{
			Fetch<T>().ListenTo(targetEntity, whenEventFired);
		}
		
		public void ListenTo<T>(IEventBus_EntityEvents.EntityEventActionRef<T> whenEventFired) where T : struct, IEventEntity
		{
			Fetch<T>().ListenTo(whenEventFired);
		}


		public IDisposable SubscribeTo<T>(EcsPackedEntityWithWorld targetEntity,
			IEventBus_EntityEvents.EntityEventActionRef<T> whenEventFired) where T : struct, IEventEntity
		{
			return Fetch<T>().SubscribeTo(targetEntity, whenEventFired);
		}

		public IDisposable SubscribeTo<T>(IEventBus_EntityEvents.EntityEventActionRef<T> whenEventFired) where T : struct, IEventEntity
		{
			return Fetch<T>().SubscribeTo(whenEventFired);
		}


		public void RemoveListener<T>(EcsPackedEntityWithWorld targetEntity, IEventBus_EntityEvents.EntityEventActionRef<T> whenEventFired) where T : struct, IEventEntity
		{
			Fetch<T>().RemoveListener(targetEntity, whenEventFired);
		}

		public void RemoveListener<T>(IEventBus_EntityEvents.EntityEventActionRef<T> whenEventFired) where T : struct, IEventEntity
		{
			Fetch<T>().RemoveListener(whenEventFired);
		}


		public IEcsRunSystem ProcessorFor<T>() where T : struct, IEventEntity
		{
			return new EntityEventSystem<T>(this);
		}


		public IEcsRunSystem ProcessorAll()
		{
			return new AllEntityEventSystem(this);
		}


		internal bool GetSubscriptionData<T>(out IEntityEventSubscription subscription)
		{
			return _entitySubscriptions.TryGetValue(typeof(T), out subscription);
		}


		internal void InvokeAll(IEcsSystems systems)
		{
			foreach (var eventProcessor in _entityEventProcessors) eventProcessor.Run(systems);
		}
	}
}