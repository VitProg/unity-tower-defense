/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * This Source Code Form is "Incompatible With Secondary Licenses", as
 * defined by the Mozilla Public License, v. 2.0.
 */
namespace Leopotam.EcsLite
{
	public static class EventBusExt
	{
		public static IEcsSystems AddWorld(this IEcsSystems systems, IEventBus bus, string worldName = "__EVENTS")
		{
			bus.Log(LogLevel.Verbose, $"AddWorld: '{worldName}'");
			systems.AddWorld(bus.GetEventsWorld(), worldName);
			return systems;
		}
		
		public static IEcsSystems AddAllEvents(this IEcsSystems systems, IEventBus bus)
		{
			bus.Log(LogLevel.Verbose, $"AddAllEvents");
			systems.Add(bus.AllEventsProcessor());
			return systems;
		}
	}
}