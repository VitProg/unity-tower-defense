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
using System.Threading.Tasks;

namespace Leopotam.EcsLite
{
	public class EventBus_Globals : IEventBus_Globals
	{
		private readonly List<IEcsRunSystem> _globalEventProcessors = new();
		private readonly Dictionary<Type, IGlobalEventSubscription> _globalSubscriptions = new();
		private readonly IEventBus _root;

		private readonly Dictionary<Type, EcsFilter> _cachedFilters;

		public EcsWorld GetEventsWorld() => _root.GetEventsWorld();


		public EventBus_Globals(IEventBus root, int capacityEvents)
		{
			_root = root;
			_cachedFilters = new Dictionary<Type, EcsFilter>(capacityEvents);
		}


		public ref T Add<T>() where T : struct, IEventGlobal
		{
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Verbose)) _root.Log($"GlobalEvents - Add {typeof(T).Name}");
#endif
			var newEntity = GetEventsWorld().NewEntity();
			return ref GetPool<T>().Add(newEntity);
		}
		
		public async Task AddOnNextFrame<T>(T value) where T : struct, IEventGlobal
		{
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Verbose)) _root.Log($"GlobalEvents - Add {typeof(T).Name}");
#endif
			await Task.Yield();
			var newEntity = GetEventsWorld().NewEntity();
			GetPool<T>().Add(newEntity) = value;
		}


		public bool Has<T>() where T : struct, IEventGlobal
		{
			var filter = GetFilter<T>();
			var result = filter.GetEntitiesCount() != 0;
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Debug)) _root.Log($"GlobalEvents - Has {typeof(T).Name}");
#endif
			return result;
		}


		public void Del<T>(int globalEventEntity) where T : struct, IEventGlobal
		{
			if (!GetPool<T>().Has(globalEventEntity)) return;
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Verbose)) _root.Log($"GlobalEvents - Del {typeof(T).Name}");
#endif
			GetPool<T>().Del(globalEventEntity);
		}
		
		public void Del<T>() where T : struct, IEventGlobal
		{
			var filter = GetFilter<T>();
			if (filter.GetEntitiesCount() > 0)
			{
#if DEBUG && EVENT_BUS_DEBUG
				if (_root.CanLog(LogLevel.Verbose)) _root.Log($"GlobalEvents - DelAll {typeof(T).Name}");
#endif
				foreach (var eventEntity in filter)
					GetEventsWorld().DelEntity(eventEntity);
			}
		}
		
		public void ListenTo<T>(IEventBus_Globals.GlobalEventActionRef<T> whenEventFired) where T : struct, IEventGlobal
		{
			Fetch<T>().ListenTo(whenEventFired);
		}


		private GlobalEventSubscription<T> Fetch<T>() where T : struct, IEventGlobal
		{
			if (!_globalSubscriptions.TryGetValue(typeof(T), out var eventSubscriptions))
			{
				eventSubscriptions = new GlobalEventSubscription<T>();
				_globalSubscriptions.Add(typeof(T), eventSubscriptions);
				_globalEventProcessors.Add(ProcessorFor<T>());
			}

			if (eventSubscriptions is GlobalEventSubscription<T> castedSubscription)
				return castedSubscription;
			else
				throw new InvalidOperationException();
		}


		public void RemoveListener<T>(IEventBus_Globals.GlobalEventActionRef<T> whenEventFired) where T : struct, IEventGlobal
		{
			Fetch<T>().RemoveListener(whenEventFired);
		}


		public IDisposable SubscribeTo<T>(IEventBus_Globals.GlobalEventActionRef<T> whenEventFired)
			where T : struct, IEventGlobal
		{
			return Fetch<T>().SubscribeTo(whenEventFired);
		}


		public IEcsRunSystem ProcessorFor<T>() where T : struct, IEventGlobal
		{
			return new GlobalEventSystem<T>(this, Fetch<T>());
		}


		public IEcsRunSystem ProcessorAll()
		{
			return new AllGlobalEventSystem(this);
		}


		internal void InvokeAll(IEcsSystems systems)
		{
			foreach (var eventProcessor in _globalEventProcessors) eventProcessor.Run(systems);
		}


		public EcsPool<T> GetPool<T>() where T : struct, IEventGlobal
		{
			return GetEventsWorld().GetPool<T>();
		}
		
		public EcsFilter GetFilter<T>() where T : struct, IEventGlobal
		{
			var type = typeof(T);
			if (!_cachedFilters.TryGetValue(type, out var filter))
			{
				filter = GetEventsWorld().Filter<T>().End();
				_cachedFilters.Add(type, filter);
			}

			return filter;
		}


		public void Destroy()
		{
			ReleaseAll();
		}


		public void ReleaseAll()
		{
			_globalSubscriptions.Clear();
			_globalEventProcessors.Clear();
			_cachedFilters.Clear();
		}
	}
}