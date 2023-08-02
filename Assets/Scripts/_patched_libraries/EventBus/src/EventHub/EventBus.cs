/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * This Source Code Form is "Incompatible With Secondary Licenses", as
 * defined by the Mozilla Public License, v. 2.0.
 */

using JetBrains.Annotations;
using UnityEngine;

namespace Leopotam.EcsLite
{
	public partial class EventsBus : IEventBus
	{
		private readonly EcsWorld _eventsWorld;
		private readonly LogLevel _logLevel;

		public IEventBus_Uniques Unique { get; }
		public IEventBus_Globals Global { get; }
		public IEventBus_EntityEvents Entity { get; }
		public IEventBus_FlagComponents Flag { get; }



		public EventsBus(int capacityEvents = 8, int capacityEventsSingleton = 8, [CanBeNull] EcsWorld world = null, LogLevel logLevel = LogLevel.Critical)
		{
			Unique = new EventsBus_Uniques(this, capacityEventsSingleton);
			Global = new EventBus_Globals(this, capacityEvents);
			Entity = new EventBus_EntityEvents(this, capacityEvents);
			Flag = new EventBus_FlagComponents(this);

			_logLevel = logLevel;
			
			_eventsWorld = world ?? new EcsWorld();
		}


		/// <summary>
		///     External modification of events world can lead to Unforeseen Consequences
		/// </summary>
		/// <returns></returns>
		public EcsWorld GetEventsWorld()
		{
			return _eventsWorld;
		}

		public bool CanLog(LogLevel level) => _logLevel >= level;

		public void Log(LogLevel level, string message, params object[] args)
		{
			if (!CanLog(level)) return;
			Log(message, args);
		}

		public void Log(string message, params object[] args)
		{
			Debug.Log($"EventBus: ${message}");
			if (args.Length <= 0) return;
			Debug.Log("> args:");
			foreach (var arg in args) Debug.Log(arg);
		}


		public void Destroy()
		{
			((EventsBus_Uniques)Unique).Destroy();
			((EventBus_Globals)Global).Destroy();
			((EventBus_EntityEvents)Entity).Destroy();
			((EventBus_FlagComponents)Flag).Destroy();
			_eventsWorld.Destroy();
		}


		public IEcsSystem AllEventsProcessor()
		{
			return new AllEventsProcessor(this);
		}
	}

	public enum LogLevel
	{
		Critical = 0,
		Info = 1,
		Verbose = 2,
		Debug = 3,
	}
}