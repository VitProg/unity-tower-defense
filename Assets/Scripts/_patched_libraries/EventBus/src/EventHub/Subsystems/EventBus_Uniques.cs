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
	public class EventsBus_Uniques : IEventBus_Uniques
	{
		private readonly IEventBus _root;
		private readonly Dictionary<Type, int> _uniqueEntities;
		private readonly List<IEcsRunSystem> _uniqueEventProcessors = new();
		private readonly Dictionary<Type, IUniqueEventSubscription> _uniqueSubscriptions = new();
		private readonly Dictionary<Type, EcsFilter> _cachedFilters = new(8);

		public EventsBus_Uniques(IEventBus root, int capacityEventsSingleton)
		{
			_root = root;
			_uniqueEntities = new Dictionary<Type, int>(capacityEventsSingleton);
		}


		public EcsWorld GetEventsWorld()
		{
			return _root.GetEventsWorld();
		}


		public ref T Add<T>() where T : struct, IEventUnique
		{
			var type = typeof(T);
			var eventsPool = GetEventsWorld().GetPool<T>();
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Verbose)) _root.Log($"UniqueEvents - Add {type.Name}");
#endif
			if (!_uniqueEntities.TryGetValue(type, out var eventEntity))
			{
				eventEntity = GetEventsWorld().NewEntity();
				_uniqueEntities.Add(type, eventEntity);
				return ref eventsPool.Add(eventEntity);
			}

			return ref eventsPool.Get(eventEntity);
		}


		public bool Has<T>() where T : struct, IEventUnique
		{
			var type = typeof(T);
			var result = _uniqueEntities.ContainsKey(type);
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Debug)) _root.Log($"UniqueEvents - Has {type.Name} - Result {result}");
#endif
			return result;
		}


		public void Del<T>() where T : struct, IEventUnique
		{
			var type = typeof(T);

			if (!_uniqueEntities.TryGetValue(type, out var eventEntity)) return;
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Verbose)) _root.Log($"UniqueEvents - Del {type.Name}");
#endif
			GetEventsWorld().DelEntity(eventEntity);
			_uniqueEntities.Remove(type);
		}

		public ref T Get<T>() where T : struct, IEventUnique
		{
			var type = typeof(T);

			if (_uniqueEntities.TryGetValue(type, out var eventEntity))
			{
				var eventsPool = GetEventsWorld().GetPool<T>();
				ref var result = ref eventsPool.Get(eventEntity);
#if DEBUG && EVENT_BUS_DEBUG
				if (_root.CanLog(LogLevel.Verbose)) _root.Log($"UniqueEvents - Get {type.Name}");
#endif
				return ref result;
			}

			throw new NullReferenceException($"Unique event {type} not exist! Use the `Has` method to make sure the global event is present");
		}
		
		public EcsFilter GetFilter<T>() where T : struct, IEventUnique
		{
			var type = typeof(T);
			if (!_cachedFilters.TryGetValue(type, out var filter))
			{
				filter = GetEventsWorld().Filter<T>().End();
				_cachedFilters.Add(type, filter);
			}

			return filter;
		}


		/// <summary>
		///     Subscribes to a unique event without tracking, which means it is up to you to handle unsubscribing
		///     appropriately (don't use lambdas)
		/// </summary>
		/// <param name="whenEventFired"></param>
		/// <typeparam name="T"></typeparam>
		public void ListenTo<T>(IEventBus_Uniques.UniqueEventActionRef<T> whenEventFired) where T : struct, IEventUnique
		{
			Fetch<T>().ListenTo(whenEventFired);
		}


		/// <summary>
		///     Subscribes to a unique event with tracking, which means simply dispose the return value to
		///     end the subscription.  A small amount of memory is allocated to contain the tracking information (2 ints).
		/// </summary>
		/// <param name="whenEventFired"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IDisposable SubscribeTo<T>(IEventBus_Uniques.UniqueEventActionRef<T> whenEventFired)
			where T : struct, IEventUnique
		{
			return Fetch<T>().SubscribeTo(whenEventFired);
		}


		/// <summary>
		///     When using the non-tracking subscription call, you have to manually call Unsubscribe to end a subscription
		/// </summary>
		/// <param name="whenEventFired"></param>
		/// <typeparam name="T"></typeparam>
		public void RemoveListener<T>(IEventBus_Uniques.UniqueEventActionRef<T> whenEventFired)
			where T : struct, IEventUnique
		{
			Fetch<T>().RemoveListener(whenEventFired);
		}


		public IEcsRunSystem ProcessorFor<T>() where T : struct, IEventUnique
		{
			return new UniqueEventSystem<T>(this, Fetch<T>());
		}


		public IEcsRunSystem ProcessorAll()
		{
			return new AllUniqueEventSystem(this);
		}


		public void Destroy()
		{
			ReleaseAll();
		}


		public void ReleaseAll()
		{
			_uniqueSubscriptions.Clear();
			_uniqueEventProcessors.Clear();
			_uniqueEntities.Clear();
		}


		private UniqueEventSubscription<T> Fetch<T>() where T : struct, IEventUnique
		{
			IUniqueEventSubscription eventSubscriptions = default;
			if (!_uniqueSubscriptions.TryGetValue(typeof(T), out eventSubscriptions))
			{
				eventSubscriptions = new UniqueEventSubscription<T>();
				_uniqueSubscriptions.Add(typeof(T), eventSubscriptions);
				_uniqueEventProcessors.Add(ProcessorFor<T>());
			}

			if (eventSubscriptions is UniqueEventSubscription<T> castedSubscription)
				return castedSubscription;
			throw new InvalidOperationException();
		}


		internal void InvokeAll(IEcsSystems systems)
		{
			foreach (var eventProcessor in _uniqueEventProcessors) eventProcessor.Run(systems);
		}
	}
}