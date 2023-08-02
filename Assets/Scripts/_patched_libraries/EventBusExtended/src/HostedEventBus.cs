using UnityEngine;

namespace Leopotam.EcsLite
{
	public class HostedEventBus : MonoBehaviour, IHostedService, IEventBus
	{
		IEventBus _bus = new EventsBus();


		public EcsWorld GetEventsWorld() => _bus.GetEventsWorld();
		public void Log(string message, params object[] args)
		{	
		}

		public void Log(LogLevel level, string message, params object[] args)
		{
		}

		public bool CanLog(LogLevel level)
		{
			return false;
		}


		public void Destroy() => _bus.Destroy();

		public IEventBus_Uniques Unique => _bus.Unique;
		public IEventBus_Globals Global => _bus.Global;
		public IEventBus_EntityEvents Entity => _bus.Entity;

		public IEventBus_FlagComponents Flag => _bus.Flag;
		
		
		public IEcsSystem AllEventsProcessor() => _bus.AllEventsProcessor();
	}
}