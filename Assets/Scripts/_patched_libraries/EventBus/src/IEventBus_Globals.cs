/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * This Source Code Form is "Incompatible With Secondary Licenses", as
 * defined by the Mozilla Public License, v. 2.0.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Leopotam.EcsLite
{
	public interface IEventBus_Globals : IEventBus_Subsystem
	{
		delegate void GlobalEventActionRef<T>(ref T item);

		ref T Add<T>() where T : struct, IEventGlobal;
		Task AddOnNextFrame<T>(T value) where T : struct, IEventGlobal;
		bool Has<T>() where T : struct, IEventGlobal;
		void Del<T>() where T : struct, IEventGlobal;
		void Del<T>(int globalEventEntity) where T : struct, IEventGlobal;
		EcsPool<T> GetPool<T>() where T : struct, IEventGlobal;
		EcsFilter GetFilter<T>() where T : struct, IEventGlobal;
		void ListenTo<T>(GlobalEventActionRef<T> whenEventFired) where T : struct, IEventGlobal;
		IDisposable SubscribeTo<T>(GlobalEventActionRef<T> whenEventFired) where T : struct, IEventGlobal;
		void RemoveListener<T>(GlobalEventActionRef<T> whenEventFired) where T : struct, IEventGlobal;
		
		IEcsRunSystem ProcessorFor<T>() where T : struct, IEventGlobal;

		IEcsRunSystem ProcessorAll();
	}
}