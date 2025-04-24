using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    public class GameMainService : MonoBehaviour
    {
        void Awake()
        {
            var services = GetComponents<MonoBehaviour>();
            var failedServices = new List<MonoBehaviour>();
            
            foreach (var service in services)
            {
                if (service is IGameService gameService)
                {
                    if(!gameService.InitializeService())
                        failedServices.Add(service);
                }
            }

            DontDestroyOnLoad(gameObject);
            if (failedServices.Count > 0)
            {
                Debug.LogError($"Failed to load these services: {string.Join(", ", failedServices.Select(s => s.GetType().Name))}");
            }
            else
            {
                Debug.Log("All services initialized.");
            }
        }
    }
}