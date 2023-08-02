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
using System.Threading;

namespace Leopotam.EcsLite
{
	public class EventBus_FlagComponents : IEventBus_FlagComponents
	{
		private readonly List<IEcsRunSystem> _flagComponentProcessors = new();
		private readonly Dictionary<Type, IFlagComponentSubscription> _flagComponentSubscriptions = new();

		private readonly IEventBus _root;

		public EcsWorld GetEventsWorld() => _root.GetEventsWorld();



		public EventBus_FlagComponents(IEventBus root)
		{
			_root = root;
		}
		
		
		public void Destroy()
		{
			ReleaseAll();
		}


		public void ReleaseAll()
		{
			_flagComponentSubscriptions.Clear();
			_flagComponentProcessors.Clear();
		}


		/// <summary>
		/// Handles adding a flag component to an entity.  Note:  A higher performance variant exists.
		/// </summary>
		/// <param name="targetEntity"></param>
		/// <param name="optionalCachedPool"></param>
		/// <typeparam name="T"></typeparam>
		public ref T Add<T>(EcsPackedEntityWithWorld targetEntity, EcsPool<T> optionalCachedPool = default)
			where T : struct, IFlagComponent
		{
			if (!targetEntity.Unpack(out var world, out var entity)) throw new NullReferenceException();
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Verbose)) _root.Log($"FlagComponent - Add {typeof(T).Name} - Target {targetEntity.ToString()}");
#endif
			optionalCachedPool ??= world.GetPool<T>();
			if (optionalCachedPool.Has(entity))
				return ref optionalCachedPool.Get(entity);
			return ref optionalCachedPool.Add(entity);
		}
		
		public void Del<T>(EcsPackedEntityWithWorld targetEntity, EcsPool<T> optionalCachedPool = default)
			where T : struct, IFlagComponent
		{
			if (!targetEntity.Unpack(out var world, out var entity)) return;
			optionalCachedPool ??= world.GetPool<T>();
			if (optionalCachedPool.Has(entity)) return;
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Verbose)) _root.Log($"FlagComponent - Del {typeof(T).Name} - Target {targetEntity.ToString()}");
#endif	
			optionalCachedPool.Del(entity);
		}

		public bool Has<T>(EcsPackedEntityWithWorld targetEntity, EcsPool<T> optionalCachedPool = default)
			where T : struct, IFlagComponent
		{
			var result = false;
			
			if (targetEntity.Unpack(out var world, out var entity))
			{
				optionalCachedPool ??= world.GetPool<T>();
				result = !optionalCachedPool.Has(entity);
			}
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Debug)) _root.Log( $"FlagComponent - Has {typeof(T).Name} - Target {targetEntity.ToString()} - Result {result}");
#endif
			return result;
		}


		/// <summary>
		/// A higher performance variant of AddFlagComponent.  Adds the flag, but takes the entity and pool directly.
		/// </summary>
		/// <param name="targetEntity"></param>
		/// <param name="cachedPool"></param>
		/// <typeparam name="T"></typeparam>
		public ref T Add<T>(int targetEntity, EcsPool<T> cachedPool) where T : struct, IFlagComponent
		{
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Verbose)) _root.Log($"FlagComponent - Add {typeof(T).Name} - Target {targetEntity.ToString()}");
#endif
			if (cachedPool.Has(targetEntity))
				return ref cachedPool.Get(targetEntity);
			return ref cachedPool.Add(targetEntity);
		}
		
		public void Del<T>(int targetEntity, EcsPool<T> cachedPool) where T : struct, IFlagComponent
        {
	        if (!cachedPool.Has(targetEntity))
	        {
#if DEBUG && EVENT_BUS_DEBUG   
		        if (_root.CanLog(LogLevel.Verbose)) _root.Log($"FlagComponent - Del {typeof(T).Name} - Target {targetEntity.ToString()}");
#endif
		        cachedPool.Del(targetEntity);
	        }
        }
		
		public bool Has<T>(int targetEntity, EcsPool<T> cachedPool) where T : struct, IFlagComponent
		{
			var result = cachedPool.Has(targetEntity);
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Debug)) _root.Log($"FlagComponent - Has {typeof(T).Name} - Target {targetEntity.ToString()} - Result {result}");
#endif
			return result;
		}

		public ref T Add<T>(int targetEntity, EcsWorld world, EcsPool<T> optionalCachedPool = default) where T : struct, IFlagComponent
		{
			optionalCachedPool ??= world.GetPool<T>();
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Verbose)) _root.Log($"FlagComponent - Add {typeof(T).Name} - Target {targetEntity.ToString()}");
#endif
			if (optionalCachedPool.Has(targetEntity))
				return ref optionalCachedPool.Get(targetEntity);
			return ref optionalCachedPool.Add(targetEntity);
		}
		
		public void Del<T>(int targetEntity, EcsWorld world, EcsPool<T> optionalCachedPool = default) where T : struct, IFlagComponent
        {
	        optionalCachedPool ??= world.GetPool<T>();
	        if (!optionalCachedPool.Has(targetEntity))
	        {
#if DEBUG && EVENT_BUS_DEBUG
		        if (_root.CanLog(LogLevel.Verbose)) _root.Log($"FlagComponent - Del {typeof(T).Name} - Target {targetEntity.ToString()}");
#endif
		        optionalCachedPool.Del(targetEntity);
	        }
        }

		public bool Has<T>(int targetEntity, EcsWorld world, EcsPool<T> optionalCachedPool = default) where T : struct, IFlagComponent
		{
			optionalCachedPool ??= world.GetPool<T>();
			var result = !optionalCachedPool.Has(targetEntity);
#if DEBUG && EVENT_BUS_DEBUG
			if (_root.CanLog(LogLevel.Debug)) _root.Log($"FlagComponent - Has {typeof(T).Name} - Target {targetEntity.ToString()} - Result {result}");
#endif
			return result;
		}


		private FlagComponentSubscription<T> Fetch<T>(EcsPackedEntityWithWorld targetEntity) where T : struct, IFlagComponent
		{
			if (!targetEntity.Unpack(out var targetWorld, out var targetEntityID))
			{
				return null;
			}
			
			if (!_flagComponentSubscriptions.TryGetValue(typeof(T), out var flagComponentSubscription))
			{
				flagComponentSubscription = new FlagComponentSubscription<T>();
				_flagComponentSubscriptions.Add(typeof(T), flagComponentSubscription);
				_flagComponentProcessors.Add(ProcessorFor<T>(targetWorld));
			}

			if (flagComponentSubscription is FlagComponentSubscription<T> castedSubscription)
				return castedSubscription;
			throw new InvalidOperationException();
		}


		public void ListenTo<T>(EcsPackedEntityWithWorld targetEntity, Action<EcsPackedEntityWithWorld> whenEventFired) where T : struct, IFlagComponent
		{
			Fetch<T>(targetEntity).ListenTo(targetEntity, whenEventFired);
		}


		public IDisposable SubscribeTo<T>(EcsPackedEntityWithWorld targetEntity, Action<EcsPackedEntityWithWorld> whenEventFired) where T : struct, IFlagComponent
		{
			return Fetch<T>(targetEntity).SubscribeTo(targetEntity, whenEventFired);
		}


		public void RemoveListener<T>(EcsPackedEntityWithWorld targetEntity, Action<EcsPackedEntityWithWorld> whenEventFired) where T : struct, IFlagComponent
		{
			Fetch<T>(targetEntity)?.RemoveListener(targetEntity, whenEventFired);
		}


		public IEcsRunSystem ProcessorFor<T>(EcsWorld world) where T : struct, IFlagComponent
		{
			return new FlagComponentSystem<T>(this, world);
		}


		public IEcsRunSystem ProcessorAll()
		{
			return new AllFlagComponentSystem(this);
		}
		

		internal bool GetSubscriptionData<T>(out IFlagComponentSubscription subscription)
		{
			subscription = default;

			return _flagComponentSubscriptions.TryGetValue(typeof(T), out subscription);
		}
		

		internal void InvokeAll(IEcsSystems systems)
		{
			foreach (var flagComponentProcessor in _flagComponentProcessors)
			{
				flagComponentProcessor.Run(systems);
			}			
		}
	}
}