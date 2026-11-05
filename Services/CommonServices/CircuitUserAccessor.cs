using Microsoft.AspNetCore.Components.Server.Circuits;
using System.Collections.Concurrent;

namespace Lumen.Services.CommonServices
{
    public class CircuitUserAccessor : CircuitHandler
    {
        // Maps the Circuit ID to the User ID
        private static readonly ConcurrentDictionary<string, string> _circuitUsers = new();

        public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        // We can't easily get the user here without DI, so we use a middleware approach below.
        public static void RegisterUser(string circuitId, string userId)
        {
            if (!string.IsNullOrEmpty(circuitId) && !string.IsNullOrEmpty(userId))
                _circuitUsers[circuitId] = userId;
        }

        public static string? GetUser(string circuitId)
        {
            return _circuitUsers.TryGetValue(circuitId, out var id) ? id : null;
        }

        public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _circuitUsers.TryRemove(circuit.Id, out _);
            return Task.CompletedTask;
        }
    }
}
