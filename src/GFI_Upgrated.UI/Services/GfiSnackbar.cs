using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GFI_Upgrated.UI.Services
{
    public class GfiSnackbar : ISnackbar
    {
        private readonly SnackbarService _original;

        public GfiSnackbar(SnackbarService original)
        {
            _original = original;
        }

        public IEnumerable<Snackbar> ShownSnackbars => _original.ShownSnackbars;

        public SnackbarConfiguration Configuration => _original.Configuration;

        public event Action? OnSnackbarsUpdated
        {
            add => _original.OnSnackbarsUpdated += value;
            remove => _original.OnSnackbarsUpdated -= value;
        }

        private bool ShouldSuppress(string? message)
        {
            if (string.IsNullOrWhiteSpace(message)) return false;
            
            // Suppress session expiration or unauthorized messages
            if (message.Contains("session has expired", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("session expired", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("unauthorized", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        public Snackbar? Add(string message, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null, string? key = null)
        {
            if (ShouldSuppress(message))
            {
                return null;
            }
            return _original.Add(message, severity, configure, key);
        }

        public Snackbar? Add(RenderFragment message, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null, string? key = null)
        {
            return _original.Add(message, severity, configure, key);
        }

        public Snackbar? Add(MarkupString message, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null, string? key = null)
        {
            if (ShouldSuppress(message.Value))
            {
                return null;
            }
            return _original.Add(message, severity, configure, key);
        }

        public Snackbar? Add<T>(Dictionary<string, object>? componentParameters = null, Severity severity = Severity.Normal, Action<SnackbarOptions>? configure = null, string? key = null) where T : IComponent
        {
            return _original.Add<T>(componentParameters, severity, configure, key);
        }

        public void Remove(Snackbar snackbar)
        {
            _original.Remove(snackbar);
        }

        public void RemoveByKey(string key)
        {
            _original.RemoveByKey(key);
        }

        public void Clear()
        {
            _original.Clear();
        }

        public void Dispose()
        {
            _original.Dispose();
        }
    }
}
